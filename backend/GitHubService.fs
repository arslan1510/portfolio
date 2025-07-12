module GitHubService

open System.Net.Http
open System.Text.Json
open System.Threading.Tasks
open Models



let private httpClient =
    let client = new HttpClient()
    client.DefaultRequestHeaders.Add("User-Agent", "F#-Portfolio-App")
    client

let private parseGitHubRepos (json: string) : Result<GitHubRepo[], string> =
    try
        let options =
            JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

        match JsonSerializer.Deserialize<GitHubRepo[]>(json, options) with
        | null -> Error "Failed to parse GitHub response"
        | repos -> Ok repos
    with ex ->
        Error $"JSON parsing error: {ex.Message}"

let private fetchRepoLanguages (username: string) (repoName: string) : Task<string[]> =
    task {
        try
            let url = $"https://api.github.com/repos/{username}/{repoName}/languages"
            let! response = httpClient.GetAsync(url)
            
            if response.IsSuccessStatusCode then
                let! json = response.Content.ReadAsStringAsync()
                let languageDict = JsonSerializer.Deserialize<Map<string, int>>(json)
                match languageDict with
                | null -> return [||]
                | dict -> return dict |> Map.keys |> Array.ofSeq
            else
                return [||]
        with
        | _ -> return [||]
    }

let private convertToProject (username: string) (repo: GitHubRepo) : Task<Project> =
    task {
        let! languages = fetchRepoLanguages username repo.name
        let allTechnologies = 
            Array.concat [
                languages
                repo.topics
            ]
            |> Array.distinct
            |> Array.filter (fun tech -> not (System.String.IsNullOrWhiteSpace(tech)))
        
        return {
            Title = repo.name
            Description = repo.description |> Option.defaultValue "No description available"
            Technologies = allTechnologies
            GitHubUrl = repo.html_url
        }
    }

let private filterAndSortRepos (repos: GitHubRepo[]) : GitHubRepo[] =
    let isQualityRepo repo =
        not repo.fork
        && (repo.description.IsSome || repo.language.IsSome || repo.topics.Length > 0)

    let calculateRelevance repo =
        let langScore = if repo.language.IsSome then 5 else 0
        let topicScore = repo.topics.Length
        let descScore = if repo.description.IsSome then 3 else 0
        langScore + topicScore + descScore

    repos |> Array.filter isQualityRepo |> Array.sortByDescending calculateRelevance

let private fetchFromGitHub (username: string) : Task<Result<string, string>> =
    task {
        try
            let url = $"https://api.github.com/users/{username}/repos?type=public&sort=updated"
            let! response = httpClient.GetAsync(url)

            if response.IsSuccessStatusCode then
                let! json = response.Content.ReadAsStringAsync()
                return Ok json
            else
                return Error $"GitHub API failed with status: {response.StatusCode}"
        with ex ->
            return Error $"Network error: {ex.Message}"
    }

let fetchPublicRepos (username: string) : Task<Result<Project[], string>> =
    task {
        let! result = fetchFromGitHub username

        match result with
        | Error error -> return Error error
        | Ok json ->
            match parseGitHubRepos json with
            | Error error -> return Error error
            | Ok repos ->
                let filteredRepos = filterAndSortRepos repos
                let! projects = 
                    filteredRepos 
                    |> Array.map (convertToProject username)
                    |> Task.WhenAll
                return Ok projects
    }

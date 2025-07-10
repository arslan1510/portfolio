module GitHubService

open System.Net.Http
open System.Text.Json
open System.Threading.Tasks
open Models



let private httpClient =
    let client = new HttpClient()
    client.DefaultRequestHeaders.Add("User-Agent", "F#-Portfolio-App")
    client

// Pure functions for transformations
let private parseGitHubRepos (json: string) : Result<GitHubRepo[], string> =
    try
        let options =
            JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

        match JsonSerializer.Deserialize<GitHubRepo[]>(json, options) with
        | null -> Error "Failed to parse GitHub response"
        | repos -> Ok repos
    with ex ->
        Error $"JSON parsing error: {ex.Message}"

let private convertToProject (repo: GitHubRepo) : Project =
    { Title = repo.name
      Description = repo.description |> Option.defaultValue "No description available"
      Technologies =
        repo.language
        |> Option.map (fun lang -> Array.append [| lang |] repo.topics)
        |> Option.defaultValue repo.topics
      GitHubUrl = repo.html_url }

let private filterAndSortRepos (repos: GitHubRepo[]) : GitHubRepo[] =
    let isQualityRepo repo =
        // More inclusive filter - just need either description or language, but exclude forks
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

let fetchPublicRepos (username: string) : Task<Project[]> =
    task {
        let! result = fetchFromGitHub username

        return
            result
            |> Result.bind parseGitHubRepos
            |> Result.map (filterAndSortRepos >> Array.map convertToProject)
            |> Result.defaultWith (fun _ -> [||])
    }

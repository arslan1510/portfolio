module TerminalCommands

open System
open Models
open GitHubService

type CommandResult =
    | Success of string
    | Error of string
    | ProjectList of Project[]
    | SkillsInfo of Map<string, string list>

// Constants
module Constants =
    let helpText =
        """
Available commands:
  help              Show this help message
  about             Display information about me
  github            List all GitHub repositories
  skills [category] Display technical skills
  contact           Show contact information
  open <number|url> Open a project or URL
  clear             Clear the terminal
  whoami            Display current user
  pwd               Print working directory
  date              Display current date and time
    """

    let aboutText =
        """<div class="about-section">Hi there, I'm Arslan Khan, and I love solving formal problems. 
I have expertise in 
    - Mathematics, 
    - Computer Science, and 
    - Software Engineering,

with particular interests in 
    - Mathematical Modelling, 
    - Functional Programming, and 
    - Machine Learning. 

Currently, I'm working as a Software Engineer & Applied Researcher at AgenticAI.
Feel free to reach out for collaborations or opportunities!
</div>"""

    let contactInfo =
        """
Email:    alephnull.dev@gmail.com
GitHub:   github.com/arslan1510
LinkedIn: linkedin.com/in/arslan-khan-411779266
    """

    let skills =
        Map.ofList
            [ "languages", [ "Python"; "F#"; "Haskell" ]
              "frameworks", [ "Oxpecker"; ".NET"; "FastAPI"; "NumPy"; "Pandas"; "PyTorch" ]
              "tools", [ "Git"; "Fish" ]
              "mathematics (pure & applied)",
              [ "Category Theory"
                "Analysis"
                "Linear & Abstract Algebra"
                "Statistics"
                "Graph Theory"
                "Combinatorics" ]
              "databases", [ "Neo4j"; "PostgreSQL" ]
              "cloud", [ "AWS" ]
              "devops", [ "Docker" ]
              "os", [ "Linux" ] ]

// Command handlers
let private handleOpen (args: string[]) =
    match args |> Array.tryHead with
    | Some arg when arg.StartsWith("http://") || arg.StartsWith("https://") -> Success $"OPEN_URL:{arg}"
    | Some projectNum ->
        let mutable num = 0

        if System.Int32.TryParse(projectNum, &num) then
            Success $"OPEN_PROJECT:{projectNum}"
        else
            Error $"Cannot open: {projectNum}. Use 'open <project-number>' or 'open <url>'"
    | None -> Error "Usage: open <project-number|url>"

let private handleCd args =
    match args |> Array.tryHead with
    | Some dir when dir = ".." || dir = "~" -> Success ""
    | Some dir when [ "github"; "skills" ] |> List.contains dir -> Success ""
    | Some dir -> Error $"cd: {dir}: No such file or directory"
    | None -> Success ""

let private handleUname args =
    let info =
        if args |> Array.contains "-a" then
            "Portfolio OS 1.0.0 portfolio-terminal F# Oxpecker.NET x86_64"
        else
            "Portfolio OS"

    Success info

let executeCommand (command: string) (args: string[]) =
    task {
        match command.ToLower() with
        | "help" -> return Success Constants.helpText
        | "about" -> return Success Constants.aboutText
        | "github" ->
            let! projects = fetchPublicRepos "arslan1510"
            return ProjectList projects
        | "skills" -> return SkillsInfo Constants.skills
        | "contact" -> return Success Constants.contactInfo
        | "whoami" -> return Success "guest"
        | "pwd" -> return Success "/home/guest"
        | "date" -> return Success(DateTime.Now.ToString())
        | "uname" -> return handleUname args
        | "echo" -> return Success(String.Join(" ", args))
        | "open" -> return handleOpen args
        | "cd" -> return handleCd args
        | "clear" -> return Success "CLEAR_TERMINAL"
        | cmd -> return Error $"Command not found: {cmd}. Type 'help' for available commands."
    }

let formatResult (result: CommandResult) =
    match result with
    | Success text ->
        {| status = "success"
           output = text
           data = null |}
    | Error text ->
        {| status = "error"
           output = text
           data = null |}
    | ProjectList projects ->
        {| status = "success"
           output = ""
           data = box projects |}
    | SkillsInfo skills ->
        {| status = "success"
           output = ""
           data = box skills |}

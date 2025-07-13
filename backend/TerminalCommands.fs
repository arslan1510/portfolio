module TerminalCommands

open System
open Models
open GitHubService
open DataLoader

let executeCommand (command: string) (args: string[]) =
    task {
        match command.ToLower() with
        | "help" -> return Ok(loadHelpText ())
        | "about" -> return Ok(loadAboutText ())
        | "github" ->
            let! result = fetchPublicRepos "arslan1510"

            match result with
            | Ok projects -> return Ok(System.Text.Json.JsonSerializer.Serialize(projects))
            | Error error -> return Error error
        | "skills" ->
            try
                let skills = loadSkills ()
                return Ok(System.Text.Json.JsonSerializer.Serialize(skills))
            with ex ->
                return Error $"Error loading skills: {ex.Message}"
        | "contact" -> return Ok(loadContactInfo ())
        | "whoami" -> return Ok "guest"
        | "pwd" -> return Ok "/home/guest"
        | "date" -> return Ok(DateTime.Now.ToString())
        | "echo" -> return Ok(String.Join(" ", args))
        | "clear" -> return Ok "CLEAR_TERMINAL"
        | "fastfetch" -> return Ok(loadFastFetchData ())
        | cmd -> return Error $"Command not found: {cmd}. Type 'help' for available commands."
    }

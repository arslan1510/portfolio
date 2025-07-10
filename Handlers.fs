module Handlers

open Oxpecker

[<CLIMutable>]
type TerminalRequest =
    { command: string
      args: string[] option }

let private loadTemplates () =
    let layout = TemplateService.tryReadTemplate "terminal-layout.html"
    let content = TemplateService.tryReadTemplate "terminal.html"

    match layout, content with
    | Ok layout, Ok content -> Ok(layout.Replace("{{TITLE}}", "Arslan Khan").Replace("{{CONTENT}}", content))
    | Error msg, _
    | _, Error msg -> Error msg

let terminalPage: EndpointHandler =
    fun ctx ->
        task {
            match loadTemplates () with
            | Ok html -> return! htmlString html ctx
            | Error msg ->
                ctx.SetStatusCode 500 |> ignore
                return! text $"Error loading terminal: {msg}" ctx
        }

let terminalCommandApi: EndpointHandler =
    fun ctx ->
        task {
            try
                let! requestBody = ctx.BindJson<TerminalRequest>()
                let args = requestBody.args |> Option.defaultValue [||]
                let! result = TerminalCommands.executeCommand requestBody.command args
                return! json (TerminalCommands.formatResult result) ctx
            with ex ->
                return!
                    json
                        {| status = "error"
                           output = $"Error: {ex.Message}"
                           data = null |}
                        ctx
        }

let terminalGitHubApi: EndpointHandler =
    fun ctx ->
        task {
            try
                let! projects = GitHubService.fetchPublicRepos "arslan1510"
                return! json projects ctx
            with ex ->
                ctx.Response.StatusCode <- 500
                return! json {| error = ex.Message |} ctx
        }

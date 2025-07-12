module Handlers

open Microsoft.AspNetCore.Http
open System.Threading.Tasks
open Oxpecker.ViewEngine
open Oxpecker.Htmx
open Oxpecker
open TerminalView
open TerminalCommands

let terminalHandler: EndpointHandler =
    fun (ctx: HttpContext) -> ctx.WriteHtmlView terminalPage

let terminalCommandHandler: EndpointHandler =
    fun (ctx: HttpContext) ->
        task {
            let command = ctx.TryGetFormValue "command"

            match command with
            | Some cmd when not (System.String.IsNullOrWhiteSpace(cmd)) ->
                let parts = cmd.Split(' ')
                let commandName = parts.[0]
                let args = parts |> Array.skip 1

                let! result = executeCommand commandName args

                // Create command echo + result response
                let responseView =
                    div () {
                        // Show what command was typed
                        div (class' = "terminal-line command") { cmd }
                        // Show the result
                        renderCommandResponse result
                        // Clear and refocus input using HTMX out-of-band swap
                        div (hxSwapOob = "innerHTML:#terminal-prompt") {
                            span (class' = "prompt-user") { "guest" }
                            span (class' = "prompt-at") { "@" }
                            span (class' = "prompt-host") { "portfolio" }
                            span (class' = "prompt-separator") { ":" }
                            span (class' = "prompt-path") { "~" }
                            span (class' = "prompt-symbol") { "$ " }

                            input (
                                type' = "text",
                                id = "terminal-input",
                                class' = "terminal-input",
                                name = "command",
                                value = "",
                                autocomplete = "off",
                                autofocus = true,
                                hxPost = "/api/terminal/command",
                                hxTarget = "#terminal-output",
                                hxSwap = "beforeend scroll:#terminal-output:bottom",
                                hxTrigger = "keyup[key=='Enter']",
                                hxIndicator = "#terminal-loading"
                            )

                            span (id = "terminal-loading", class' = "terminal-loading", style = "display: none;") { "..." }
                            
                            span (id = "terminal-prompt-time", class' = "terminal-prompt-time", 
                                  hxGet = "/api/terminal/time",
                                  hxTrigger = "load, every 1s",
                                  hxSwap = "innerHTML") { 
                                System.DateTime.Now.ToString("HH:mm:ss") 
                            }
                        }
                    }

                return! ctx.WriteHtmlView responseView
            | _ ->
                // Empty command - return empty response
                let emptyView = div () { }
                return! ctx.WriteHtmlView emptyView
        }

let terminalGitHubApi: EndpointHandler =
    fun ctx ->
        task {
            try
                let! result = GitHubService.fetchPublicRepos "arslan1510"

                match result with
                | Ok projects -> return! ctx.WriteJson projects
                | Error error ->
                    ctx.Response.StatusCode <- 500
                    return! ctx.WriteJson {| error = error |}
            with ex ->
                ctx.Response.StatusCode <- 500
                return! ctx.WriteJson {| error = ex.Message |}
        }

let terminalTimeHandler: EndpointHandler =
    fun (ctx: HttpContext) ->
        task {
            let timeString = System.DateTime.Now.ToString("HH:mm:ss")
            return! ctx.WriteText timeString
        }

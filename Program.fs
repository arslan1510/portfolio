open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Oxpecker

let configureApp (app: WebApplication) =
    if app.Environment.IsDevelopment() then
        app.UseDeveloperExceptionPage() |> ignore

    app.UseStaticFiles() |> ignore
    app.UseRouting() |> ignore

    app.UseOxpecker(
        [ GET
              [ route "/" Handlers.terminalPage
                route "/api/terminal/github" Handlers.terminalGitHubApi ]
          POST [ route "/api/terminal/command" Handlers.terminalCommandApi ] ]
    )
    |> ignore

    app

[<EntryPoint>]
let main args =
    WebApplication.CreateBuilder(args)
    |> fun builder ->
        builder.Services.AddOxpecker() |> ignore
        builder.Build()
    |> configureApp
    |> fun app ->
        app.Run()
        0

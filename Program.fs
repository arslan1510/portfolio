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
    // Set Railway port configuration
    let port =
        System.Environment.GetEnvironmentVariable("PORT")
        |> Option.ofObj
        |> Option.defaultValue "8080"

    System.Environment.SetEnvironmentVariable("ASPNETCORE_URLS", $"http://0.0.0.0:{port}")

    let builder = WebApplication.CreateBuilder(args)
    builder.Services.AddOxpecker() |> ignore

    let app = builder.Build()

    app
    |> configureApp
    |> fun app ->
        app.Run()
        0

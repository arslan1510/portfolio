open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.FileProviders
open System.IO
open Oxpecker

let configureApp (app: WebApplication) =
    if app.Environment.IsDevelopment() then
        app.UseDeveloperExceptionPage() |> ignore

    // Configure static files to serve from frontend directory
    let frontendPath =
        let devPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "frontend")
        let containerPath = Path.Combine(Directory.GetCurrentDirectory(), "frontend")

        if Directory.Exists(containerPath) then
            containerPath
        else
            devPath

    app.UseStaticFiles(
        Microsoft.AspNetCore.Builder.StaticFileOptions(
            FileProvider = new PhysicalFileProvider(frontendPath),
            RequestPath = ""
        )
    )
    |> ignore

    app.UseRouting() |> ignore

    app.UseOxpecker(
        [ GET
              [ route "/" Handlers.terminalHandler
                route "/api/terminal/time" Handlers.terminalTimeHandler ]
          POST [ route "/api/terminal/command" Handlers.terminalCommandHandler ] ]
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

    // Configure web root to frontend directory
    let frontendPath =
        let devPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "frontend")
        let containerPath = Path.Combine(Directory.GetCurrentDirectory(), "frontend")

        if Directory.Exists(containerPath) then
            containerPath
        else
            devPath

    builder.Environment.WebRootPath <- frontendPath

    builder.Services.AddOxpecker() |> ignore

    let app = builder.Build()

    app
    |> configureApp
    |> fun app ->
        app.Run()
        0

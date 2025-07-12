module TerminalView

open Oxpecker.ViewEngine
open Oxpecker.Htmx
open Models

let asciiArt =
    """
     █████╗ ██╗     ███████╗██████╗ ██╗  ██╗███╗   ██╗██╗   ██╗██╗     ██╗          ██████╗ ███████╗██╗   ██╗
    ██╔══██╗██║     ██╔════╝██╔══██╗██║  ██║████╗  ██║██║   ██║██║     ██║          ██╔══██╗██╔════╝██║   ██║
    ███████║██║     █████╗  ██████╔╝███████║██╔██╗ ██║██║   ██║██║     ██║          ██║  ██║█████╗  ██║   ██║
    ██╔══██║██║     ██╔══╝  ██╔═══╝ ██╔══██║██║╚██╗██║██║   ██║██║     ██║          ██║  ██║██╔══╝  ╚██╗ ██╔╝
    ██║  ██║███████╗███████╗██║     ██║  ██║██║ ╚████║╚██████╔╝███████╗███████╗ ██╗ ██████╔╝███████╗ ╚████╔╝ 
    ╚═╝  ╚═╝╚══════╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝╚══════╝ ╚═╝ ╚═════╝ ╚══════╝  ╚═══╝  
"""

let terminalWelcome =
    div (id = "terminal-welcome", class' = "terminal-welcome") {
        pre (class' = "ascii-art") { asciiArt }
        div (class' = "terminal-line info left-aligned") { "Portfolio v0.2" }
        div (class' = "terminal-line dim left-aligned") { "Type 'help' for available commands" }
    }

let terminalOutput (lines: string list) =
    div (id = "terminal-output", class' = "terminal-output") {
        for line in lines do
            div (class' = "terminal-line") { line }
    }

let terminalPrompt =
    div (id = "terminal-prompt", class' = "terminal-prompt") {
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

let terminalContent =
    div (class' = "terminal-content") {
        terminalWelcome
        terminalOutput []
        terminalPrompt
    }

let terminalComponent =
    div (class' = "terminal-container") { 
        div (class' = "terminal-screen") { terminalContent } 
    }

let terminalPage =
    html (lang = "en") {
        head () {
            meta (charset = "UTF-8")
            meta (name = "viewport", content = "width=device-width, initial-scale=1.0")
            meta (name = "description", content = "Arslan - Portfolio")

            // Favicon
            link (rel = "icon", type' = "image/x-icon", href = "/favicon.ico")

            // Cascadia Code Font
            link (
                href = "https://cdn.jsdelivr.net/npm/@fontsource/cascadia-code@5.0.16/index.css",
                rel = "stylesheet"
            )

            title () { "Arslan - Portfolio" }

            // Terminal-specific CSS
            link (rel = "stylesheet", href = "/css/terminal.css")

            // HTMX
            script (src = "https://unpkg.com/htmx.org@1.9.9") { }


        }

        body () { terminalComponent }
    }

let projectCard (project: Project) =
    div (class' = "project-card") {
        div (class' = "project-title") { project.Title }
        div (class' = "project-description") { project.Description }
        div (class' = "project-tech") { project.Technologies |> String.concat ", " }

        div (class' = "project-links") {
            a (href = project.GitHubUrl, target = "_blank", class' = "project-link") { "View Code" }
        }
    }

let skillCategory (categoryKey: string) (category: SkillCategory) =
    let getCategoryClass (cat: string) =
        match cat.ToLower() with
        | "languages" -> "language"
        | "frameworks" -> "framework"
        | "databases" -> "database"
        | "cloud" -> "cloud"
        | "devops" -> "devops"
        | "tools" -> "tool"
        | "os" -> "os"
        | cat when cat.Contains("math") -> "math"
        | _ -> "default"
    
    let categoryClass = getCategoryClass categoryKey
    
    div (class' = "skill-category") {
        div (class' = "skill-name") { category.displayName.ToUpper() + ":" }

        div (class' = "skill-values") {
            for kvp in category.skills do
                let skillName = kvp.Key
                let skillInfo = kvp.Value
                let icon = if System.String.IsNullOrEmpty(skillInfo.icon) then "•" else skillInfo.icon
                span (class' = $"skill-tag {categoryClass}") { $"[{icon} {skillName}]" }
        }
    }

// Command response handlers
let renderCommandResponse (result: Result<string, string>) =
    match result with
    | Ok output when output.StartsWith("[{") ->
        // Handle GitHub projects JSON
        try
            let projects = System.Text.Json.JsonSerializer.Deserialize<Project[]>(output)

            div (class' = "command-output") {
                div (class' = "projects-grid") {
                    for project in projects do
                        projectCard project
                }
            }
        with _ ->
            div (class' = "command-output") {
                pre (class' = "output-content") { output }
            }
    | Ok output when output.Contains("categories") ->
        // Handle skills JSON with new structure
        try
            let skillsData = System.Text.Json.JsonSerializer.Deserialize<SkillsData>(output)
            
            match skillsData with
            | null -> 
                div (class' = "command-output") {
                    pre (class' = "output-content") { output }
                }
            | data ->
                div (class' = "command-output") {
                    div (class' = "skills-grid") {
                        for kvp in data.categories do
                            let categoryKey = kvp.Key
                            let category = kvp.Value
                            skillCategory categoryKey category
                    }
                }
        with _ ->
            div (class' = "command-output") {
                pre (class' = "output-content") { output }
            }
    | Ok output when output = "CLEAR_TERMINAL" ->
        // Clear terminal by replacing entire output div
        div (id = "terminal-output", class' = "terminal-output", hxSwapOob = "innerHTML") { "" }
    | Ok output when output.Contains("Email:") && output.Contains("GitHub:") ->
        // Handle contact command output with special formatting
        let lines = output.Split('\n') |> Array.filter (fun s -> not (System.String.IsNullOrWhiteSpace(s)))
        div (class' = "command-output") {
            div (class' = "contact-section") {
                for line in lines do
                    let trimmedLine = line.Trim()
                    if trimmedLine.StartsWith("Email:") then
                        let email = trimmedLine.Replace("Email:", "").Trim()
                        div (class' = "contact-email") { 
                            span () { "Email: " }
                            a (href = $"mailto:{email}", class' = "terminal-link") { email }
                        }
                    elif trimmedLine.StartsWith("GitHub:") then
                        let github = trimmedLine.Replace("GitHub:", "").Trim()
                        let githubUrl = if github.StartsWith("http") then github else $"https://{github}"
                        div (class' = "contact-github") { 
                            span () { "GitHub: " }
                            a (href = githubUrl, target = "_blank", class' = "terminal-link") { github }
                        }
                    elif trimmedLine.StartsWith("LinkedIn:") then
                        let linkedin = trimmedLine.Replace("LinkedIn:", "").Trim()
                        let linkedinUrl = if linkedin.StartsWith("http") then linkedin else $"https://{linkedin}"
                        div (class' = "contact-linkedin") { 
                            span () { "LinkedIn: " }
                            a (href = linkedinUrl, target = "_blank", class' = "terminal-link") { linkedin }
                        }
            }
        }
    | Ok output ->
        // General handler for all other successful outputs
        div (class' = "command-output") {
            pre (class' = "output-content") { output }
        }
    | Error error -> div (class' = "terminal-line error") { error }

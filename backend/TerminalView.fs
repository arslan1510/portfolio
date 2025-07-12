module TerminalView

open Oxpecker.ViewEngine
open Oxpecker.Htmx
open Models

let asciiArt =
    """
 █████╗ ██╗     ███████╗██████╗ ██╗  ██╗███╗   ██╗██╗   ██╗██╗     ██╗        ██████╗ ███████╗██╗   ██╗
██╔══██╗██║     ██╔════╝██╔══██╗██║  ██║████╗  ██║██║   ██║██║     ██║        ██╔══██╗██╔════╝██║   ██║
███████║██║     █████╗  ██████╔╝███████║██╔██╗ ██║██║   ██║██║     ██║        ██║  ██║█████╗  ██║   ██║
██╔══██║██║     ██╔══╝  ██╔═══╝ ██╔══██║██║╚██╗██║██║   ██║██║     ██║        ██║  ██║██╔══╝  ╚██╗ ██╔╝
██║  ██║███████╗███████╗██║     ██║  ██║██║ ╚████║╚██████╔╝███████╗███████╗██╗██████╔╝███████╗ ╚████╔╝ 
╚═╝  ╚═╝╚══════╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝╚══════╝╚═╝╚═════╝ ╚══════╝  ╚═══╝  
"""

let terminalWelcome =
    div (id = "terminal-welcome", class' = "terminal-welcome") {
        pre (class' = "ascii-art") { asciiArt }
        div (class' = "terminal-line info left-aligned") { "Portfolio v0.3" }
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

        span (
            id = "terminal-prompt-time",
            class' = "terminal-prompt-time",
            hxGet = "/api/terminal/time",
            hxTrigger = "load, every 1s",
            hxSwap = "innerHTML"
        ) {
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
    div (id = "terminal-container", class' = "terminal-container") {
        div (id = "terminal-window", class' = "terminal-window") {
            div (class' = "terminal-title-bar") {
                div (class' = "terminal-buttons") {
                    div (
                        class' = "terminal-button minimize",
                        hxPost = "/api/terminal/minimize",
                        hxTarget = "#terminal-window",
                        hxSwap = "outerHTML"
                    ) {
                        ""
                    }

                    div (
                        class' = "terminal-button maximize",
                        hxPost = "/api/terminal/maximize",
                        hxTarget = "#terminal-window",
                        hxSwap = "outerHTML"
                    ) {
                        ""
                    }

                    div (
                        class' = "terminal-button close",
                        hxPost = "/api/terminal/close",
                        hxTarget = "#terminal-container",
                        hxSwap = "innerHTML"
                    ) {
                        ""
                    }
                }

                div (class' = "terminal-title") { "guest@portfolio:~" }
            }

            div (class' = "terminal-screen") { terminalContent }
        }
    }


let minimizedTerminal =
    div (id = "terminal-window", class' = "terminal-window", style = "height: 40px; width: 300px;") {
        div (class' = "terminal-title-bar") {
            div (class' = "terminal-buttons") {
                div (
                    class' = "terminal-button minimize",
                    hxPost = "/api/terminal/restore",
                    hxTarget = "#terminal-window",
                    hxSwap = "outerHTML"
                ) {
                    ""
                }

                div (
                    class' = "terminal-button maximize",
                    hxPost = "/api/terminal/maximize",
                    hxTarget = "#terminal-window",
                    hxSwap = "outerHTML"
                ) {
                    ""
                }

                div (
                    class' = "terminal-button close",
                    hxPost = "/api/terminal/close",
                    hxTarget = "#terminal-container",
                    hxSwap = "innerHTML"
                ) {
                    ""
                }
            }

            div (class' = "terminal-title") { "guest@portfolio:~" }
        }
    }

let maximizedTerminal =
    div (id = "terminal-window", class' = "terminal-window", style = "width: 100%; height: 100%; border-radius: 0;") {
        div (class' = "terminal-title-bar") {
            div (class' = "terminal-buttons") {
                div (
                    class' = "terminal-button minimize",
                    hxPost = "/api/terminal/minimize",
                    hxTarget = "#terminal-window",
                    hxSwap = "outerHTML"
                ) {
                    ""
                }

                div (
                    class' = "terminal-button maximize",
                    hxPost = "/api/terminal/restore",
                    hxTarget = "#terminal-window",
                    hxSwap = "outerHTML"
                ) {
                    ""
                }

                div (
                    class' = "terminal-button close",
                    hxPost = "/api/terminal/close",
                    hxTarget = "#terminal-container",
                    hxSwap = "innerHTML"
                ) {
                    ""
                }
            }

            div (class' = "terminal-title") { "guest@portfolio:~" }
        }

        div (class' = "terminal-screen") { terminalContent }
    }

let normalTerminal =
    div (id = "terminal-window", class' = "terminal-window") {
        div (class' = "terminal-title-bar") {
            div (class' = "terminal-buttons") {
                div (
                    class' = "terminal-button minimize",
                    hxPost = "/api/terminal/minimize",
                    hxTarget = "#terminal-window",
                    hxSwap = "outerHTML"
                ) {
                    ""
                }

                div (
                    class' = "terminal-button maximize",
                    hxPost = "/api/terminal/maximize",
                    hxTarget = "#terminal-window",
                    hxSwap = "outerHTML"
                ) {
                    ""
                }

                div (
                    class' = "terminal-button close",
                    hxPost = "/api/terminal/close",
                    hxTarget = "#terminal-container",
                    hxSwap = "innerHTML"
                ) {
                    ""
                }
            }

            div (class' = "terminal-title") { "guest@portfolio:~" }
        }

        div (class' = "terminal-screen") { terminalContent }
    }

let closedTerminal = div () { "" }

let terminalPage =
    html (lang = "en") {
        head () {
            meta (charset = "UTF-8")
            meta (name = "viewport", content = "width=device-width, initial-scale=1.0")
            meta (name = "description", content = "Arslan - Portfolio")
            link (rel = "icon", type' = "image/x-icon", href = "/favicon.ico")
            link (href = "https://cdn.jsdelivr.net/npm/@fontsource/cascadia-code@5.0.16/index.css", rel = "stylesheet")
            title () { "Arslan - Portfolio" }
            link (rel = "stylesheet", href = "/css/terminal.css")
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
    div (class' = "skill-category") {
        div (class' = "skill-badges") {
            for skill in category.skills do
                let skillName = skill.name
                let skillInfo = skill.info
                let skillId = skillName.Replace(" ", "-").ToLower()

                img (
                    src = skillInfo.badgeUrl,
                    alt = skillName,
                    class' = "skill-badge",
                    id = $"skill-{skillId}",
                    style = "margin: 2px;"
                )
        }
    }

let renderCommandResponse (result: Result<string, string>) =
    match result with
    | Ok output when output.StartsWith("[{") ->
        try
            let projects = System.Text.Json.JsonSerializer.Deserialize<Project[]>(output)

            div (class' = "command-output") {
                div (class' = "projects-grid") {
                    for project in projects do
                        projectCard project
                }
            }
        with _ ->
            div (class' = "command-output") { pre (class' = "output-content") { output } }
    | Ok output when output.Contains("categories") ->
        try
            let skillsData = System.Text.Json.JsonSerializer.Deserialize<SkillsData>(output)

            match skillsData with
            | null -> div (class' = "command-output") { pre (class' = "output-content") { output } }
            | data ->
                div (class' = "command-output") {
                    div (class' = "skills-grid") {
                        for categoryItem in data.categories do
                            let categoryKey = categoryItem.key
                            let category = categoryItem.category
                            skillCategory categoryKey category
                    }
                }
        with _ ->
            div (class' = "command-output") { pre (class' = "output-content") { output } }
    | Ok output when output = "CLEAR_TERMINAL" ->
        div (id = "terminal-output", class' = "terminal-output", hxSwapOob = "innerHTML") { "" }
    | Ok output when output.StartsWith("{") && output.Contains("\"header\"") ->
        try
            let aboutData = System.Text.Json.JsonSerializer.Deserialize<AboutData>(output)

            match aboutData with
            | null -> div (class' = "command-output") { pre (class' = "output-content") { output } }
            | data ->
                div (class' = "command-output") {
                    div (class' = "terminal-line") { data.header }

                    div (class' = "terminal-line", style = "margin-top: 0.5rem;") {
                        span (style = "color: var(--gruvbox-blue-bright); font-weight: bold;") {
                            "I have expertise in:"
                        }
                    }

                    for expertise in data.expertise do
                        div (class' = "terminal-line") {
                            span (style = "color: var(--gruvbox-yellow-bright);") { "  • " }
                            span () { expertise }
                        }

                    div (class' = "terminal-line", style = "margin-top: 0.5rem;") {
                        span (style = "color: var(--gruvbox-blue-bright); font-weight: bold;") {
                            "With particular interests in:"
                        }
                    }

                    for interest in data.interests do
                        div (class' = "terminal-line") {
                            span (style = "color: var(--gruvbox-yellow-bright);") { "  • " }
                            span () { interest }
                        }

                    div (class' = "terminal-line", style = "margin-top: 0.5rem;") {
                        span (style = "color: var(--gruvbox-green-bright);") { data.current }
                    }

                    div (class' = "terminal-line", style = "margin-top: 0.5rem;") { data.footer }
                }
        with _ ->
            div (class' = "command-output") { pre (class' = "output-content") { output } }
    | Ok output when output.Contains("  help") || output.Contains("  about") ->
        let lines =
            output.Split('\n')
            |> Array.filter (fun s -> not (System.String.IsNullOrWhiteSpace(s)))

        div (class' = "command-output") {
            for line in lines do
                let trimmedLine = line.Trim()

                if trimmedLine.StartsWith("  ") && trimmedLine.Length > 2 then
                    let commandPart = trimmedLine.Substring(2)

                    let parts =
                        commandPart.Split([| ' ' |], System.StringSplitOptions.RemoveEmptyEntries)

                    if parts.Length >= 2 then
                        let command = parts.[0]
                        let description = System.String.Join(" ", parts |> Array.skip 1)

                        div (class' = "terminal-line") {
                            span (style = "color: var(--gruvbox-green-bright); font-weight: bold;") { $"  {command}" }
                            span (style = "color: var(--gruvbox-fg3); margin-left: 1rem;") { description }
                        }
                    else
                        div (class' = "terminal-line") { trimmedLine }
                else
                    div (class' = "terminal-line") { trimmedLine }
        }
    | Ok output when output.Contains("Email:") && output.Contains("GitHub:") ->
        let lines =
            output.Split('\n')
            |> Array.filter (fun s -> not (System.String.IsNullOrWhiteSpace(s)))

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

                        let githubUrl =
                            if github.StartsWith("http") then
                                github
                            else
                                $"https://{github}"

                        div (class' = "contact-github") {
                            span () { "GitHub: " }
                            a (href = githubUrl, target = "_blank", class' = "terminal-link") { github }
                        }
                    elif trimmedLine.StartsWith("LinkedIn:") then
                        let linkedin = trimmedLine.Replace("LinkedIn:", "").Trim()

                        let linkedinUrl =
                            if linkedin.StartsWith("http") then
                                linkedin
                            else
                                $"https://{linkedin}"

                        div (class' = "contact-linkedin") {
                            span () { "LinkedIn: " }
                            a (href = linkedinUrl, target = "_blank", class' = "terminal-link") { linkedin }
                        }
            }
        }
    | Ok output -> div (class' = "command-output") { pre (class' = "output-content") { output } }
    | Error error -> div (class' = "terminal-line error") { error }

module TerminalView

open Oxpecker.ViewEngine
open Oxpecker.Htmx
open Models

let asciiArt =
    """
 █████╗ ██╗     ███████╗██████╗ ██╗  ██╗███╗   ██╗██╗   ██╗██╗     ██╗        ██████╗ ███████╗██╗   ██║
██╔══██╗██║     ██╔════╝██╔══██╗██║  ██║████╗  ██║██║   ██║██║     ██║        ██╔══██╗██╔════╝██║   ██║
███████║██║     █████╗  ██████╔╝███████║██╔██╗ ██║██║   ██║██║     ██║        ██║  ██║█████╗  ██║   ██║
██╔══██║██║     ██╔══╝  ██╔═══╝ ██╔══██║██║╚██╗██║██║   ██║██║     ██║        ██║  ██║██╔══╝  ╚██╗ ██╔╝
██║  ██║███████╗███████╗██║     ██║  ██║██║ ╚████║╚██████╔╝███████╗███████╗██╗██████╔╝███████╗ ╚████╔╝ 
╚═╝  ╚═╝╚══════╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝╚══════╝╚═╝╚═════╝ ╚══════╝  ╚═══╝  
"""

let fastFetchAsciiArt =
    """                                                                                                                      
                            ▓  ▓▓                             
                           ▓▓  ▓▓▓                            
                         ▓▓▓▓  ▓▓▓▓▓                          
                       ▓▓▓▓▓▓  ▓▓▓▓▓▓▓                        
                     ▓▓▓▓▓▓▓▓  ▓▓▓▓▓▓▓▓▓                      
                   ▓▓▓▓▓▓▓▓▓▓  ▓▓▓▓▓▓▓▓▓▓▓                    
                 ▓▓▓▓▓▓▓▓▓▓▓▓  ▓▓▓▓▓▓▓▓▓▓▓▓▓                  
                ▓▓▓▓▓▓▓▓▓▓▓▓▓  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                
              ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓   ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓              
            ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓       ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓            
          ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓      ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓           
        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓         
       ▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓         ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓       
     ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓           ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓     
   ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓             ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓   
 ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓               ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ 
 ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓   ▓▓▓▓▓▓▓▓▓▓▓                ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ 
  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓▓              ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  
    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓            ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    
      ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓          ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓      
        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓        
          ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓      ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓          
           ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓            
             ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓     ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓              
               ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                
                 ▓▓▓▓▓▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓▓▓▓▓                  
                   ▓▓▓▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓▓▓▓                   
                     ▓▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓▓                     
                      ▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓                       
                        ▓▓▓▓▓▓ ▓▓▓▓▓▓                         
                          ▓▓▓▓ ▓▓▓▓                           
                            ▓▓ ▓▓                             
                                                                       
"""


module Styles =
    let gruvboxColor color = $"color: var(--gruvbox-{color});"

    let gruvboxBright color =
        $"color: var(--gruvbox-{color}-bright);"

    let marginTop value = $"margin-top: {value};"
    let fontWeight weight = $"font-weight: {weight};"
    let marginLeft value = $"margin-left: {value};"

type TerminalState =
    | Normal
    | Minimized
    | Maximized

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

let terminalButtons (state: TerminalState) =
    let (minimizeAction, maximizeAction) =
        match state with
        | Normal -> ("/api/terminal/minimize", "/api/terminal/maximize")
        | Minimized -> ("/api/terminal/restore", "/api/terminal/maximize")
        | Maximized -> ("/api/terminal/minimize", "/api/terminal/restore")

    div (class' = "terminal-buttons") {
        div (
            class' = "terminal-button minimize",
            hxPost = minimizeAction,
            hxTarget = "#terminal-window",
            hxSwap = "outerHTML"
        ) {
            ""
        }

        div (
            class' = "terminal-button maximize",
            hxPost = maximizeAction,
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

let createTerminalWindow (state: TerminalState) (includeContent: bool) =
    let windowStyle =
        match state with
        | Minimized -> "height: 40px; width: 300px;"
        | Maximized -> "width: 100%; height: 100%; border-radius: 0;"
        | Normal -> ""

    div (id = "terminal-window", class' = "terminal-window", style = windowStyle) {
        div (class' = "terminal-title-bar") {
            terminalButtons state
            div (class' = "terminal-title") { "guest@portfolio:~" }
        }

        if includeContent then
            div (class' = "terminal-screen") { terminalContent }
    }

let minimizedTerminal = createTerminalWindow Minimized false
let maximizedTerminal = createTerminalWindow Maximized true
let normalTerminal = createTerminalWindow Normal true

let terminalComponent =
    div (id = "terminal-container", class' = "terminal-container") { normalTerminal }

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


let renderProjects (output: string) =
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

// Skills rendering
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

let renderSkills (output: string) =
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

let renderAboutData (data: AboutData) =
    div (class' = "command-output") {
        div (class' = "terminal-line") { data.header }

        div (class' = "terminal-line", style = Styles.marginTop "0.5rem") {
            span (style = Styles.gruvboxBright "blue" + Styles.fontWeight "bold") { "I have expertise in:" }
        }

        for expertise in data.expertise do
            div (class' = "terminal-line") {
                span (style = Styles.gruvboxBright "yellow") { "  • " }
                span () { expertise }
            }

        div (class' = "terminal-line", style = Styles.marginTop "0.5rem") {
            span (style = Styles.gruvboxBright "blue" + Styles.fontWeight "bold") { "With particular interests in:" }
        }

        for interest in data.interests do
            div (class' = "terminal-line") {
                span (style = Styles.gruvboxBright "yellow") { "  • " }
                span () { interest }
            }

        div (class' = "terminal-line", style = Styles.marginTop "0.5rem") {
            span (style = Styles.gruvboxBright "green") { data.current }
        }

        div (class' = "terminal-line", style = Styles.marginTop "0.5rem") { data.footer }
    }

let renderAbout (output: string) =
    try
        let aboutData = System.Text.Json.JsonSerializer.Deserialize<AboutData>(output)

        match aboutData with
        | null -> div (class' = "command-output") { pre (class' = "output-content") { output } }
        | data -> renderAboutData data
    with _ ->
        div (class' = "command-output") { pre (class' = "output-content") { output } }

let renderHelp (output: string) =
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
                        span (style = Styles.gruvboxBright "green" + Styles.fontWeight "bold") { $"  {command}" }
                        span (style = Styles.gruvboxColor "fg3" + Styles.marginLeft "1rem") { description }
                    }
                else
                    div (class' = "terminal-line") { trimmedLine }
            else
                div (class' = "terminal-line") { trimmedLine }
    }

let formatUrl (url: string) =
    if url.StartsWith("http") then url else $"https://{url}"

let createContactLink (label: string) (value: string) (linkType: string) =
    let href =
        match linkType with
        | "email" -> $"mailto:{value}"
        | _ -> formatUrl value

    let target = if linkType = "email" then "" else "_blank"

    div (class' = $"contact-{linkType}") {
        span () { $"{label}: " }
        a (href = href, target = target, class' = "terminal-link") { value }
    }

let renderContact (output: string) =
    let lines =
        output.Split('\n')
        |> Array.filter (fun s -> not (System.String.IsNullOrWhiteSpace(s)))

    div (class' = "command-output") {
        div (class' = "contact-section") {
            for line in lines do
                let trimmedLine = line.Trim()

                if trimmedLine.StartsWith("Email:") then
                    let email = trimmedLine.Replace("Email:", "").Trim()
                    createContactLink "Email" email "email"
                elif trimmedLine.StartsWith("GitHub:") then
                    let github = trimmedLine.Replace("GitHub:", "").Trim()
                    createContactLink "GitHub" github "github"
                elif trimmedLine.StartsWith("LinkedIn:") then
                    let linkedin = trimmedLine.Replace("LinkedIn:", "").Trim()
                    createContactLink "LinkedIn" linkedin "linkedin"
        }
    }

let renderFastFetch (output: string) =
    try
        let fastFetchData =
            System.Text.Json.JsonSerializer.Deserialize<FastFetchData>(output)

        match fastFetchData with
        | null -> div (class' = "command-output") { pre (class' = "output-content") { output } }
        | data ->
            div (class' = "command-output") {
                div (class' = "fastfetch-container") {
                    div (class' = "fastfetch-ascii") { pre (class' = "output-content") { fastFetchAsciiArt } }

                    div (class' = "fastfetch-info") {
                        div (class' = "terminal-line", style = Styles.gruvboxBright "blue" + Styles.fontWeight "bold") {
                            data.header
                        }

                        for item in data.items do
                            div (class' = "terminal-line") {
                                span (style = Styles.gruvboxBright "yellow") { $"{item.icon} {item.title}:" }
                                span (style = Styles.marginLeft "1rem") { item.description }
                            }
                    }
                }
            }
    with _ ->
        div (class' = "command-output") {
            pre (class' = "output-content") { output }
            pre (class' = "output-content") { fastFetchAsciiArt }
        }

let renderCommandResponse (result: Result<string, string>) =
    match result with
    | Ok output when output.StartsWith("[{") -> renderProjects output
    | Ok output when output.Contains("categories") -> renderSkills output
    | Ok output when output = "CLEAR_TERMINAL" ->
        div (id = "terminal-output", class' = "terminal-output", hxSwapOob = "innerHTML") { "" }
    | Ok output when
        output.StartsWith("{")
        && output.Contains("\"header\"")
        && output.Contains("\"items\"")
        ->
        renderFastFetch output
    | Ok output when output.StartsWith("{") && output.Contains("\"header\"") -> renderAbout output
    | Ok output when output.Contains("  help") || output.Contains("  about") -> renderHelp output
    | Ok output when output.Contains("Email:") && output.Contains("GitHub:") -> renderContact output
    | Ok output -> div (class' = "command-output") { pre (class' = "output-content") { output } }
    | Error error -> div (class' = "terminal-line error") { error }

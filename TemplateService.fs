module TemplateService

open System.IO

let tryReadTemplate (fileName: string) : Result<string, string> =
    let templatePath = Path.Combine("templates", fileName)

    if File.Exists(templatePath) then
        try
            Ok(File.ReadAllText(templatePath))
        with ex ->
            Error $"Failed to read template {fileName}: {ex.Message}"
    else
        Error $"Template file not found: {templatePath}"

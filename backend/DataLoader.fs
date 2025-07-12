module DataLoader

open System.IO
open System.Text.Json
open System.Collections.Generic
open Models


let private readJsonFile<'T> (filePath: string) : 'T =
    let json = File.ReadAllText(filePath)

    if System.String.IsNullOrEmpty(json) then
        failwith $"JSON file is empty or null: {filePath}"
    else
        let options = JsonSerializerOptions()
        options.PropertyNameCaseInsensitive <- true

        match JsonSerializer.Deserialize<'T>(json, options) with
        | null -> failwith $"Failed to deserialize JSON from file: {filePath}"
        | result -> result

let private memoize (fn: 'a -> 'b) =
    let cache = Dictionary<'a, 'b>()

    fun x ->
        match cache.TryGetValue(x) with
        | true, value -> value
        | false, _ ->
            let value = fn x
            cache.[x] <- value
            value

let private loadTextDataMemoized =
    memoize (fun filePath -> readJsonFile<TextData> filePath)

let private loadHelpDataMemoized =
    memoize (fun filePath -> readJsonFile<HelpData> filePath)

let private loadContactDataMemoized =
    memoize (fun filePath -> readJsonFile<ContactData> filePath)

let private loadSkillsDataMemoized =
    memoize (fun filePath -> readJsonFile<SkillsData> filePath)

let loadHelpText () =
    let helpData = loadHelpDataMemoized "data/help.json"
    let commands = helpData.commands

    let commandList =
        commands
        |> Map.toList
        |> List.map (fun (cmd, desc) -> sprintf "  %-17s %s" cmd desc)
        |> String.concat "\n"

    sprintf "\nAvailable commands:\n\n%s\n" commandList

let loadAboutText () =
    let data = loadTextDataMemoized "data/about.json"
    sprintf "\n%s\n" data.text

let loadContactInfo () =
    let contactData = loadContactDataMemoized "data/contact.json"

    sprintf
        "Email:    %s\n  GitHub:   %s\n  LinkedIn: %s\n"
        contactData.email
        contactData.github
        contactData.linkedin

let loadSkills () =
    let skillsData = loadSkillsDataMemoized "data/skills.json"
    skillsData

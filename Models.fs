module Models

type Project =
    { Title: string
      Description: string
      Technologies: string[]
      GitHubUrl: string }

type Skill = { Name: string; Category: string }

type GitHubRepo =
    { name: string
      description: string option
      html_url: string
      homepage: string option
      language: string option
      topics: string[]
      updated_at: string
      fork: bool }

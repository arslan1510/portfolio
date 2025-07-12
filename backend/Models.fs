module Models

type Project =
    { Title: string
      Description: string
      Technologies: string[]
      GitHubUrl: string }


type GitHubRepo =
    { name: string
      description: string option
      html_url: string
      homepage: string option
      language: string option
      topics: string[]
      updated_at: string
      created_at: string
      size: int
      fork: bool
      archived: bool
      stargazers_count: int
      forks_count: int
      open_issues_count: int
      has_wiki: bool
      has_pages: bool }

type ContactData =
    { email: string
      github: string
      linkedin: string }

type SkillInfo = 
    { icon: string
      proficiency: string option }

type SkillCategory = 
    { displayName: string
      skills: Map<string, SkillInfo> }

type SkillsData = 
    { categories: Map<string, SkillCategory> }

type TextData = { text: string }

type HelpData = { commands: Map<string, string> }


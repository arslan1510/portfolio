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

type AboutData =
    { header: string
      expertise: string[]
      interests: string[]
      current: string
      footer: string }

type FastFetchItem =
    { title: string
      description: string
      icon: string }

type FastFetchData =
    { header: string
      items: FastFetchItem[] }

type SkillInfo = { badgeUrl: string }

type SkillItem = { name: string; info: SkillInfo }

type SkillCategory =
    { displayName: string
      skills: SkillItem[] }

type CategoryItem =
    { key: string; category: SkillCategory }

type SkillsData = { categories: CategoryItem[] }

type TextData = { text: string }

type HelpData = { commands: Map<string, string> }

type Blog =
    { title: string
      content: string
      date: string }

# GitHub Repo Documentation tool

This tool is designed to assist with reporting on a GitHub Repo. By generating a private API Token, choosing the repository and owner, the application will simply deliver a markdown file with the status of the repo.

Features:

1. Gives overall details on the repo itself
2. Shows all issues modified in the last 30 days with links
3. Shows all open issues with links
4. Shows every project, column and issues and cards in each column

Download today and give it a go!

## Usage

```
Documentor:
  Produces a markdown status report of a github repo

Usage:
  Documentor [options]

Options:
  --repository <repository>    The Repository
  --owner <owner>              The Owner's Github Alias
  --token <token>              The GitHub Token
  --version                    Show version information
  -?, -h, --help               Show help and usage information
```

This version written in dotnet core, and portable to Linux, Mac and PC


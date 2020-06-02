![.NET Core](https://github.com/nickbeau/project-documentor/workflows/.NET%20Core/badge.svg)

# project-documentor

Github project documentor

Welcome to the GitHub project Documentor.

This simple dotnet core application documents a github repo, preparing a status update for stakeholders.

It requires three variables:

- **owner** is the repo owner or contributors github alias (mine is nickbeau)
- **repo** is the repo you wish to document (e.g. this one is project-documentor)
- **token** is the GitHub Personal access token avaulable here: [github.com/settings/tokens](https://github.com/settings/tokens)

To start, download a release and run documentor with the below syntax

```
Documentor:
  Produces a markdown status report of a github repo. The report is delivered into this directory with the same filename as the repository chosen below. This markdown file can be printed to pdf or edited in a markdown editor.

Usage:
  Documentor [options]

Options:
  --repository <repository>    The GitHub Repository.
  --owner <owner>              The Owner's Github Alias.
  --token <token>              The GitHub Token.
  --version                    Show version information
  -?, -h, --help               Show help and usage information
  ```

Simple now, with more to come.

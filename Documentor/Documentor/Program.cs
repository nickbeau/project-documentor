using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Documentor
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            string owner = "nickbeau";
            string repository = "cbrook";
            string token = "1cb0e948dacfabc1813d95cbfb363745fc265721";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# Released Group Project Status Report");
            sb.AppendLine("");
            sb.AppendLine($"## Status Report for {repository} created on {DateTime.Now}");
            sb.AppendLine("");
            sb.AppendLine("## Overall Information");
            sb.AppendLine("");
            
            var client = new GitHubClient(new ProductHeaderValue("project-documentor"));
            var tokenAuth = new Credentials(token);
            client.Credentials = tokenAuth;
            var user = await client.User.Current();

            var repo = await client.Repository.Get(owner, repository);

            sb.AppendLine("| Item | Value |");
            sb.AppendLine("| -- | -- |");
            sb.AppendLine($"| Date Created | {repo.CreatedAt} |");
            sb.AppendLine($"| Default Branch | {repo.DefaultBranch} |");
            sb.AppendLine($"| Description | {repo.Description} |");
            sb.AppendLine($"| Full Name | {repo.FullName} |");
            sb.AppendLine($"| URL | {repo.GitUrl} |");
            sb.AppendLine($"| Name | {repo.Name} |");
            sb.AppendLine($"| Current Open Issues | {repo.OpenIssuesCount} |");
            sb.AppendLine($"| Last Code Update | {repo.PushedAt} |");
            sb.AppendLine($"| Subscribers | {repo.SubscribersCount} |");
            sb.AppendLine($"| Last Update | {repo.UpdatedAt} |");

            sb.AppendLine("");
            sb.AppendLine("## Issues");

            sb.AppendLine("");
            sb.AppendLine("### Issues modified in the last 30 days");
            var issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                Since = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(30)),
                SortProperty = IssueSort.Updated,
                SortDirection = SortDirection.Descending
            };
            var issues = await client.Issue.GetAllForRepository(owner, repository, issuefilter);
            sb.AppendLine("");
            sb.AppendLine("| Issue ID | Modified | Status | Title |");
            sb.AppendLine("| -------- | -------- | ------ | ----- |");
            foreach (var issue in issues)
            {
                sb.AppendLine($"| [{issue.Number}]({issue.Url}) | {issue.UpdatedAt.Value.ToLocalTime().ToString("dd-MM-yyyy HH:mm")} | {issue.State} | {issue.Title} |");
            }

            sb.AppendLine("");
            sb.AppendLine("### Open Issues");
            sb.AppendLine("");
            sb.AppendLine("| Issue ID | Modified | Status | Title |");
            sb.AppendLine("| -------- | -------- | ------ | ----- |");
            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.Open,
                Since = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(30)),
                SortProperty=IssueSort.Updated,
                SortDirection=SortDirection.Descending
            };

            issues = await client.Issue.GetAllForRepository(owner, repository, issuefilter);
            foreach (var issue in issues)
            {
                sb.AppendLine($"| [{issue.Number}]({issue.Url}) | {issue.UpdatedAt.Value.ToLocalTime().ToString("dd-MM-yyyy HH:mm")} | {issue.State} | {issue.Title} |");
            }

            sb.AppendLine("");
            sb.AppendLine("## Projects");
            //Projects
            var projects = await client.Repository.Project.GetAllForRepository(owner, repository);

            foreach(var project in projects)
            {
                sb.AppendLine("");
                sb.AppendLine($"### {project.Name}");
                sb.AppendLine("");
               
                sb.AppendLine("| Item | Value |");
                sb.AppendLine("| -----| ----- |");
                sb.AppendLine($"| Created | {project.CreatedAt.ToLocalTime().ToString("dd-MM-yyyy HH:mm")} |");
                sb.AppendLine($"| Project Number | {project.Number}");
                sb.AppendLine($"| State | {project.State}");
                sb.AppendLine($"| State | {project.UpdatedAt.ToLocalTime().ToString("dd-MM-yyyy HH:mm")} |");
                sb.AppendLine("");

                sb.AppendLine("#### Project Status");
                sb.AppendLine("");
                var columns = await client.Repository.Project.Column.GetAll(project.Id);
                string line1="";
                string line2="";
                List<string> lines = new List<string>();
                int lcount = 0;
                int ccount = 0;
                foreach(var column in columns)
                {
                    sb.AppendLine($"##### {column.Name}");
                    sb.AppendLine("");

                    var cards = await client.Repository.Project.Card.GetAll(column.Id);
                    lcount = 0;
                   foreach (var card in cards)
                    {
                        Console.WriteLine($"Working on card: {card.Id}"); 
                        if (string.IsNullOrEmpty(card.Note))
                            { 
                            string issueId = card.ContentUrl.Split("/")[card.ContentUrl.Split("/").Length-1];
                            var cardissue = await client.Issue.Get(owner, repository, Convert.ToInt32(issueId));
                            sb.AppendLine($"*{cardissue.Title}*");
                            sb.AppendLine($"{cardissue.Body}");
                            sb.AppendLine("");
                        }
                        else
                        {
                            sb.AppendLine(card.Note);
                            sb.AppendLine("");
                        }

                    }
                }
              
                


            }

           



            string output = sb.ToString();
            System.IO.File.WriteAllText($"c:\\dev\\{repository}.md", output);
            Console.WriteLine($"{user.Name} has {user.PublicRepos} public repositories");
           
            
            foreach(var issue in issues)
            {
                Console.WriteLine($"{issue.Number} - {issue.Title}");
            }
            Console.WriteLine("Hello World!");
        }
    }
}

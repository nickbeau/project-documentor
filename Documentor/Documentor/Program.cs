using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using System.CommandLine;
using System.CommandLine.Invocation;
using Documentor.Properties;

namespace Documentor
{
    class Program
    {
        /// <summary>
        /// Produces a markdown status report of a github repo
        /// </summary>
        /// <param name="repository">The Repository</param>
        /// <param name="owner">The Owner's Github Alias</param>
        /// <param name="token">The GitHub Token</param>
        /// <returns></returns>
        public static async Task Main(string repository, string owner, string token)
        {
           

            Console.WriteLine($"{Resources.AppTitle}");
            Console.WriteLine($"{Resources.Copyright}");
            Console.WriteLine($"{Resources.PlsWait}");
            Console.WriteLine("Step 1 of # - Headers");
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
            Console.WriteLine("Step 2 of # - Issues modified in last 30 days");
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

            Console.WriteLine("Step 3 of #, Projects");
            sb.AppendLine("");
            sb.AppendLine("## Projects");
            //Projects
            var projects = await client.Repository.Project.GetAllForRepository(owner, repository);
            int projectid = 1;
            foreach(var project in projects)
            {
                Console.WriteLine($" Working on Project {projectid} of {projects.Count}: {project.Name}");
                projectid++;
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
                int colcount = 0;
                foreach(var column in columns)
                {
                    colcount++;
                    sb.AppendLine($"##### {column.Name}");
                    sb.AppendLine("");
                    Console.WriteLine($"  Column {colcount} of {columns.Count} - {column.Name}");
                    var cards = await client.Repository.Project.Card.GetAll(column.Id);
                    lcount = 0;
                    int cardcount = 0;

                    //Handle no cards, produce nice message
                    if (cards.Count == 0) 
                    { 
                        sb.AppendLine("     *There are no cards in this status*");
                        sb.AppendLine();
                    }
                   foreach (var card in cards)
                    {
                        cardcount++;
                        Console.WriteLine($"Working on card: {cardcount} of {cards.Count}");
                        if (column.Name != "Done" && column.Name !="Closed")
                        {
                            if (string.IsNullOrEmpty(card.Note))
                            {
                                string issueId = card.ContentUrl.Split("/")[card.ContentUrl.Split("/").Length - 1];
                                var cardissue = await client.Issue.Get(owner, repository, Convert.ToInt32(issueId));
                                sb.AppendLine($"**[{issueId}]({cardissue.Url})** - *{cardissue.Title}*");
                                sb.AppendLine("");
                                sb.AppendLine($"{cardissue.Body}");
                                sb.AppendLine("");
                            }
                            else
                            {
                                sb.AppendLine(card.Note);
                                sb.AppendLine("");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(card.Note))
                            {
                                string issueId = card.ContentUrl.Split("/")[card.ContentUrl.Split("/").Length - 1];
                                var cardissue = await client.Issue.Get(owner, repository, Convert.ToInt32(issueId));
                                sb.AppendLine($"- [{issueId}]({cardissue.Url}) - {cardissue.Title}");
                            }
                            else
                            {
                                
                            }    
                        }

                    }
                }
              
                


            }

           



            string output = sb.ToString();
            System.IO.File.WriteAllText($".\\{repository}.md", output);
            Console.WriteLine($"{user.Name} has {user.PublicRepos} public repositories");
           

        }
    }
}

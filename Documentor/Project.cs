using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Documentor.Properties;
using Octokit;

namespace Documentor
{
    /// <summary>
    /// This class creates reports on Projects
    /// </summary>
    public class Project
    {
        private readonly string _repository;
        private readonly string _owner;
        private readonly string _token;

        /// <summary>
        /// Creates a new instance of the project class
        /// </summary>
        /// <param name="repository">The Repo Name</param>
        /// <param name="owner">The Owner of the Repo</param>
        /// <param name="token">The Token</param>
        public Project(string repository, string owner, string token)
        {
            _repository = repository;
            _owner = owner;
            _token = token;
        }

        /// <summary>
        /// Creates the report in markdown and returns it as a string
        /// </summary>
        /// <returns>The Markdown string</returns>
        public async Task<string> CreateReport()
        {
            Console.WriteLine($"{Resources.AppTitle}");
            Console.WriteLine($"{Resources.Copyright}");
            Console.WriteLine($"{Resources.PlsWait}");

            Console.WriteLine($"{Resources.Step} 1 {Resources.of} 3 - {Resources.Headers}");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"# {Resources.ReportTitle}");
            sb.AppendLine("");
            sb.AppendLine($"## {Resources.StatusReportFor} {_repository} {Resources.CreatedOn} {DateTime.Now}");
            sb.AppendLine("");
            sb.AppendLine($"## {Resources.OverallInfo}");
            sb.AppendLine("");

            var client = new GitHubClient(new ProductHeaderValue("project-documentor"));
            var tokenAuth = new Credentials(_token);
            client.Credentials = tokenAuth;
            var user = await client.User.Current().ConfigureAwait(true);

            var repo = await client.Repository.Get(_owner, _repository).ConfigureAwait(true);

            sb.AppendLine($"| {Resources.Item} | {Resources.Value} |");
            sb.AppendLine($"| -- | -- |");
            sb.AppendLine($"| {Resources.DateCreated} | {repo.CreatedAt} |");
            sb.AppendLine($"| {Resources.DefaultBranch} | {repo.DefaultBranch} |");
            sb.AppendLine($"| {Resources.Description} | {repo.Description} |");
            sb.AppendLine($"| {Resources.FullName} | {repo.FullName} |");
            sb.AppendLine($"| {Resources.URL} | {repo.GitUrl} |");
            sb.AppendLine($"| {Resources.Name} | {repo.Name} |");
            sb.AppendLine($"| {Resources.CurrentOpenIssues} | {repo.OpenIssuesCount} |");
            sb.AppendLine($"| {Resources.LastCodeUpdate} | {repo.PushedAt} |");
            sb.AppendLine($"| {Resources.Subscribers} | {repo.WatchersCount} |");
            sb.AppendLine($"| {Resources.Last_Update} | {repo.UpdatedAt} |");

            sb.AppendLine("");
            sb.AppendLine($"## {Resources.Issues}");
            Console.WriteLine($"{Resources.Step} 2 {Resources.of} 3 - {Resources.IssuesModified365}");
            sb.AppendLine("");
            sb.AppendLine($"### {Resources.IssuesModified30}");
            var issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                Since = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(365)),
                SortProperty = IssueSort.Updated,
                SortDirection = SortDirection.Descending
            };
            var issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            sb.AppendLine("");
            sb.AppendLine($"| {Resources.IssueId} | {Resources.Modified} | {Resources.Status} | {Resources.Title} |");
            sb.AppendLine("| -------- | -------- | ------ | ----- |");
            foreach (var issue in issues)
            {
                sb.AppendLine($"| [{issue.Number}]({issue.Url}) | {issue.UpdatedAt.Value.ToLocalTime().ToString("dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture)} | {issue.State} | {issue.Title.Replace("|","-")} |");
            }

            sb.AppendLine("");
            sb.AppendLine($"### {Resources.Open_Issues}");
            sb.AppendLine("");
            sb.AppendLine($"| {Resources.IssueId} | {Resources.Modified} | {Resources.Status} | {Resources.Title} |");
            sb.AppendLine("| -------- | -------- | ------ | ----- |");
            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.Open,
                Since = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(30)),
                SortProperty = IssueSort.Updated,
                SortDirection = SortDirection.Descending
            };

            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var issue in issues)
            {

                sb.AppendLine($"| [{issue.Number}]({issue.Url}) | {issue.UpdatedAt.Value.ToLocalTime().ToString("dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture)} | {issue.State} | {issue.Title} |");

            }

            Console.WriteLine($"{Resources.Step} 3 {Resources.of} 3, {Resources.Projects}");
            sb.AppendLine("");
            sb.AppendLine($"## {Resources.Projects}");
            //Projects
            var projects = await client.Repository.Project.GetAllForRepository(_owner, _repository).ConfigureAwait(true);
            int projectid = 1;
            foreach (var project in projects)
            {
                Console.WriteLine($" Working on Project {projectid} of {projects.Count}: {project.Name}");
                projectid++;
                sb.AppendLine("");
                sb.AppendLine($"### {project.Name}");
                sb.AppendLine("");

                sb.AppendLine($"| {Resources.Item} | {Resources.Value} |");
                sb.AppendLine("| -----| ----- |");
                sb.AppendLine($"| {Resources.DateCreated} | {project.CreatedAt.ToLocalTime().ToString("dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture)} |");
                sb.AppendLine($"| {Resources.Project_Number} | {project.Number}");
                sb.AppendLine($"| {Resources.Status} | {project.State}");
                sb.AppendLine($"| {Resources.Modified} | {project.UpdatedAt.ToLocalTime().ToString("dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture)} |");
                sb.AppendLine("");

                sb.AppendLine($"#### {Resources.Project_Status}");
                sb.AppendLine("");
                var columns = await client.Repository.Project.Column.GetAll(project.Id).ConfigureAwait(true);

                List<string> lines = new List<string>();

                int colcount = 0;
                foreach (var column in columns)
                {
                    colcount++;
                    sb.AppendLine($"##### {column.Name}");
                    sb.AppendLine("");
                    Console.WriteLine($"  Column {colcount} of {columns.Count} - {column.Name}");
                    var cards = await client.Repository.Project.Card.GetAll(column.Id).ConfigureAwait(true);
                    int cardcount = 0;

                    //Handle no cards, produce nice message
                    if (cards.Count == 0)
                    {
                        sb.AppendLine($"     *{Resources.ThereAreNoIssuesinThisStatus}*");
                        sb.AppendLine();
                    }
                    foreach (var card in cards)
                    {
                        cardcount++;
                        Console.WriteLine($"         {Resources.Working_on_card}: {cardcount} {Resources.of} {cards.Count}");
                        if (column.Name != "Done" && column.Name != "Closed")
                        {
                            if (string.IsNullOrEmpty(card.Note))
                            {
                                string issueId = card.ContentUrl.Split("/")[card.ContentUrl.Split("/").Length - 1];
                                var cardissue = await client.Issue.Get(_owner, _repository, Convert.ToInt32(issueId, CultureInfo.CurrentCulture)).ConfigureAwait(true);
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
                                var cardissue = await client.Issue.Get(_owner, _repository, Convert.ToInt32(issueId, CultureInfo.CurrentCulture)).ConfigureAwait(true);
                                sb.AppendLine($"- [{issueId}]({cardissue.Url}) - {cardissue.Title}");
                            }
                            else
                            {

                            }
                        }

                    }
                }




            }
            return sb.ToString();

        }
    }
}

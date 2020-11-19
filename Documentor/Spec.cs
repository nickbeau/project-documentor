using Documentor.Properties;
using Octokit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Documentor
{
    /// <summary>
    /// This class creates reports as specifications
    /// </summary>
    public class Spec
    {
        private string _repository;
        private string _owner;
        private string _token;

        /// <summary>
        /// Creates a new instance of the spec class
        /// </summary>
        /// <param name="repository">The Repo Name</param>
        /// <param name="owner">The Owner of the Repo</param>
        /// <param name="token">The Token</param>
        public Spec(string repository, string owner, string token)
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
            sb.AppendLine($"# {Resources.Specification}");
            sb.AppendLine("");
            sb.AppendLine($"## {Resources.SpecificationFor} {_repository} {Resources.CreatedOn} {DateTime.Now}");
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
            sb.AppendLine($"| {Resources.Subscribers} | {repo.SubscribersCount} |");
            sb.AppendLine($"| {Resources.Last_Update} | {repo.UpdatedAt} |");

            sb.AppendLine("");
            sb.AppendLine("## Project Information");
            sb.AppendLine("");
            var readme = await client.Repository.Content.GetReadme(_owner, _repository);
            sb.Append(readme.Content);
            sb.AppendLine("");
            sb.AppendLine("## Non-Functional Requirements");
            sb.AppendLine("Non functional requirements are those which affect the environment of the running application but offer no additional value to the user");
            sb.AppendLine("");
            sb.AppendLine("### Overview");
            sb.AppendLine("");
            sb.AppendLine("| Id | Title | Priority |");
            sb.AppendLine("| --: | --- | --- |");

            var issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("Pri1");
            issuefilter.Labels.Add("NonFunctionalReq");
            var issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach(var item in issues)
            {
                sb.AppendLine($"| [{item.Number}]({item.HtmlUrl}) | {item.Title} | 1 |");
            }

            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("Pri2");
            issuefilter.Labels.Add("NonFunctionalReq");
            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine($"| [{item.Number}]({item.HtmlUrl}) | {item.Title} | 2 |");
            }

            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                Since = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(120)),
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("Pri3");
            issuefilter.Labels.Add("NonFunctionalReq");
            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine($"| [{item.Number}]({item.HtmlUrl}) | {item.Title} | 3 |");
            }
            //Do Pri 1 NonFunc
            //Do Pri 2 NonFunc
            //Do Pri 3 NonFunc

            sb.AppendLine("");
            sb.AppendLine("### Details");
            //Do All NonFunc with details
            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("NonFunctionalReq");
            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine("");
                sb.AppendLine($"### {item.Number} - {item.Title}");
                sb.AppendLine("");
               

                foreach (var lbl in item.Labels)
                {
                   switch(lbl.Name)
                        {
                        case "Pri1":
                            sb.AppendLine("> **Priority One**");
                            sb.AppendLine("");
                            break;
                        case "Pri2":
                            sb.AppendLine("> ***Priority Two***");
                            sb.AppendLine("");
                            break;
                        case "Pri3":
                            sb.AppendLine("> *Priority Three*");
                            sb.AppendLine("");
                            break;
                        case "SpecRequired":
                            sb.AppendLine("**Note:** This item still requires detailed specifications");
                            break;
                    }
                }

                sb.AppendLine("");
                sb.Append(item.Body);
                sb.AppendLine("");

            }





            sb.AppendLine("");
            sb.AppendLine("## Functional Requirements");
            sb.AppendLine("");
            sb.AppendLine("### Overview");
            sb.AppendLine("");
            sb.AppendLine("| Id | Title | Priority |");
            sb.AppendLine("| --: | --- | --- |");
            //Do Pri 1 Func

            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("Pri1");
            issuefilter.Labels.Add("FunctionalReq");
            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine($"| [{item.Number}]({item.HtmlUrl}) | {item.Title} | 1 |");
            }

            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("Pri2");
            issuefilter.Labels.Add("FunctionalReq");
            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine($"| [{item.Number}]({item.HtmlUrl}) | {item.Title} | 2 |");
            }

            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("Pri3");
            issuefilter.Labels.Add("FunctionalReq");
            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine($"| [{item.Number}]({item.HtmlUrl}) | {item.Title} | 3 |");
            }
            //Do Pri 2 Func
            //Do Pri 3 Func

            sb.AppendLine("");
            sb.AppendLine("### Details");
            //Do All Func with details
            sb.AppendLine("");
            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("FunctionalReq");
            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine("");
                sb.AppendLine($"### {item.Number} - {item.Title}");
                sb.AppendLine("");


                foreach (var lbl in item.Labels)
                {
                    switch (lbl.Name)
                    {
                        case "Pri1":
                            sb.AppendLine("> **Priority One**");
                            sb.AppendLine("");
                            break;
                        case "Pri2":
                            sb.AppendLine("> ***Priority Two***");
                            sb.AppendLine("");
                            break;
                        case "Pri3":
                            sb.AppendLine("> *Priority Three*");
                            sb.AppendLine("");
                            break;
                        case "SpecRequired":
                            sb.AppendLine("**Note:** This item still requires detailed specifications");
                            break;
                    }
                }

                sb.AppendLine("");
                sb.Append(item.Body);
                sb.AppendLine("");

            }
            return sb.ToString();
        }

        /// <summary>
        /// Creates a specificaiton with just Pri1 Items
        /// </summary>
        /// <returns></returns>
        public async Task<string> CreateMVPReport()
        {
            Console.WriteLine($"{Resources.AppTitle}");
            Console.WriteLine($"{Resources.Copyright}");
            Console.WriteLine($"{Resources.PlsWait}");
            Console.WriteLine($"{Resources.Step} 1 {Resources.of} 3 - {Resources.Headers}");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"# {Resources.Specification} - MVP");
            sb.AppendLine("");
            sb.AppendLine($"## {Resources.SpecificationFor} {_repository} {Resources.CreatedOn} {DateTime.Now}");
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
            sb.AppendLine($"| {Resources.Subscribers} | {repo.SubscribersCount} |");
            sb.AppendLine($"| {Resources.Last_Update} | {repo.UpdatedAt} |");

            sb.AppendLine("");
            sb.AppendLine("## Project Information");
            sb.AppendLine("");
            var readme = await client.Repository.Content.GetReadme(_owner, _repository);
            sb.Append(readme.Content);
            sb.AppendLine("");
            sb.AppendLine("## Non-Functional Requirements");
            sb.AppendLine("Non functional requirements are those which affect the environment of the running application but offer no additional value to the user");
            sb.AppendLine("");
            sb.AppendLine("### Overview");
            sb.AppendLine("");
            sb.AppendLine("| Id | Title | Priority |");
            sb.AppendLine("| --: | --- | --- |");

            var issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("Pri1");
            issuefilter.Labels.Add("NonFunctionalReq");
            var issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine($"| [{item.Number}]({item.HtmlUrl}) | {item.Title} | 1 |");
            }

           

            sb.AppendLine("");
            sb.AppendLine("### Details");
            //Do All NonFunc with details
            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("NonFunctionalReq");
            issuefilter.Labels.Add("Pri1");
            
            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine("");
                sb.AppendLine($"### {item.Number} - {item.Title}");
                sb.AppendLine("");


                foreach (var lbl in item.Labels)
                {
                    switch (lbl.Name)
                    {
                        case "Pri1":
                            sb.AppendLine("> **Priority One**");
                            sb.AppendLine("");
                            break;
                        case "Pri2":
                            sb.AppendLine("> ***Priority Two***");
                            sb.AppendLine("");
                            break;
                        case "Pri3":
                            sb.AppendLine("> *Priority Three*");
                            sb.AppendLine("");
                            break;
                        case "SpecRequired":
                            sb.AppendLine("**Note:** This item still requires detailed specifications");
                            break;
                    }
                }

                sb.AppendLine("");
                sb.Append(item.Body);
                sb.AppendLine("");

            }





            sb.AppendLine("");
            sb.AppendLine("## Functional Requirements");
            sb.AppendLine("");
            sb.AppendLine("### Overview");
            sb.AppendLine("");
            sb.AppendLine("| Id | Title | Priority |");
            sb.AppendLine("| --: | --- | --- |");
            //Do Pri 1 Func

            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("Pri1");
            issuefilter.Labels.Add("FunctionalReq");
            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine($"| [{item.Number}]({item.HtmlUrl}) | {item.Title} | 1 |");
            }

           

           
            //Do Pri 2 Func
            //Do Pri 3 Func

            sb.AppendLine("");
            sb.AppendLine("### Details");
            //Do All Func with details
            sb.AppendLine("");
            issuefilter = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending
            };
            issuefilter.Labels.Add("FunctionalReq");
            issuefilter.Labels.Add("Pri3");
            issues = await client.Issue.GetAllForRepository(_owner, _repository, issuefilter).ConfigureAwait(true);
            foreach (var item in issues)
            {
                sb.AppendLine("");
                sb.AppendLine($"### {item.Number} - {item.Title}");
                sb.AppendLine("");


                foreach (var lbl in item.Labels)
                {
                    switch (lbl.Name)
                    {
                        case "Pri1":
                            sb.AppendLine("> **Priority One**");
                            sb.AppendLine("");
                            break;
                        case "Pri2":
                            sb.AppendLine("> ***Priority Two***");
                            sb.AppendLine("");
                            break;
                        case "Pri3":
                            sb.AppendLine("> *Priority Three*");
                            sb.AppendLine("");
                            break;
                        case "SpecRequired":
                            sb.AppendLine("**Note:** This item still requires detailed specifications");
                            break;
                    }
                }

                sb.AppendLine("");
                sb.Append(item.Body);
                sb.AppendLine("");

            }
            return sb.ToString();
        }

    }
}

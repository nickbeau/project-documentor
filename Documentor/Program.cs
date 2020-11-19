using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using System.CommandLine;
using System.CommandLine.Invocation;
using Documentor.Properties;
using System.Linq.Expressions;
using System.Globalization;
using System.Net.Http.Headers;

namespace Documentor
{
    class Program
    {
        /// <summary>
        /// Produces a markdown status report of a github repo. The report is delivered into this directory with the 
        /// same filename as the repository chosen below. This markdown file can be printed to pdf or edited in a markdown
        /// editor.
        /// </summary>
        /// <param name="repository">The GitHub Repository.</param>
        /// <param name="owner">The Owner's Github Alias.</param>
        /// <param name="token">The GitHub Token.</param>
        /// <param name="doctype">The type of document to build, project or spec</param>
        /// <returns></returns>
        public static async Task Main(string repository, string owner, string token, string doctype)
        {
            string output;
            switch (doctype.ToUpperInvariant())
            {
                case "PROJECT":
                    var myProject = new Project(repository, owner, token);
                    output = await myProject.CreateReport().ConfigureAwait(false);
                    System.IO.File.WriteAllText($".\\{repository}-projectreport.md", output);
                    break;
                case "SPEC":
                    var mySpec = new Spec(repository, owner, token);
                    output = await mySpec.CreateReport().ConfigureAwait(false);
                    System.IO.File.WriteAllText($".\\{repository}-specification.md", output);
                    break;
                case "MVPSPEC":
                    mySpec = new Spec(repository, owner, token);
                    output = await mySpec.CreateMVPReport().ConfigureAwait(false);
                    System.IO.File.WriteAllText($".\\{repository}-mvpspecification.md", output);
                    break;
                default:
                    break;

            }

           
           



           
            
            Console.WriteLine($"{Resources.Complete}.");
           

        }
    }
}

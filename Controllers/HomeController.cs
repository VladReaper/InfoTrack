using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Results(string searchPhrase)
        {
            ViewBag.SearchPhrase = searchPhrase;
            var searchResults = GetSearchResults(GetResultsFromGoogle(searchPhrase));
            var matchingResults = new List<string>();
            for(int i = 0; i < searchResults.Count; i++)
            {
                if(searchResults[i].Value.Contains("infotrack.co.uk"))
                {
                    matchingResults.Add((i+1).ToString());
                }
            }

            if (!matchingResults.Any())
                matchingResults.Add("0");

            ViewBag.Places = string.Join(",", matchingResults);
            return View();
        }

        private string GetResultsFromGoogle(string searchPhrase)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://www.google.co.uk/search?num=100&q={searchPhrase}");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private MatchCollection GetSearchResults(string results)
        {
            var regex = new Regex("(?:<a href=\"/url\\?q=)(.*?)(?:>)", RegexOptions.ECMAScript);
            var matches = regex.Matches(results);
            return matches;
        }
    }
}
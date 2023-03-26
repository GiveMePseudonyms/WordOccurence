using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;

namespace WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter a website URL: ");
            string url = Console.ReadLine();

            //build a http client and read the entire website content to a document
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            string content = response.Content.ReadAsStringAsync().Result;

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(content);


            //split nodes into 'words' and track how many occurences of each word there are
            var wordCount = new Dictionary<string, int>();

            foreach (var textNode in htmlDocument.DocumentNode.DescendantsAndSelf().Where(n => n.NodeType == HtmlNodeType.Text))
            {
                string[] words = textNode.InnerHtml.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string word in words)
                {
                    if (wordCount.ContainsKey(word))
                    {
                        wordCount[word]++;
                    }
                    else
                    {
                        wordCount.Add(word, 1);
                    }
                }
            }

            var mostCommonWords = wordCount.OrderByDescending(x => x.Value).Take(10);

            Console.WriteLine("The 10 most common words on the website:");

            int maxFrequency = mostCommonWords.First().Value;

            //make chart and show user.
            foreach (var word in mostCommonWords)
            {
                string bar = new string('=', (int)Math.Round((double)word.Value / maxFrequency * 50));
                Console.WriteLine($"{word.Key.PadRight(20)}|{bar} {word.Value}");
            }
            Console.ReadLine();
        }
    }
}

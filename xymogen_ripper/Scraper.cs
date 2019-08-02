using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace xymogen_ripper
{
    public class Scraper
    {
		protected void sanitizeNode(HtmlNode node)
		{
			List<HtmlNode> toRemove = new List<HtmlNode>();
			foreach (HtmlNode n in node.ChildNodes)
			{
				if (n.Name == "#text" && string.IsNullOrWhiteSpace(n.OuterHtml))
				{
					toRemove.Add(n);
				}
				else
				{
					sanitizeNode(n);
				}
			}

			foreach (HtmlNode n in toRemove)
			{
				node.ChildNodes.Remove(n);
			}
		}

		protected string rawHtml;

		protected HtmlDocument GetPage(string url, WebClient wc)
		{
			WebClient client = wc;

			if (client == null)
			{
				client = new WebClient();
			}
			rawHtml = client.DownloadString(new Uri(url));
			if (wc == null)
			{
				client.Dispose();
			}
//			while (rawHtml.Contains("\n "))
//			{
//				rawHtml = rawHtml.Replace("\n ", "\n");
//			}

//			rawHtml = rawHtml.Replace("\n", "\r\n");
//			rawHtml = rawHtml.Replace("\r\r\n", "\r\n");
//			rawHtml = rawHtml.Replace("\r\n\r\n", "\r\n");
//			rawHtml = rawHtml.Replace("\r\n", "\n");
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(rawHtml);
			sanitizeNode(doc.DocumentNode);
			return doc;
		}

		protected HtmlDocument GetPage2(string url, WebClient wc)
		{
			HtmlWeb htmlWeb = new HtmlWeb();

			HtmlDocument doc = htmlWeb.Load(url);
			sanitizeNode(doc.DocumentNode);
			return doc;
		}


		protected static void CleanupInventory(string input, string output)
		{
			if (!File.Exists(output))
			{
				List<string> lines = File.ReadAllLines(input).ToList();
				lines.RemoveAll(l => l == "    \"\": \"\"");
				lines.RemoveAll(l => l.StartsWith("    \"Cost\": \"\""));
				lines.RemoveAll(l => l.StartsWith("    \"Retail\": \"\""));
				lines.RemoveAll(l => l.StartsWith("    \"Cost\": \"$\""));
				lines.RemoveAll(l => l.StartsWith("    \"Retail\": \"$\""));
				lines.RemoveAll(l => l.StartsWith("    \"Cost\": \"$-\""));
				lines.RemoveAll(l => l.StartsWith("    \"Retail\": \"$-\""));

				lines.RemoveAll(l => l.StartsWith("    \"Cost\": \"TBA\""));
				lines.RemoveAll(l => l.StartsWith("    \"Retail\": \"TBA\""));
				lines.RemoveAll(l => l.StartsWith("    \"Cost\": \"N/A\""));
				lines.RemoveAll(l => l.StartsWith("    \"Retail\": \"N/A\""));

				int index = 0;
				while ((index = lines.FindIndex(index, l => l.Contains("\"Cost\":") || l.Contains("\"Retail\":"))) != -1)
				{
					lines[index] = lines[index].Replace("\"$", "").Replace("\",", ",").TrimEnd('"');
					index++;
				}

				index = 0;
				while ((index = lines.FindIndex(index, l => l.Contains("\"Active?\": \"Y\""))) != -1)
				{
					lines[index] = lines[index].Replace("\"Active?", "\"Active").Replace("\"Y\"", "true").TrimEnd('"');
					index++;
				}

				index = 0;
				while ((index = lines.FindIndex(index, l => l.Contains("\"Active?\": \"N\""))) != -1)
				{
					lines[index] = lines[index].Replace("\"Active?", "\"Active?").Replace("\"N\"", "false").TrimEnd('"');
					index++;
				}

				index = 0;
				while ((index = lines.FindIndex(index, l => l.Contains("}"))) != -1)
				{
					if (lines[index - 1].EndsWith(','))
					{
						lines[index - 1] = lines[index - 1].Substring(0, lines[index - 1].Length - 1);
					}
					index++;
				}

				File.WriteAllLines(output, lines);
			}
		}



	}
}

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace m1201.Library
{
    public class Scraper
    {
        private const string RootDir = @"..\..\..\results";

        private readonly string _currDir;

        private readonly string _url;

        public Scraper(string url)
        {
            this._url = url;

            this._currDir = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-FFF");

            Directory.CreateDirectory(Path.Combine(RootDir, _currDir));
        }

        public void Do()
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = client.GetAsync(_url).Result)
            using (HttpContent content = response.Content)
            {
                string result = content.ReadAsStringAsync().Result;

                File.WriteAllText(Path.Combine(RootDir, _currDir, "index.html"), result);

                var html = new HtmlDocument();

                html.Load(Path.Combine(RootDir, _currDir, "index.html"));

                var links = html.DocumentNode
                    .Descendants("a")
                    .Where(m => m.Attributes?["href"]?.Value != null
                           && !Regex.IsMatch(m.Attributes["href"]?.Value, "^#"));

                Directory.CreateDirectory(Path.Combine(RootDir, _currDir, "html"));

                foreach (var l in links)
                {
                    Console.WriteLine($"Downloading {l.Attributes["href"].Value}...");

                    GetAndReplaceFileAsync(l, Path.Combine(RootDir, _currDir), "html", ".html", "href", client).Wait();

                    html.Save(Path.Combine(RootDir, _currDir, "index.html"));
                }

                var cssLinks = html.DocumentNode
                    .Descendants("link")
                    .Where(m => m.Attributes?["href"]?.Value != null
                                && Regex.IsMatch(m.Attributes["href"].Value, @"\.css$"));

                Directory.CreateDirectory(Path.Combine(RootDir, _currDir, "css"));

                foreach (var l in cssLinks)
                {
                    Console.WriteLine($"Downloading {l.Attributes["href"].Value}...");

                    GetAndReplaceFileAsync(l, Path.Combine(RootDir, _currDir), "css", ".css", "href", client).Wait();

                    html.Save(Path.Combine(RootDir, _currDir, "index.html"));
                }

                var jsLinks = html.DocumentNode
                    .Descendants("script")
                    .Where(m => m.Attributes?["src"] != null);

                Directory.CreateDirectory(Path.Combine(RootDir, _currDir, "js"));

                foreach (var l in jsLinks)
                {
                    Console.WriteLine($"Downloading {l.Attributes["src"].Value}...");

                    GetAndReplaceFileAsync(l, Path.Combine(RootDir, _currDir), "js", ".js", "src", client).Wait();

                    html.Save(Path.Combine(RootDir, _currDir, "index.html"));
                }

                var imageLinks = html.DocumentNode
                    .Descendants("img")
                    .Where(m => m.Attributes?["src"]?.Value != null);

                Directory.CreateDirectory(Path.Combine(RootDir, _currDir, "images"));

                foreach (var l in imageLinks)
                {
                    Console.WriteLine($"Downloading {l.Attributes["src"].Value}...");

                    GetAndReplaceFileAsync(l, Path.Combine(RootDir, _currDir), "images", null, "src", client, true).Wait();

                    html.Save(Path.Combine(RootDir, _currDir, "index.html"));
                }
            }
        }

        /// <summary>
        /// Common method for link content download.
        /// </summary>
        /// <param name="linkNode">HAP node</param>
        /// <param name="directory">Directory containing index.html</param>
        /// <param name="subdirectory">Subdirectory for concrete files</param>
        /// <param name="extension">Concrete files extension</param>
        /// <param name="sourceAttr">Source attribute of concrete links</param>
        /// <param name="client">HttpClient</param>
        /// <param name="isImage">Optonal flag to mark image links</param>
        /// <returns>Task</returns>
        private async Task GetAndReplaceFileAsync(HtmlNode linkNode,
                                                  string directory,
                                                  string subdirectory,
                                                  string extension,
                                                  string sourceAttr,
                                                  HttpClient client,
                                                  bool isImage = false)
        {
            try
            {
                var linkUri = new Uri(linkNode.Attributes[sourceAttr].Value, UriKind.RelativeOrAbsolute);

                if (!linkUri.IsAbsoluteUri)
                {
                    linkUri = new Uri(new Uri(_url), linkUri);
                }

                var cts = new CancellationTokenSource();

                cts.CancelAfter(3000);

                string w;

                string path;

                if (isImage)
                {
                    var imageBytes = await Task.Run(() => client.GetByteArrayAsync(linkUri), cts.Token);

                    w = Path.Combine(directory, subdirectory, Path.GetFileName(linkUri.LocalPath));

                    path = Path.Combine(subdirectory, Path.GetFileName(linkUri.LocalPath));

                    File.WriteAllBytes(w, imageBytes);
                }
                else
                {
                    var response = await client.GetAsync(linkUri, cts.Token);

                    var result = await response.Content.ReadAsStringAsync();

                    var fileName = DateTime.Now.ToString("mm-ss-FFFFFF") + extension;

                    w = Path.Combine(directory, subdirectory, fileName);

                    File.WriteAllText(w, result);

                    path = Path.Combine(subdirectory, fileName);
                }

                linkNode.Attributes[sourceAttr].Value = path;
            }
            catch (Exception) { }
        }
    }
}

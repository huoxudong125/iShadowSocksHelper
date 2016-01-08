using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Network;

namespace GetShadowSocksPWD
{
    internal class Program
    {
        private static readonly string configfile = "gui-config.json";
        private static void Main(string[] args)
        {
            GenrateFreeServersConfig();

            Console.Read();
        }

        private static void GenrateFreeServersConfig()
        {
            try
            {
                var browser = new ScrapingBrowser();

                //set UseDefaultCookiesParser as false if a website returns invalid cookies format
                //browser.UseDefaultCookiesParser = false;

                var homePage = browser.NavigateToPage(new Uri("http://www.ishadowsocks.com/"));

                var freeSection = homePage.Find("Section", By.Id("free")).FirstOrDefault();
                if (freeSection == null)
                {
                    Console.WriteLine("Can't find the Free section.");
                }

                //PageWebForm form = homePage.FindFormById("sb_form");
                //form["q"] = "scrapysharp";
                //form.Method = HttpVerb.Get;
                //WebPage resultsPage = form.Submit();

                //HtmlNode[] resultsLinks = resultsPage.Html.CssSelect("div.sb_tlst h3 a").ToArray();

                var serverNodes = homePage.Html.CssSelect("#free  >div.container > div.row > div.col-lg-4");

                var rootObject = new RootObject()
                {
                    configs = new List<ServerConfig>(),
                    index = -1,
                    strategy = "com.shadowsocks.strategy.balancing",
                    global = false,
                    enabled = true,
                    shareOverLan = false,
                    isDefault = false,
                    localPort = 1080,
                    pacUrl = null,
                    useOnlinePac = false,
                    availabilityStatistics = false
                };

                Console.WriteLine("Read Servers from HTML");

                var serverList = rootObject.configs;

                Console.WriteLine("Parse the server html");
                foreach (var serverNode in serverNodes)
                {
                    var h4nodes = serverNode.ChildNodes.Where(n => n.Name.Contains("h4")).ToList();
                    var server = new ServerConfig()
                    {
                        server = h4nodes[0].InnerText.Split(':')[1],
                        server_port = int.Parse(h4nodes[1].InnerText.Split(':')[1]),
                        password = h4nodes[2].InnerText.Split(':')[1],
                        method = h4nodes[3].InnerText.Split(':')[1],
                        remarks = h4nodes[0].InnerText.Split(':')[1],
                    };
                    serverList.Add(server);
                }

                Console.WriteLine("Serialize the config to file.");
                var path = "server.txt";
                WriteServersToFile(rootObject,path);

                Console.WriteLine("Update the gui-config.json");

                File.Delete(configfile);

                File.Move(path, configfile);

                Console.WriteLine("Successful! Enjoy yourself!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sorry,Run Error:{0}", ex.Message);
            }
        }

        private static void WriteServersToFile(RootObject rootObject,string path)
        {
            //var str = JsonHelper.FormatJson(JsonHelper.Serialize(rootObject));
            var str = JsonConvert.SerializeObject(rootObject,Formatting.Indented);
            
            using (var file = new StreamWriter(path, false, Encoding.UTF8))
            {
                file.Write(str);
            }
        }
    }
}
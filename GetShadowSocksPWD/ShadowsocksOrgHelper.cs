using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetShadowSocksPWD;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Network;

namespace iShadowSocksHelper
{
   public class ShadowsocksOrgHelper
    {
        private static RootObject GenrateFreeServersConfig()
        {
            try
            {
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

                var serverList = rootObject.configs;

                try
                {
                    GetIshadowsocksServers(serverList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                try
                {
                    GetFreeShadowsocksServers(serverList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return rootObject;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Sorry,Run Error:{0}", ex.Message);
            }
            return null;
        }

        private static void GetIshadowsocksServers(List<ServerConfig> serverList)
        {
            var browser = new ScrapingBrowser();

            //set UseDefaultCookiesParser as false if a website returns invalid cookies format
            //browser.UseDefaultCookiesParser = false;
            Console.WriteLine("Open website http://www.ishadowsocks.org/");
            var homePage = browser.NavigateToPage(new Uri("http://www.ishadowsocks.org/"));

            var freeSection = homePage.Find("Section", By.Id("free")).FirstOrDefault();
            if (freeSection == null)
            {
                Console.WriteLine("Can't find the Free section.");
            }

            var serverNodes = homePage.Html.CssSelect("#free  >div.container > div.row > div.col-sm-4");

            Console.WriteLine("Read Servers from HTML");

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
        }

        private static void GetFreeShadowsocksServers(List<ServerConfig> serverList)
        {
            var browser = new ScrapingBrowser();

            //set UseDefaultCookiesParser as false if a website returns invalid cookies format
            //browser.UseDefaultCookiesParser = false;
            Console.WriteLine("Open website http://freeshadowsocks.cf/");
            var homePage = browser.NavigateToPage(new Uri("http://freeshadowsocks.cf/"));

            var serverNodes = homePage.Html.CssSelect("div.container > div.row > div.col-md-4");

            Console.WriteLine("Read Servers from HTML");

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
        }

    }
}

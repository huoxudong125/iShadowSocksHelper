using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using iShadowSocksHelper;
using Newtonsoft.Json;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Network;

namespace GetShadowSocksPWD
{
    internal class Program
    {
        private static readonly string configfile = "gui-config.json";

        private  static void Main(string[] args)
        {
            //GenrateFreeServersConfig();
            //ReStartShadowSocks();

            FreessOrgHelper.GetIshadowsocksServers();

            Console.WriteLine("Enter any key to exit.");
            Console.Read();
        }

        private static void GenrateFreeServersConfig()
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

                Console.WriteLine("Serialize the config to file.");
                var path = "server.txt";
                WriteServersToFile(rootObject, path);

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

        private static void WriteServersToFile(RootObject rootObject, string path)
        {
            //var str = JsonHelper.FormatJson(JsonHelper.Serialize(rootObject));
            var str = JsonConvert.SerializeObject(rootObject, Formatting.Indented);

            using (var file = new StreamWriter(path, false, Encoding.UTF8))
            {
                file.Write(str);
            }
        }

        private static void ReStartShadowSocks()
        {
            try
            {
                var exeFile = "shadowsocks";

                Console.WriteLine("Try to kill the process [{0}]", exeFile);
                //kill the process
                Process.GetProcesses()
                             .Where(x => x.ProcessName.ToLower()
                                          .StartsWith(exeFile))
                             .ToList().ForEach(p => KillProcessAndChildren(p.Id));
                //.ForEach(x => x.Kill());

                Console.WriteLine("{1}Start the process [{0}]", exeFile, Environment.NewLine);

                //start
                Process.Start(exeFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:{0}", ex.Message);
            }
        }

        /// <summary>
        /// Kill a process, and all of its children, grandchildren, etc.
        /// </summary>
        /// <param name="pid">Process ID.</param>
        private static void KillProcessAndChildren(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
              ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);

                proc.Kill();
                Console.WriteLine("Kill Process :{0}", proc.ProcessName);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Kill Process error:{0}", ex.Message);
                // Process already exited.
            }
        }
    }
}
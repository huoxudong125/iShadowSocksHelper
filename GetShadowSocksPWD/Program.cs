﻿using System;
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
           

            var rootobject = FreessOrgHelper.GetFullServerConfig();
            UpdateGuiConfig(rootobject);

            ReStartShadowSocks();

            Console.WriteLine("Enter any key to exit.");
            Console.Read();
        }

        private static void UpdateGuiConfig(object rootObject)
        {
            Console.WriteLine("Serialize the config to file.");
            var path = "server.txt";
            WriteServersToFile(rootObject, path);

            Console.WriteLine("Update the gui-config.json");

            File.Delete(configfile);

            File.Move(path, configfile);

            Console.WriteLine("Successful! Enjoy yourself!");
        }

        private static void WriteServersToFile(Object rootObject, string path)
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
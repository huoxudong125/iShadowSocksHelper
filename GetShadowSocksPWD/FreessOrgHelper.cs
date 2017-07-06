using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using iShadowSocksHelper.Domain;
using iShadowSocksHelper.Utils;
using ZXing;

namespace iShadowSocksHelper
{
    public class FreessOrgHelper
    {
        private static List<string> _imageUrls;
        public static readonly Regex
            UrlFinder = new Regex(@"ss://(?<base64>[A-Za-z0-9+-/=_]+)(?:#(?<tag>\S+))?", RegexOptions.IgnoreCase),
            DetailsParser = new Regex(@"^((?<method>.+?):(?<password>.*)@(?<hostname>.+?):(?<port>\d+?))$", RegexOptions.IgnoreCase);


        static FreessOrgHelper()
        {
            _imageUrls = new List<string> {
            "http://freess.org/images/servers/jp01.png",
            "http://freess.org/images/servers/jp02.png",
            "http://freess.org/images/servers/jp03.png"};
        }


        public static  List<Config> GetIshadowsocksServers()
        {
            List<Config> serverConfigs = new List<Config>();
            foreach (var imgUrl in _imageUrls)
            {
               var imageFilePath=  ImageDownloader.LoadImage(imgUrl).Result;

                if (!string.IsNullOrEmpty(imageFilePath))
                {

                    // create a barcode reader instance
                    IBarcodeReader reader = new BarcodeReader();
                    // load a bitmap
                    var barcodeBitmap = new Bitmap(imageFilePath);
                    // detect and decode the barcode inside the bitmap
                    var ssURL = reader.Decode(barcodeBitmap);
                   
                    // do something with the result
                    if (ssURL != null)
                    {
                        Debug.WriteLine(ssURL.BarcodeFormat.ToString());
                        Debug.WriteLine(ssURL.Text);

                        var matches = UrlFinder.Matches(ssURL.Text);
                        if (matches.Count <= 0) return null;
                        foreach (Match match in matches)
                        {
                            var tmp = new Config();
                            var base64 = match.Groups["base64"].Value;
                            var tag = match.Groups["tag"].Value;
                            if (!string.IsNullOrEmpty(tag))
                            {
                                tmp.remarks = HttpUtility.UrlDecode(tag, Encoding.UTF8);
                            }
                            Match details = DetailsParser.Match(Encoding.UTF8.GetString(Convert.FromBase64String(
                                base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '='))));
                            if (!details.Success)
                                continue;
                            tmp.method = details.Groups["method"].Value;
                            tmp.password = details.Groups["password"].Value;
                            tmp.server = details.Groups["hostname"].Value;
                            tmp.server_port = int.Parse(details.Groups["port"].Value);

                            serverConfigs.Add(tmp);
                        }
                       
                    }
                }
               
            }

          

            return serverConfigs;


        }

        public static Rootobject GetFullServerConfig()
        {
            var root = new Rootobject()
            {
                strategy = "com.shadowsocks.strategy.scbs",
                index = -1,
                global = false,
                enabled = false,
                shareOverLan = false,
                isDefault = false,
                localPort = 1080,
                pacUrl = null,
                useOnlinePac = false,
                secureLocalPac = true,
                availabilityStatistics = false,
                autoCheckUpdate = true,
                checkPreRelease = false,
                isVerboseLogging = true
            };
            var logViewer=new Logviewer()
            {
                topMost = false,
                wrapText = false,
                toolbarShown = false,
                Font = "Consolas, 8pt",
                BackgroundColor = "Black",
                TextColor="White"
            };
            root.logViewer = logViewer;

            var proxy=new Proxy()
            {
                useProxy = false,
                proxyType = 0,
                proxyServer = "",
                proxyPort = 0,
                proxyTimeout = 3
            };
            root.proxy = proxy;

            var hotKey=new Hotkey()
            {
                SwitchSystemProxy = "",
                SwitchSystemProxyMode = "",
                SwitchAllowLan = "",
                ShowLogs = "",
                ServerMoveUp = "",
                ServerMoveDown = ""
            };

            root.hotkey = hotKey;
            root.configs = GetIshadowsocksServers();

            return root;
        }
        
    }
}

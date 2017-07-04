using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iShadowSocksHelper.Domain
{
 
    public class Rootobject
    {
        public Config[] configs { get; set; }
        public string strategy { get; set; }
        public int index { get; set; }
        public bool global { get; set; }
        public bool enabled { get; set; }
        public bool shareOverLan { get; set; }
        public bool isDefault { get; set; }
        public int localPort { get; set; }
        public object pacUrl { get; set; }
        public bool useOnlinePac { get; set; }
        public bool secureLocalPac { get; set; }
        public bool availabilityStatistics { get; set; }
        public bool autoCheckUpdate { get; set; }
        public bool checkPreRelease { get; set; }
        public bool isVerboseLogging { get; set; }
        public Logviewer logViewer { get; set; }
        public Proxy proxy { get; set; }
        public Hotkey hotkey { get; set; }
    }

    public class Logviewer
    {
        public bool topMost { get; set; }
        public bool wrapText { get; set; }
        public bool toolbarShown { get; set; }
        public string Font { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
    }

    public class Proxy
    {
        public bool useProxy { get; set; }
        public int proxyType { get; set; }
        public string proxyServer { get; set; }
        public int proxyPort { get; set; }
        public int proxyTimeout { get; set; }
    }

    public class Hotkey
    {
        public string SwitchSystemProxy { get; set; }
        public string SwitchSystemProxyMode { get; set; }
        public string SwitchAllowLan { get; set; }
        public string ShowLogs { get; set; }
        public string ServerMoveUp { get; set; }
        public string ServerMoveDown { get; set; }
    }

    public class Config
    {
        public string server { get; set; }
        public int server_port { get; set; }
        public string password { get; set; }
        public string method { get; set; }
        public string remarks { get; set; }
        public int timeout { get; set; }
    }

}

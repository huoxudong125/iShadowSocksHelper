using System.Collections.Generic;

namespace GetShadowSocksPWD
{
    public class RootObject
    {
        public List<ServerConfig> configs { get; set; }
        public string strategy { get; set; }
        public int index { get; set; }
        public bool global { get; set; }
        public bool enabled { get; set; }
        public bool shareOverLan { get; set; }
        public bool isDefault { get; set; }
        public int localPort { get; set; }
        public object pacUrl { get; set; }
        public bool useOnlinePac { get; set; }
        public bool availabilityStatistics { get; set; }
    }
}
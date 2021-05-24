using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaBot.Utopia
{
    class SystemInfo
    {
        public class Result
        {
            public string buildAbi { get; set; }
            public string buildCpuArchitecture { get; set; }
            public string build_number { get; set; }
            public string currentCpuArchitecture { get; set; }
            public int netCoreRate { get; set; }
            public int networkCores { get; set; }
            public bool networkEnabled { get; set; }
            public int numberOfConnections { get; set; }
            public int packetCacheSize { get; set; }
            public string uptime { get; set; }
        }

        public class ResultExtraInfo
        {
            public string elapsed { get; set; }
        }

        public class Root
        {
            public string error { get; set; }
            public Result result { get; set; }
            public ResultExtraInfo resultExtraInfo { get; set; }
        }
    }
}

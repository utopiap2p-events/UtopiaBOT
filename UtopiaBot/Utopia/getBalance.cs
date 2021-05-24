using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaBot.Utopia
{
    class getBalance
    {
        public class ResultExtraInfo
        {
            public string elapsed { get; set; }
        }

        public class Root
        {
            public int result { get; set; }
            public ResultExtraInfo resultExtraInfo { get; set; }
        }
    }
}

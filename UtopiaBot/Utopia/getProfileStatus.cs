using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaBot.Utopia
{
    class getProfileStatus
    {
        public class Result
        {
            public string mood { get; set; }
            public string status { get; set; }
            public int status_code { get; set; }
        }

        public class Root
        {
            public Result result { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaBot.Utopia
{
    class getCards
    {
        public class Result
        {
            public double balance { get; set; }
            public string cardid { get; set; }
            public string color { get; set; }
            public string created { get; set; }
            public string name { get; set; }
        }

        public class Root
        {
            public List<Result> result { get; set; }
        }


    }
}

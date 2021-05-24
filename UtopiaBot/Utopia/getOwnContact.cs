using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaBot.Utopia
{
    class getOwnContact
    {
        public class Result
        {
            public string avatarMd5 { get; set; }
            public string firstName { get; set; }
            public string hashedPk { get; set; }
            public bool isFriend { get; set; }
            public string lastName { get; set; }
            public string moodMessage { get; set; }
            public string nick { get; set; }
            public string pk { get; set; }
            public int status { get; set; }
        }

        public class Root
        {
            public Result result { get; set; }
        }


    }
}

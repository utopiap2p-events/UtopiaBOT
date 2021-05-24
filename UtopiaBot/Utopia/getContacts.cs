using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaBot.Utopia
{
    class getContacts
    {
        public class Result
        {
            public int authorizationStatus { get; set; }
            public string avatarMd5 { get; set; }
            public string group { get; set; }
            public string hashedPk { get; set; }
            public bool isFriend { get; set; }
            public DateTime? lastSeen { get; set; }
            public string moodMessage { get; set; }
            public string nick { get; set; }
            public string pk { get; set; }
            public int status { get; set; }
        }

        public class Root
        {
            public List<Result> result { get; set; }
        }


    }
}

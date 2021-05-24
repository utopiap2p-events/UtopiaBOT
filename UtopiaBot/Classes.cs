using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaBot
{
    public class Classes
    {
        public class Settings
        {
            public string tg_token { get; set; }
            public List<User> users { get; set; }
        }

        public class User
        {
            //Telegram userdata
            public bool auth { get; set; }
            public int chatid { get; set; }
            public string domain { get; set; }
            public int id { get; set; }

            //Utopia API
            public string utopia_host { get; set; }
            public int utopia_port { get; set; }
            public string utopia_token { get; set; }
        }

        //ListView class
        public class lstUser
        {
            public int IDUser { get; set; }
            public string DomainUser { get; set; }
            public int ChatIdUser { get; set; }
            public string AuthUser { get; set; }
        }

        //

    }
}

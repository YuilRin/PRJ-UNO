using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO
{
    class ThisPlayer
    {
        public static string name { get; set; }
        public static int turn { get; set; }
        public static int numOfCards { get; set; }
        public static List<string> cards = new List<string>();
    }

    class OtherPlayers
    {
        public string name { get; set; }
        public string turn { get; set; }
        public string numofCards { get; set; }
    }
}

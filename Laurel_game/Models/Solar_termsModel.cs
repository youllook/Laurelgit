using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laurel_game.Models
{
    public class Solar_termsModel
    {
        public int month { get; set; }      
        public int randomWeek { get; set; }
        public string terms { get; set; }       
        public string product { get; set; } //all salad dumpling riceball
        public string event_name { get; set; }
        public int event_min { get; set; }
        public int event_max { get; set; }
        public Solar_termsModel()
        {
            randomWeek = 1;
        }
    }
}
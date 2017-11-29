using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laurel_game.Models
{
    public class RoomModel
    {
        public string key { get; set; }
        public string RoomId { get; set; }             //房間id
        public string Mode { get; set; }               //模式 單人(one)/多人(multi)
        public int Week { get; set; }
        public int Countdown { get; set; }
        public DateTime StartTime { get; set; }        //遊戲開始時間
        public DateTime EndTime { get; set; }          //遊戲結束時間
        public StorageModel Storage { get; set; }

        public RoomModel()
        {
            this.key = Guid.NewGuid().ToString();
            this.RoomId = "";
            this.Mode = "";
            this.Week = 0;
            this.Countdown = 15;
            this.StartTime = DateTime.Now;
            this.EndTime = DateTime.Now;
        }
    }
}
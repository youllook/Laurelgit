using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laurel_game.Models
{
    public class UserModel
    {
        public string ConnectID { get; set; } //Browser ID
        public string RoomID { get; set; }    //名稱
        public string Role { get; set; }      //名稱
        public string userNo { get; set; }    //員工編號
        public string password { get; set; }  //密碼
        public string Name { get; set; }  //密碼
        public UserModel()
        {
            this.ConnectID = "";
            this.RoomID = "";
            this.Role = "";
            this.userNo = "";
            this.password = "";
            this.Name = "";
        }
    }

    //員工資料
    public class UserProfileModel
    {
        public string No { get; set; }        //員工編號
        public string Password { get; set; }  //密碼
        public string Name { get; set; }      //名稱
    }
    //玩家資料
    public class PlayerModel: UserProfileModel
    {
        public string Role { get; set; }      //角色代碼
        public bool IsAI { get; set; }        //是否為AI
    }
    //玩家註冊
    public class PlayerRegistModel : UserProfileModel
    {
        public string RoomId { get; set; }
        public string Mode { get; set; }
    }

   
}
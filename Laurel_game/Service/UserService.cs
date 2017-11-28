using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Laurel_game.Models;
using Laurel_game.Library;

namespace Laurel_game.Service
{
    public class UserService
    {
        // 登入
        public static UserModel Login(string No,string Password) {
         
            var UserDataFile = FileLib.rootPath + "employee.json";

            if (FileLib.IsFileExist(UserDataFile))
            {
                List<UserProfileModel> users = new List<UserProfileModel>();
                string FileData = FileLib.ReadFile(UserDataFile);
                users = JsonConvert.DeserializeObject<List<UserProfileModel>>(FileData);
                var user = users.Where(p => p.No == No && p.Password == Password).FirstOrDefault();
                if(user != null)
                {
                    UserModel usermodel = new UserModel()
                    {
                      Name = user.Name,
                      password = user.Password,
                      userNo = user.No
                    };
                    return usermodel;
                }else
                {
                    if (No.IndexOf("guest") > -1)
                    {
                        UserModel usermodel = new UserModel()
                        {
                            Name = "訪客 " + No,
                            password = "",
                            userNo = No
                        };
                        return usermodel;
                    }
                }
            }
            return null;

            
        }
        //取得User資訊
        public UserProfileModel GetUser()
        {
         return (UserProfileModel)HttpContext.Current.Session["user"];
        }
    }
}
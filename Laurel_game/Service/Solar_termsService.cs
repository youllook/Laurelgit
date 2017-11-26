using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Laurel_game.Models;
using Laurel_game.Library;

namespace Laurel_game.Service
{
    public class Solar_termsService
    {
        // 登入
        public static List<Solar_termsModel> GetSolar_terms() {
         
            var DataFile = FileLib.rootPath + "solar_terms.json";

            if (FileLib.IsFileExist(DataFile))
            {
                List<Solar_termsModel> Solar_terms = new List<Solar_termsModel>();

                string FileData = FileLib.ReadFile(DataFile);
                
                Solar_terms = JsonConvert.DeserializeObject<List<Solar_termsModel>>(FileData);
                Random rnd = new Random();
                foreach (var item in Solar_terms)
                {
                    item.randomWeek = rnd.Next(1, 4);
                }

                return Solar_terms;
            }
            return null;
        }
    }
}
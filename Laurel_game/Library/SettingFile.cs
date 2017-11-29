using System;
using Laurel_game.Models;
using Newtonsoft.Json;

namespace Laurel_game.Library
{
    public class SettingFile
    {
        public static CostSettingModel GetCostSet()
        {
            //Get CostData
            CostSettingModel costSet = new CostSettingModel();
            string path = AppDomain.CurrentDomain.BaseDirectory + "Files/cost_setting.json";
            string content = Library.FileLib.ReadFile(path);
            costSet = JsonConvert.DeserializeObject<CostSettingModel>(content);
            return costSet;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laurel_game.Models;
using Laurel_game.Service;
using Newtonsoft.Json;
namespace Laurel_game.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Get CostData
            //CostSettingModel costSet = new CostSettingModel();
            //string path =  AppDomain.CurrentDomain.BaseDirectory + "Files/cost_setting.json";
            //string content = Library.FileLib.ReadFile(path);
            //costSet = JsonConvert.DeserializeObject<CostSettingModel>(content);

            //Get gameData
            //List<GameRecordModel> grecord = new List<GameRecordModel>();
            //string path = AppDomain.CurrentDomain.BaseDirectory + "Record/demo.json";
            //string content = Library.FileLib.ReadFile(path);
            //grecord = JsonConvert.DeserializeObject<List<GameRecordModel>>(content);

            ////WriteToJson txt
            //string newFilePath = AppDomain.CurrentDomain.BaseDirectory + "Record/201709060236_1111.json";
            //Library.FileLib.WriteOverFile(newFilePath, JsonConvert.SerializeObject(grecord));
            GameRecordModel _record = new GameRecordModel()
            {
                RoomId = "123",
                month = 10,
                week = 1,
            };
            return View();
        }


        public ActionResult Record(string FileName)
        {
            if (string.IsNullOrEmpty(FileName))
                FileName = "demo";

            GameRecordJson result = new GameRecordJson();
            string path = AppDomain.CurrentDomain.BaseDirectory + "Record/" + FileName + ".json";
            if (Library.FileLib.IsFileExist(path)) {
                result.GameData_Json = Library.FileLib.ReadFile(path);
            }

            string cost_setting_path = AppDomain.CurrentDomain.BaseDirectory + "Files/cost_setting.json";
            if (Library.FileLib.IsFileExist(cost_setting_path))
            {
                result.CostSet_Json = Library.FileLib.ReadFile(cost_setting_path);
            }
            return View(result);
        }

        public ActionResult Chat()
        {
           
            return View();
        }
        //測試User Group
        public ActionResult ChatGroup()
        {
            return View();
        }
        //測試User Connection
        public ActionResult ChatCount()
        {
            return View();
        }
        //測試User Connection
        public ActionResult ShowAllUser()
        {
            return View();
        }
        //測試User Connection
        public ActionResult Timer()
        {
            return View();
        }
        public ActionResult mobile_wii()
        {
            return View();
        }
        public ActionResult mobile_wii_controller()
        {
            return View();
        }
    }
}
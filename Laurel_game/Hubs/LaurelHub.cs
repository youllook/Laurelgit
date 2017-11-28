using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Laurel_game.Models;
using Laurel_game.Service;
using System.Threading;
using System.Web.Mvc;

namespace Laurel_game.Hubs
{
    public class LaurelHub : Hub
    {
        public static DateTime time = DateTime.Now;
        public static TimerCallback timecallback;
        public static TimerCallback Gametimer;
        public static System.Timers.Timer timersTimer;
        //基本節氣資料
        public static List<Solar_termsModel> Solar_terms = Solar_termsService.GetSolar_terms();

        //目前登入
        public static List<UserModel> UserList = new List<UserModel>();
        //目前遊戲房間
        public static List<RoomModel> RoomList = new List<RoomModel>();
        //目前遊戲資料
        public static List<StorageModel> StorageList = new List<StorageModel>();
        //目前遊戲紀錄
        public static List<GameRecordModel> GameRecordList = new List<GameRecordModel>();
        //當周遊戲紀錄
        public GameRecordModel GameRecord = new GameRecordModel();

        #region 
        //登入 USed!
        public Object Login(UserModel user,string type) {

      
            if (UserList.Where(p => p.userNo == user.userNo && p.password == user.password).ToList().Count() == 0) {

                //檢查房間
                var Room = RoomList.Where(p => p.RoomId == user.RoomID).FirstOrDefault();
                if (Room == null)
                {
                    //建立Room
                    RoomModel room = new RoomModel()
                    {
                        RoomId = user.RoomID,
                        Mode = type,
                        StartTime = DateTime.Now
                    };
                    RoomList.Add(room);
                }


                var usermodel = UserService.Login(user.userNo, user.password);
                if (usermodel != null) {
                    if (UserList.Where(p => p.RoomID == user.RoomID).ToList().Count() <= 4)
                    {
                        //data merge
                        user.ConnectID = Context.ConnectionId;
                        user.Name = usermodel.Name;
                        UserList.Add(user);//加入玩家列表選單
                        Groups.Add(Context.ConnectionId, user.RoomID);//分組廣播
                        //DisplayRoles(user.RoomID);//推播角色資訊至選擇畫面
                        ShowAllUser();//監控用
                        return user;
                    }
                    else
                    {
                        //房間已滿人！
                    }

                }
            }else
            {
                //已經登入
            }
            return false;
        }

        #endregion

        //選擇角色 USed!
        public bool RoleSelect(UserModel user)
        {
            var Room = RoomList.Where(p => p.RoomId == user.RoomID).FirstOrDefault();
            if (Room != null)
            {
                if (Room.Mode == "one")
                {
                    var user_data = UserList.Where(p => p.userNo == user.userNo && p.password == user.password).FirstOrDefault();
                    //若已經有設定電腦則交換
                    var changeCom = UserList.Where(p => p.Role == user.Role).FirstOrDefault();
                    if (changeCom != null)
                    {
                        changeCom.Role = user_data.Role;
                    }
                    //寫入新腳色選擇
                    user_data.Role = user.Role;
                   
                    //塞入電腦OR Check
                    AddComputer(user.RoomID, user.Role);
                    DisplayRoles(user.RoomID);
                    return true;
                }
                else
                {
                    var user_data = UserList.Where(p => p.userNo == user.userNo && p.password == user.password).FirstOrDefault();
                    user_data.Role = user.Role;
                    DisplayRoles(user.RoomID);
                    return true;
                }

            }

            return false;

            //角色已被選擇
            //var RoleCount = UserList.Where(p => p.Role == user.Role && p.RoomID == user.RoomID).ToList().Count();
            //if (RoleCount == 0)
            //{
            //    var Room = RoomList.Where(p => p.RoomId == user.RoomID).FirstOrDefault();
            //    if (Room != null)
            //    {
            //        if (Room.Mode == "one")
            //        {
            //            var user_data = UserList.Where(p => p.userNo == user.userNo && p.password == user.password).FirstOrDefault();
            //            user_data.Role = user.Role;
            //            //塞入電腦
            //            AddComputer(user.RoomID,user.Role);
            //            DisplayRoles(user.RoomID);
            //            return true;
            //        }else
            //        {
            //            var user_data = UserList.Where(p => p.userNo == user.userNo && p.password == user.password).FirstOrDefault();
            //            user_data.Role = user.Role;
            //            DisplayRoles(user.RoomID);
            //            return true;
            //        }

            //    }
            //    else
            //    {
            //        DisplayRoles(user.RoomID);
            //        return false;
            //    }
            //}
            //else
            //{
            //    DisplayRoles(user.RoomID);
            //    return false;
            //}
                
        }
        //顯示玩家角色 USed!
        public void DisplayRoles(string RoomID)
        {
            //推播目前所有相同RoomId群組
            var user_list = UserList.Where(p => p.RoomID == RoomID && p.Role != "").ToList();
            if (user_list.Count() > 0)
            {
                Clients.Group(RoomID).ShowRole(user_list);
            }
        }
        //加入電腦
        public void AddComputer(string RoomID,string role)
        {
            string[] storageNameList = { "Manufacturer", "Distribution", "Wholesale", "Retailer" };
 
            foreach (var name in storageNameList)
            {
                if (name != role)
                {
                    var user = UserList.Where(p => p.Role == name).FirstOrDefault();
                    if (user != null)
                    {
                        user.Role = name;
                        user.Name = name + "(電腦)";
                    }
                    else
                    {
                        UserModel com = new UserModel()
                        {
                            ConnectID = "",
                            Name = name + "(電腦)",
                            password = "",
                            Role = name,
                            RoomID = RoomID,
                            userNo = Guid.NewGuid().ToString(),
                            AI = true
                        };
                        UserList.Add(com);
                    }
                }
                
            }

        }
        //遊戲開始 USed!
        public object Game_Init(string RoomID)
        {

                var room = RoomList.Where(p => p.RoomId == RoomID).FirstOrDefault();
                //建立各倉儲
                //產生初始資料
                ProductItem pItem = new ProductItem()
                {
                    dumpling = new Product { No = "001", Name = "餃類", Cost = 10, Price = 15, Stock = 50, Require = 0, Note = "" },
                    salad = new Product { No = "002", Name = "沙拉", Cost = 10, Price = 15, Stock = 50, Require = 0, Note = "" },
                    riceball = new Product { No = "003", Name = "湯圓", Cost = 10, Price = 15, Stock = 50, Require = 0, Note = "" }
                };
               //產生客戶端需求資料
                Random rnd = new Random();
                ProductItem RetailerData = new ProductItem()
                {
                    //dumpling = new Product { No = "001", Name = "餃類", Cost = 10, Price = 15, Stock = 100, Require = rnd.Next(60,80), Note = "" },
                    //salad = new Product { No = "002", Name = "沙拉", Cost = 10, Price = 15, Stock = 100, Require = rnd.Next(60, 80), Note = "" },
                    //riceball = new Product { No = "003", Name = "湯圓", Cost = 10, Price = 15, Stock = 100, Require = rnd.Next(60, 80), Note = "" }
                    dumpling = new Product { No = "001", Name = "餃類", Cost = 10, Price = 15, Stock = 50, Require = 10, Note = "" },
                    salad = new Product { No = "002", Name = "沙拉", Cost = 10, Price = 15, Stock = 50, Require = 20, Note = "" },
                    riceball = new Product { No = "003", Name = "湯圓", Cost = 10, Price = 15, Stock = 50, Require = 15, Note = "" }
                };

                 StorageModel Storage_ini = new StorageModel()
                 {
                        RoomId = RoomID,
                        Distribution = new StorageData() {Role = "Distribution", ProductItem = pItem },
                        Manufacturer = new StorageData() { Role = "Manufacturer", ProductItem = pItem },
                        Wholesale = new StorageData() { Role = "Wholesale", ProductItem = pItem },
                        Retailer = new StorageData() { Role = "Retailer", ProductItem = RetailerData }
                 };

                 StorageList.Add(Storage_ini);
                //推送至畫面 Room 遊戲房間資料,Storage 各倉儲資料，Solar_terms 節氣資料
            　　return new
                {
                    Room = room,
                    Storage = Storage_ini,
                    Solar_terms = Solar_terms
                };


        }
 
        //遊戲資料更新 USed!
        public bool Game_Update(UserModel user,StorageData Storage_data,int WeekNum)
        {
            var StorageItgem = StorageList.Where(p => p.RoomId == user.RoomID).FirstOrDefault();
            
            if (StorageItgem != null)
            {
                switch (Storage_data.Role)
                {
                    case "Manufacturer":
                        StorageItgem.Manufacturer = Storage_data;
                        StorageItgem.Manufacturer.IsUpdate = true;
                        break;
                    case "Distribution":
                        StorageItgem.Distribution = Storage_data;
                        StorageItgem.Distribution.IsUpdate = true;
                        break;
                    case "Wholesale":
                        StorageItgem.Wholesale = Storage_data;
                        StorageItgem.Wholesale.IsUpdate = true;
                        break;
                    case "Retailer":
                        StorageItgem.Retailer = Storage_data;
                        StorageItgem.Retailer.IsUpdate = true;
                        break;
                }
            }
            else
            {
                return false;
            }

            //等待所有人資料更新
            if (StorageItgem.Manufacturer.IsUpdate &&
                StorageItgem.Distribution.IsUpdate &&
                StorageItgem.Wholesale.IsUpdate &&
                StorageItgem.Retailer.IsUpdate
                )
            {
                
                //改回待更新狀態
                StorageItgem.Manufacturer.IsUpdate = false;
                StorageItgem.Distribution.IsUpdate = false;
                StorageItgem.Wholesale.IsUpdate = false;
                StorageItgem.Retailer.IsUpdate = false;

                //修改　Retailer　的資料
                RetailerItemHandle(user.RoomID, WeekNum);
                //更新所有人資料
                //Clients.Group.addNewMessageToPage(JsonConvert.SerializeObject(UserList));
                Clients.All.UpdateClienData(StorageItgem);
                //結算 累計盈餘與累計罰金
                GameSettlement SettlementData = GetSettlement(user.RoomID);
                Clients.All.UpdateClienSettlement(SettlementData);
            }

            return true;
        }

        #region 各倉儲業務邏輯
        //各倉儲業務
        //通路C
        public void RetailerItemHandle(string RoomID, int WeekNum) {


            //換算出月/週數
            int thisMonth = (WeekNum / 4) + 3;
            if (thisMonth > 12)
                thisMonth = thisMonth - 12;

            int thisWeek = (WeekNum % 4) + 1;


            #region 加入遊戲紀錄設定
            GameRecord.RoomId = RoomID;
            GameRecord.month = thisMonth;
            GameRecord.week = thisWeek;
            #endregion


            var GameData = StorageList.Where(p => p.RoomId == RoomID).FirstOrDefault();

            if (GameData != null) {

                var RetailerItem = GameData.Retailer.ProductItem;
                var WholesaleItem = GameData.Wholesale.ProductItem;
                var RetailerRecord = GameRecord.Retailer; //遊戲紀錄用
                //通路C執行流程:
                //請營業所將貨品入庫
                WholesaleItemHandle(RoomID);

                

                string[] pArray = { "dumpling", "riceball", "salad" };

                foreach (var Pname in pArray)
                {
                    //需求扣減庫存
                    ((Product)RetailerItem[Pname]).Stock = ((Product)RetailerItem[Pname]).Stock - ((Product)RetailerItem[Pname]).Require;
                    //[ GameRecord ] Retailer 通路C之出貨數量紀錄 = 需求
                    ((RecordDataItem)RetailerRecord[Pname]).income = ((Product)RetailerItem[Pname]).Require;
                }
                // RetailerItem.dumpling.Stock = RetailerItem.dumpling.Stock - RetailerItem.dumpling.Require;
                //RetailerItem.riceball.Stock = RetailerItem.riceball.Stock - RetailerItem.riceball.Require;
                //RetailerItem.salad.Stock = RetailerItem.salad.Stock - RetailerItem.salad.Require;

                //加入需求
                Random rand = new Random();
                //檢查是否有特殊事件
                var SpecEvent = Solar_terms.Where(p => p.month == thisMonth && p.randomWeek == thisWeek).FirstOrDefault();
                if (SpecEvent != null)
                {
                    switch (SpecEvent.product)
                    {
                        //測試用邏輯
                        //case "all":
                        //    RetailerItem.dumpling.Require = 20;//rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                        //    RetailerItem.salad.Require = 25;//rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                        //    RetailerItem.riceball.Require = 30; //rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                        //    break;
                        //case "salad":
                        //    RetailerItem.dumpling.Require = 20;//rand.Next(60, 80);
                        //    RetailerItem.salad.Require = 25;//rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                        //    RetailerItem.riceball.Require = 30;// rand.Next(60, 80);
                        //    break;
                        //case "dumpling":
                        //    RetailerItem.dumpling.Require = 20;//rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                        //    RetailerItem.salad.Require = 25;//rand.Next(60, 80);
                        //    RetailerItem.riceball.Require = 30; ;//rand.Next(60, 80);
                        //    break;
                        //case "riceball":
                        //    RetailerItem.dumpling.Require += 20;//rand.Next(60, 80);
                        //    RetailerItem.salad.Require += 25;//rand.Next(60, 80);
                        //    RetailerItem.riceball.Require += 30; //rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                        //    break;

                        case "all":
                            RetailerItem.dumpling.Require = rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                            RetailerItem.salad.Require = rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                            RetailerItem.riceball.Require = rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                            break;
                        case "salad":
                            RetailerItem.dumpling.Require = rand.Next(60, 80);
                            RetailerItem.salad.Require = rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                            RetailerItem.riceball.Require = rand.Next(60, 80);
                            break;
                        case "dumpling":
                            RetailerItem.dumpling.Require = rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                            RetailerItem.salad.Require = rand.Next(60, 80);
                            RetailerItem.riceball.Require = rand.Next(60, 80);
                            break;
                        case "riceball":
                            RetailerItem.dumpling.Require = rand.Next(60, 80);
                            RetailerItem.salad.Require = rand.Next(60, 80);
                            RetailerItem.riceball.Require = rand.Next(SpecEvent.event_min, SpecEvent.event_max);
                            break;
                    }

                }
                else
                {
                    //測試用邏輯
                    //RetailerItem.dumpling.Require = 20;
                    //RetailerItem.salad.Require = 25;
                    //RetailerItem.riceball.Require = 30;
                    RetailerItem.dumpling.Require = rand.Next(60, 80);
                    RetailerItem.salad.Require = rand.Next(60, 80);
                    RetailerItem.riceball.Require = rand.Next(60, 80);
                }

                #region[ GameRecord ] Retailer 邏輯
                foreach (var Pname in pArray)
                {
                    //[ GameRecord ] Retailer Require
                    ((RecordDataItem)RetailerRecord[Pname]).require = ((Product)RetailerItem[Pname]).Require;
                    //[ GameRecord ] Retailer Lack 庫存短缺
                    ((RecordDataItem)RetailerRecord[Pname]).lack = (((Product)RetailerItem[Pname]).Stock < 0) ? Math.Abs(((Product)RetailerItem[Pname]).Stock) : 0;
                }
                //Retailer 是否超出最大庫存
                Storage_Overflow("Retailer", RoomID);

                #endregion




            }

            // 遊戲紀錄儲存
            GameRecordList.Add(GameRecord);
            DataSave((RoomID));

        }
        //營業所
        public void WholesaleItemHandle(string RoomID)
        {

            var GameData = StorageList.Where(p => p.RoomId == RoomID).FirstOrDefault();

            if (GameData != null)
            {
                var RetailerItem = GameData.Retailer.ProductItem;
                var WholesaleItem = GameData.Wholesale.ProductItem;
                //遊戲紀錄用
                var WholesaleRecord = GameRecord.Wholesale;
                var RetailerRecord = GameRecord.Retailer;
                
                //營業所執行流程:
                //請發貨中心將貨品入庫
                DistributionItemHandle(RoomID);

                string[] pArray = { "dumpling", "riceball", "salad"};

                foreach (var Pname in pArray) {

                    //營業所庫存扣減 營業所 出貨量
                    ((Product)WholesaleItem[Pname]).Stock = ((Product)WholesaleItem[Pname]).Stock - ((Product)WholesaleItem[Pname]).Shipping;
                    //[ GameRecord ] Wholesale 營業所之出貨數量紀錄 = 營業所欲配送量
                    ((RecordDataItem)WholesaleRecord[Pname]).income = ((Product)WholesaleItem[Pname]).Shipping;

                    //將需求貨品增加至通路C
                    ((Product)RetailerItem[Pname]).Stock += ((Product)WholesaleItem[Pname]).Shipping;
                    //[ GameRecord ] Retailer 通路C之進貨數量紀錄
                    ((RecordDataItem)RetailerRecord[Pname]).cost = ((Product)WholesaleItem[Pname]).Shipping;

                    //將配送數量紀錄至配送欄位
                    ((Product)WholesaleItem[Pname]).Require = ((Product)RetailerItem[Pname]).Distribute;
                    //配送完畢，營業所 出貨量歸零
                    ((Product)WholesaleItem[Pname]).Shipping = 0;
                    //配送完畢，通路配送量歸零
                    ((Product)RetailerItem[Pname]).Distribute = 0;

                    //[ GameRecord ] Wholesale Require
                    ((RecordDataItem)WholesaleRecord[Pname]).require = ((Product)WholesaleItem[Pname]).Require;
                    //[ GameRecord ] Wholesale Lack
                    var nowStock = ((Product)WholesaleItem[Pname]).Stock;
                    ((RecordDataItem)WholesaleRecord[Pname]).lack = (nowStock < 0) ? Math.Abs(nowStock) : 0;
                    ////營業所庫存扣減通路C欲配送量
                    //((Product)WholesaleItem[Pname]).Stock = ((Product)WholesaleItem[Pname]).Stock - ((Product)RetailerItem[Pname]).Distribute;
                    ////將需求貨品增加至通路C
                    //((Product)RetailerItem[Pname]).Stock += ((Product)RetailerItem[Pname]).Distribute;
                    ////將配送數量紀錄至配送欄位
                    //((Product)WholesaleItem[Pname]).Require = ((Product)RetailerItem[Pname]).Distribute;
                    ////配送完畢，通路配送量歸零
                    //((Product)RetailerItem[Pname]).Distribute = 0;
                }
                #region[ GameRecord ] Wholesale 邏輯
                //Wholesale 是否超出最大庫存
                Storage_Overflow("Wholesale", RoomID);
                #endregion

            }

        }
        //發貨中心
        public void DistributionItemHandle(string RoomID)
        {
            var GameData = StorageList.Where(p => p.RoomId == RoomID).FirstOrDefault();

            if (GameData != null)
            {
                var WholesaleItem = GameData.Wholesale.ProductItem;
                var DistributionItem = GameData.Distribution.ProductItem;
                //遊戲紀錄用
                var DistributionRecord = GameRecord.Distribution;
                var WholesaleRecord = GameRecord.Wholesale;

                //請工廠先將貨品入庫
                ManufacturerItemHandle(RoomID);

                string[] pArray = { "dumpling", "riceball", "salad" };

                foreach (var Pname in pArray)
                {
                    //發貨中心庫存扣減營業所欲配送量
                    ((Product)DistributionItem[Pname]).Stock = ((Product)DistributionItem[Pname]).Stock - ((Product)DistributionItem[Pname]).Shipping;
                    //[ GameRecord ] Distribution 發貨中心之出貨數量紀錄 = 營業所欲配送量
                    ((RecordDataItem)DistributionRecord[Pname]).income = ((Product)DistributionItem[Pname]).Shipping;

                    //扣掉之準備配送的量增加至營業所
                    ((Product)WholesaleItem[Pname]).Stock += ((Product)DistributionItem[Pname]).Shipping;
                    //[ GameRecord ] Wholesale 營業所之進貨數量紀錄
                    ((RecordDataItem)WholesaleRecord[Pname]).cost = ((Product)DistributionItem[Pname]).Shipping;

                    //將配送數量紀錄至配送欄位
                    ((Product)DistributionItem[Pname]).Require = ((Product)WholesaleItem[Pname]).Distribute;
                    //配送完畢，發貨中心 出貨量歸零
                    ((Product)DistributionItem[Pname]).Shipping = 0;
                    //配送完畢，發貨中心配送需求歸零
                    ((Product)WholesaleItem[Pname]).Distribute = 0;

                    //[ GameRecord ] Distribution Require
                    ((RecordDataItem)DistributionRecord[Pname]).require = ((Product)DistributionItem[Pname]).Require;
                    //[ GameRecord ] Distribution Lack
                    var nowStock = ((Product)DistributionItem[Pname]).Stock;
                    ((RecordDataItem)DistributionRecord[Pname]).lack = (nowStock < 0) ? Math.Abs(nowStock) : 0;

                    ////發貨中心庫存扣減營業所欲配送量
                    //((Product)DistributionItem[Pname]).Stock = ((Product)DistributionItem[Pname]).Stock - ((Product)WholesaleItem[Pname]).Distribute;
                    ////扣掉之準備配送的量增加至營業所
                    //((Product)WholesaleItem[Pname]).Stock += ((Product)WholesaleItem[Pname]).Distribute;
                    ////將配送數量紀錄至配送欄位
                    //((Product)DistributionItem[Pname]).Require = ((Product)WholesaleItem[Pname]).Distribute;
                    ////配送完畢，配送需求歸零
                    //((Product)WholesaleItem[Pname]).Distribute = 0;

                }
                #region[ GameRecord ] Distribution 邏輯
                //Distribution 是否超出最大庫存
                Storage_Overflow("Distribution", RoomID);
                #endregion
            }

        }
        //工廠
        public void ManufacturerItemHandle(string RoomID)
        {

            var GameData = StorageList.Where(p => p.RoomId == RoomID).FirstOrDefault();

            if (GameData != null)
            {
                var ManufacturerItem = GameData.Manufacturer.ProductItem;
                var DistributionItem = GameData.Distribution.ProductItem;
                //遊戲紀錄用
                var ManufacturerRecord = GameRecord.Manufacturer;
                var DistributionRecord = GameRecord.Distribution;
                string[] pArray = { "dumpling", "riceball", "salad" };

                foreach (var Pname in pArray)
                {

                    //將生產中之數量增加至發貨中心
                    ((Product)DistributionItem[Pname]).Stock += ((Product)ManufacturerItem[Pname]).Stock;
                    //[ GameRecord ] Distribution 發貨中心之進貨數量紀錄 = 工廠的庫存
                    ((RecordDataItem)DistributionRecord[Pname]).cost = ((Product)ManufacturerItem[Pname]).Stock;
                    //[ GameRecord ] Manufacturer 工廠之出貨數量紀錄
                    ((RecordDataItem)ManufacturerRecord[Pname]).income = ((Product)ManufacturerItem[Pname]).Stock;
                    //已出貨工廠庫存歸零
                    ((Product)ManufacturerItem[Pname]).Stock = 0;
                    //將欲生產值紀錄至生產數量中
                    ((Product)ManufacturerItem[Pname]).Stock = ((Product)ManufacturerItem[Pname]).Distribute;
                    //[ GameRecord ] Manufacturer 工廠之進貨數量紀錄
                    ((RecordDataItem)ManufacturerRecord[Pname]).cost = ((Product)ManufacturerItem[Pname]).Distribute;
                    //新增需求(發貨中心之配送)
                    ((Product)ManufacturerItem[Pname]).Require = ((Product)DistributionItem[Pname]).Distribute;
                    //發貨中心之配送歸零
                    ((Product)DistributionItem[Pname]).Distribute = 0 ;
                    //生產數量歸零
                    ((Product)ManufacturerItem[Pname]).Distribute = 0;

                    //[ GameRecord ] Manufacturer 工廠之進貨數量紀錄
                    ((RecordDataItem)ManufacturerRecord[Pname]).cost = ((Product)ManufacturerItem[Pname]).Stock;
                    //[ GameRecord ] Manufacturer 工廠需求數量紀錄
                    ((RecordDataItem)ManufacturerRecord[Pname]).require = ((Product)ManufacturerItem[Pname]).Require;
                }



            }

        }
        #endregion

        #region 遊戲紀錄用
        //超出庫存計算
        public void Storage_Overflow(string StorageName,string RoomId)
        {
            //總庫存設定
            int MaxCount = 0;
            switch (StorageName)
            {
                case "Retailer":
                    MaxCount = 180;
                    break;
                case "Wholesale":
                    MaxCount = 250;
                    break;
                case "Distribution":
                    MaxCount = 400;
                    break;
                // 工廠(Manufacturer)生產沒有爆倉的問題
                //case "Manufacturer":
                //    MaxCount = 500;
                //    break;
                default:
                    break;
            };

            var GameData = StorageList.Where(p => p.RoomId == RoomId).FirstOrDefault();
            if (GameData != null)
            {
                StorageData StorageItem = (StorageData)GameData[StorageName];
                //檢查單位是否超出庫存!若有直接扣最大項之庫存至符合倉庫數量
                int[] ItemStock = { StorageItem.ProductItem.dumpling.Stock,
                                    StorageItem.ProductItem.salad.Stock,
                                    StorageItem.ProductItem.riceball.Stock};

                int _index = 0;
                if (ItemStock.Sum() > MaxCount)
                {
                    while (ItemStock.Sum() > MaxCount)
                    {
                        int maxValue = ItemStock.Max();
                        int maxIndex = ItemStock.ToList().IndexOf(maxValue);
                        ItemStock[maxIndex] = ItemStock[maxIndex] - 1;
                    }
                    //加入遊戲紀錄內
                   ((RecordData)GameRecord[StorageName]).dumpling.overrun =  StorageItem.ProductItem.dumpling.Stock - ItemStock[0];
                   ((RecordData)GameRecord[StorageName]).salad.overrun = StorageItem.ProductItem.salad.Stock - ItemStock[1];
                   ((RecordData)GameRecord[StorageName]).riceball.overrun = StorageItem.ProductItem.riceball.Stock - ItemStock[2];
                    //複寫原有庫存
                    StorageItem.ProductItem.dumpling.Stock = ItemStock[0];
                    StorageItem.ProductItem.salad.Stock = ItemStock[1];
                    StorageItem.ProductItem.riceball.Stock = ItemStock[2];
                }

            }

      

        }
        //存檔
        public void DataSave(string Roomid)
        {
            var roomData = RoomList.Where(p => p.RoomId == Roomid).FirstOrDefault();
            if (roomData != null)
            {
                string filename = roomData.key + "_" + roomData.RoomId;
                var thisGameData = GameRecordList.Where(p => p.RoomId == Roomid).ToList();
                string Content = Newtonsoft.Json.JsonConvert.SerializeObject(thisGameData);
                string filepath = Library.FileLib.rootPath + "save/" + filename + ".txt";
                Library.FileLib.WriteOverFile(filepath, Content);
            }
        }
        #endregion

        // 各單位累計盈餘與罰金計算
        public GameSettlement GetSettlement(string RoomID)
        {
            //取得成本資料
            CostSettingModel costSet = GetCostSet();

            // 盈餘 => 出貨 - 成本 (出貨數量*該項目賣出金額) - (進貨數量*該項目成本金額)
            // 罰金 => (超出庫存數量 + 短缺數量)*該項目罰金金額　
            GameSettlement data = new GameSettlement();
            
            foreach (var item in GameRecordList.Where(p => p.RoomId == RoomID).ToList())
            {
                string[] storageNameList = { "Manufacturer", "Distribution", "Wholesale" , "Retailer" };
                foreach (var storageName in storageNameList)
                {
                    string[] pArray = { "dumpling", "riceball", "salad" };
                    foreach (var pName in pArray)
                    {
                        ((SettlementItem)data[storageName]).income +=
                        (((RecordDataItem)((RecordData)item[storageName])[pName]).income * (int)((StorageSetting)costSet[storageName]).income[pName]) -
                        (((RecordDataItem)((RecordData)item[storageName])[pName]).cost * (int)((StorageSetting)costSet[storageName]).cost[pName]);

                        ((SettlementItem)data[storageName]).fine +=
                        (
                        ((RecordDataItem)((RecordData)item[storageName])[pName]).overrun +
                        ((RecordDataItem)((RecordData)item[storageName])[pName]).lack
                        ) * (int)((StorageSetting)costSet[storageName]).fine[pName];
                    }
                }

            }
            return data;
        }
        public static CostSettingModel GetCostSet()
        {
            //Get CostData
            CostSettingModel costSet = new CostSettingModel();
            string path = AppDomain.CurrentDomain.BaseDirectory + "Files/cost_setting.json";
            string content = Library.FileLib.ReadFile(path);
            costSet = JsonConvert.DeserializeObject<CostSettingModel>(content);
            return costSet;
        }
  

        #region UserConnection

        //Show all User
        public void ShowAllUser() {
            var users = JsonConvert.SerializeObject(UserList);
            Clients.All.showUserList(users);
        }
        //Sends the update user count to the listening view.
        public void Send(int count)
        {
            // Call the addNewMessageToPage method to update clients.
            var context = GlobalHost.ConnectionManager.GetHubContext<LaurelHub>();
            context.Clients.All.updateUsersOnlineCount(count);
        }
        //The OnConnected event.
        public override System.Threading.Tasks.Task OnConnected()
        {
            string clientId = GetClientId();
            CreateTimer();
            return base.OnConnected();
        }
        //The OnReconnected event.
        public override System.Threading.Tasks.Task OnReconnected()
        {
            string clientId = GetClientId();
            return base.OnReconnected();
        }
        //OnDisconnected USed!
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            string clientId = GetClientId();
            //斷線刪除玩家
            var Delete_User = UserList.Where(p => p.ConnectID == clientId).FirstOrDefault();
            if (Delete_User != null) {
                UserList.Remove(Delete_User);
                //若在選角的時候　斷線刪除該角色
                DisplayRoles(Delete_User.RoomID);

                var RoomData = RoomList.Where(p => p.RoomId == Delete_User.RoomID).FirstOrDefault();
                //單人模式 - 檢查是否有遊戲紀錄，有的話全部刪除
                if (RoomData.Mode == "one")
                {
                    //刪除倉儲
                    var thisStoreges = StorageList.Where(p => p.RoomId == RoomData.RoomId).ToList();
                    foreach (var st in thisStoreges)
                    {
                        StorageList.Remove(st);
                    }
                    //刪除遊戲紀錄
                    var thisRecord = GameRecordList.Where(p => p.RoomId == RoomData.RoomId).ToList();
                    foreach (var rc in thisRecord)
                    {
                        GameRecordList.Remove(rc);
                    }
                    //刪除電腦玩家
                    var thisUsers = UserList.Where(p => p.RoomID == RoomData.RoomId).ToList();
                    foreach (var us in thisUsers)
                    {
                        UserList.Remove(us);
                    }
                    //刪除Room
                    RoomList.Remove(RoomData);
                }
                else
                {
                    //待補...
                }
            }


            // Send the current count of users
            //Clients.All.addNewMessageToPage(JsonConvert.SerializeObject(UserList));
            return base.OnDisconnected(true);
        }
        //GetClientId
        private string GetClientId()
        {
            string clientId = "";
            if (Context.QueryString["clientId"] != null)
            {
                // clientId passed from application 
                clientId = this.Context.QueryString["clientId"];
            }

            if (string.IsNullOrEmpty(clientId.Trim()))
            {
                clientId = Context.ConnectionId;
            }

            return clientId;
        }
        #endregion

        #region UserGROUP
        public void Group(String GroupId)
        {
            Groups.Add(Context.ConnectionId, GroupId);
            Clients.Group(GroupId).addMessage(Context.ConnectionId + ":Welcome");
        }
        public void Groupsend(String GroupId, String Message)
        {
            Clients.Group(GroupId).addMessage(Message);
        }
        public void SendOne(String ClientId, String Message)
        {
            Clients.Client(ClientId).addMessage(Message);
        }
        #endregion


        //Timer
        public void TimerStart()
        {
            if(timersTimer == null)
            {
              timersTimer = new System.Timers.Timer();
            }
            var context = GlobalHost.ConnectionManager.GetHubContext<LaurelHub>();
        }
        private void CreateTimer() {

                if (timecallback == null)
                {
                    timecallback = new TimerCallback(TimerTask);
                    //for gameloop test
                    Timer timer = new Timer(timecallback, null, 1000, 1000);
                }
        }
        private static void TimerTask(object obj)
        {
            //Console.WriteLine(count.ToString());
            var context = GlobalHost.ConnectionManager.GetHubContext<LaurelHub>();
            context.Clients.All.GetTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        }
        #region ChatDemo 聊天室測試用
        public void Hello()
        {
            Clients.All.hello();
        }
        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }
        #endregion
        #region mobile_wii測試用
        public void MobileCoordinate(float x, float y)
        {
            Clients.All.coordinateCallBack(x, y);
        }

        #endregion
    }
}
using System;
using System.Reflection;

namespace Laurel_game.Models
{
    public class GameRecordJson
    {
        public string GameData_Json { get; set; }
        public string CostSet_Json { get; set; }
    }
    public class GameRecordModel
    {
        //get set propertyName by string
        public object this[string propertyName]
        {
            get
            {
                Type thisType = typeof(GameRecordModel);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                return thisPropInfo.GetValue(this, null);
            }
            set
            {
                Type thisType = typeof(GameRecordModel);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                thisPropInfo.SetValue(this, value, null);
            }
        }
        public string RoomId { get; set; }
        public int month { get; set; }
        public int week { get; set; }
        public RecordData Manufacturer { get; set; }
        public RecordData Distribution { get; set; }
        public RecordData Wholesale { get; set; }
        public RecordData Retailer { get; set; }
        public GameRecordModel()
        {
            this.RoomId = "0";
            this.month = 0;
            this.week = 0;
            Manufacturer = new RecordData();
            Distribution = new RecordData();
            Wholesale = new RecordData();
            Retailer = new RecordData();
        }
    }

    public class RecordData
    {
        //get set propertyName by string
        public object this[string propertyName]
        {
            get
            {
                Type thisType = typeof(RecordData);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                return thisPropInfo.GetValue(this, null);
            }
            set
            {
                Type thisType = typeof(RecordData);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                thisPropInfo.SetValue(this, value, null);
            }
        }
        public RecordDataItem dumpling { get; set; }     
        public RecordDataItem riceball { get; set; }     
        public RecordDataItem salad { get; set; }
        public RecordData()
        {
            this.dumpling = new RecordDataItem();
            this.riceball = new RecordDataItem();
            this.salad = new RecordDataItem();
        }
    }
    public class RecordDataItem
    {
        public int require { get; set; }      //需求數量
        public int income { get; set; }       //出貨(收入)數量
        public int cost { get; set; }         //進貨數量
        public int overrun { get; set; }      //超出庫存數量
        public int lack { get; set; }         //庫存短缺數量

        public RecordDataItem()
        {
            this.require = 0;
            this.income = 0;
            this.cost = 0;
            this.overrun = 0;
            this.lack = 0;
        }
    }
    public class GameSettlement
    {
        public object this[string propertyName]
        {
            get
            {
                Type thisType = typeof(GameSettlement);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                return thisPropInfo.GetValue(this, null);
            }
            set
            {
                Type thisType = typeof(GameSettlement);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                thisPropInfo.SetValue(this, value, null);
            }
        }
        public SettlementItem Manufacturer { get; set; }
        public SettlementItem Distribution { get; set; }
        public SettlementItem Wholesale { get; set; }
        public SettlementItem Retailer { get; set; }
        public GameSettlement()
        {

            Manufacturer = new SettlementItem();
            Distribution = new SettlementItem();
            Wholesale = new SettlementItem();
            Retailer = new SettlementItem();
        }

    }
    public class SettlementItem
    {
        public int income { get; set; }
        public int fine { get; set; }
        public SettlementItem()
        {
            this.income = 0;
            this.fine = 0;
        }
    }
}
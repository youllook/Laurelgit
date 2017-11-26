using System;
using System.Reflection;

namespace Laurel_game.Models
{
    public class StorageModel
    {
        //get set propertyName by string
        public object this[string propertyName]
        {
            get
            {
                Type thisType = typeof(StorageModel);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                return thisPropInfo.GetValue(this, null);
            }
            set
            {
                Type thisType = typeof(StorageModel);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                thisPropInfo.SetValue(this, value, null);
            }
        }
        public string RoomId { get; set; }        //遊戲房間編號
        public StorageData Manufacturer { get; set; }//工廠
        public StorageData Distribution { get; set; }//發貨中心
        public StorageData Wholesale { get; set; }   //營業所
        public StorageData Retailer { get; set; }    //C 通路
        public StorageModel()
        {
            this.RoomId = "";
        }
    }
    public class StorageData
    {
        //get set propertyName by string
        public object this[string propertyName]
        {
            get
            {
                Type thisType = typeof(StorageData);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                return thisPropInfo.GetValue(this, null);
            }
            set
            {
                Type thisType = typeof(StorageData);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                thisPropInfo.SetValue(this, value, null);
            }
        }
        public string Role { get; set; }
        public bool IsUpdate { get; set; }
        public int maxStock { get; set; }         //最大庫存數
        public ProductItem ProductItem { get; set; }
        public StorageData()
        {
            this.Role = "";
            this.IsUpdate = false;
        }
    }
    public class ProductItem
    {
        public object this[string propertyName]
        {
            get
            {
                Type thisType = typeof(ProductItem);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                return thisPropInfo.GetValue(this, null);
            }
            set
            {
                Type thisType = typeof(ProductItem);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                thisPropInfo.SetValue(this, value, null);
            }
        }
        public Product dumpling { get; set; }          //餃類
        public Product salad { get; set; }             //沙拉
        public Product riceball { get; set; }          //湯圓

    }
    public class Product
    {
        public object this[string propertyName]
        {
            get
            {
                Type thisType = typeof(Product);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                return thisPropInfo.GetValue(this, null);
            }
            set
            {
                Type thisType = typeof(Product);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                thisPropInfo.SetValue(this, value, null);
            }
        }
        public string No { get; set; }          //商品編號
        public string Name { get; set; }        //品名
        public int Price { get; set; }          //售價
        public int Cost { get; set; }           //成本
        public int Require { get; set; }        //需求數
        public int Stock { get; set; }          //庫存數
        public int Distribute { get; set; }     //配送數
        public int Shipping { get; set; }       //出貨數
        public string Note { get; set; }        //說明
        public Product() {
            Shipping = 0;
        }
    }
}
using System;
using System.Reflection;

namespace Laurel_game.Models
{
    public class CostSettingModel
    {
        //get set propertyName by string
        public object this[string propertyName]
        {
            get
            {
                Type thisType = typeof(CostSettingModel);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                return thisPropInfo.GetValue(this, null);
            }
            set
            {
                Type thisType = typeof(CostSettingModel);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                thisPropInfo.SetValue(this, value, null);
            }
        }
        public StorageSetting Manufacturer { get; set; }
        public StorageSetting Distribution { get; set; }
        public StorageSetting Wholesale { get; set; }
        public StorageSetting Retailer { get; set; }
    }

    public class StorageSetting
    {
        //get set propertyName by string
        public object this[string propertyName]
        {
            get
            {
                Type thisType = typeof(StorageSetting);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                return thisPropInfo.GetValue(this, null);
            }
            set
            {
                Type thisType = typeof(StorageSetting);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                thisPropInfo.SetValue(this, value, null);
            }
        }
        public SettingItem income { get; set; }  //收入
        public SettingItem cost { get; set; }    //成本
        public SettingItem fine { get; set; }    //罰金
    }
    public class SettingItem
    {
        //get set propertyName by string
        public object this[string propertyName]
        {
            get
            {
                Type thisType = typeof(SettingItem);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                return thisPropInfo.GetValue(this, null);
            }
            set
            {
                Type thisType = typeof(SettingItem);
                PropertyInfo thisPropInfo = thisType.GetProperty(propertyName);
                thisPropInfo.SetValue(this, value, null);
            }
        }
        public int dumpling { get; set; }   //餃類
        public int riceball { get; set; }   //湯圓
        public int salad { get; set; }      //沙拉
        public SettingItem()
        {
            this.dumpling = 0;
            this.riceball = 0;
            this.salad = 0;
        }
    }
}
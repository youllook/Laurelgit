using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Laurel_game.Library
{
    public class FileLib
    {
        public static string recordPath = AppDomain.CurrentDomain.BaseDirectory + "Record/";
        public static string rootPath = AppDomain.CurrentDomain.BaseDirectory + "Files/";
        //檔案是否存在
        public static bool IsFileExist(string FilePath)
        {
            if (File.Exists(FilePath))
                return true;
            else
                return false;
        }
        //建立檔案
        public static void CreateFile(string FilePath)
        {
            string FilaName = Path.GetFileName(FilePath);
            string Folder = FilePath.Replace(FilaName, "");
            if (IsFileExist(Folder))
            {
                File.CreateText(FilePath).Close();
            }
            else
            {
                Directory.CreateDirectory(Folder);
                File.CreateText(FilePath).Close();
            }

        }
        //寫入檔案 (含編碼)
        public static void WriteInFile(string FilePath, List<string> ContentList, System.Text.Encoding Encode)
        {
            //如果沒有就建立一個
            if (!IsFileExist(FilePath))
                CreateFile(FilePath);

            List<string> Content = new List<string>();
            //讀取
            StreamReader sr = new StreamReader(FilePath, Encode);
            while (!sr.EndOfStream)
            {
                Content.Add(sr.ReadLine());
            }
            sr.Close();
            //加入
            Content.AddRange(ContentList);

            WriteOverFile(FilePath, Content, Encode);
        }
        //重寫檔案
        public static void WriteOverFile(string FilePath, string Content)
        {
            //如果沒有就建立一個
            if (!IsFileExist(FilePath))
                CreateFile(FilePath);

            StreamWriter file = new System.IO.StreamWriter(FilePath);
            file.WriteLine(Content);

            file.Close();
        }
        //重寫檔案 (含編碼)
        public static void WriteOverFile(string FilePath, List<string> ContentList, System.Text.Encoding Encode)
        {
            //如果沒有就建立一個
            if (!IsFileExist(FilePath))
                CreateFile(FilePath);

            StreamWriter file = new System.IO.StreamWriter(FilePath, false, Encode);
            foreach (var row in ContentList)
            {
                file.WriteLine(row);
            }

            file.Close();
        }
        //讀取檔案
        public static List<string> ReadFileToList(string FilePath)
        {
            List<string> list = new List<string>();
            //記得指定編碼不然會亂碼
            using (StreamReader reader = new StreamReader(FilePath, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line); // Add to list.
                }
                reader.Close();
                reader.Dispose();
            }

            return list;
        }

        public static string ReadFile(string FilePath)
        {
            //記得指定編碼不然會亂碼
            string txtcontent = "";
            using (StreamReader reader = new StreamReader(FilePath, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    txtcontent += line; // Add to list.
                }
                reader.Close();
                reader.Dispose();
            }

            return txtcontent;
        }
        //讀取檔案 (含編碼)
        public static List<string> ReadFile(string FilePath, System.Text.Encoding Encode)
        {
            List<string> list = new List<string>();
            //記得指定編碼不然會亂碼
            using (StreamReader reader = new StreamReader(FilePath, Encode))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line); // Add to list.
                }
                reader.Close();
                reader.Dispose();
            }

            return list;
        }
        //刪除檔案
        public static void DeleteFile(string FilePath)
        {
            File.Delete(FilePath);
        }
    }
}
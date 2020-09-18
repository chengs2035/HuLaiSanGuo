using JiangDongXiaoQiaoTools.Bean;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiangDongXiaoQiaoTools.DBHelp
{
    class OperDB
    {
        System.Data.SQLite.SQLiteConnection sqlite = null;

        public void connDB(string dbpath)
        {

            sqlite = new System.Data.SQLite.SQLiteConnection(string.Format("Data Source={0};UTF8Encoding=True;Version=3", dbpath));
            try
            {
                sqlite.Open();
            }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                throw new System.Data.SQLite.SQLiteException("数据库连接异常：请关闭小乔后重试！异常具体信息：" + ex.Message);
            }
            finally
            {
                //conn success.
                sqlite.Close();
            }
        }
        /// <summary>
        /// 获取指定某一行的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String getDBItemValue(string key)
        {
            List<ItemTable> list = getDBItemTable();
            for (int i = 0, l = list.Count; i < l; i++)
            {
                ItemTable table = list[i];
                if (table.key != null && table.key.Contains(key))
                {
                    return table.valueJson;

                }
            }
            return string.Empty;
        }
        public int updateItemTable(string key,string value) {
            SQLiteCommand sqliteCmd = sqlite.CreateCommand();
            sqliteCmd.CommandText = "UPDATE ItemTable SET VALUE  =:value WHERE KEY=:key";
            SQLiteParameter parameter = new SQLiteParameter();
            
            try
            {
                sqlite.Open();
                byte[] bytes = System.Text.Encoding.Unicode.GetBytes(value);

                sqliteCmd.Parameters.Add("value", DbType.Binary).Value = bytes;
                sqliteCmd.Parameters.Add("key", DbType.String).Value = key;

                return sqliteCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception("数据读取报错：" + ex.Message);
            }
            finally
            {
                if (sqlite != null)
                {
                    sqliteCmd.Dispose();
                    sqlite.Close();
                }
            }
           

        }
        /// <summary>
        /// 获取小乔数据库表的所有数据
        /// </summary>
        /// <returns></returns>
        public List<ItemTable> getDBItemTable()
        {
            SQLiteCommand sqliteCmd = sqlite.CreateCommand();
            sqliteCmd.CommandText = "select key,value from ItemTable";
            try
            {
                sqlite.Open();
                // SQLiteDataReader dataReader = sqliteCmd.ExecuteReader();

                List<ItemTable> list = new List<ItemTable>();
                using (SQLiteDataReader dr = sqliteCmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        ItemTable tb = new ItemTable();
                        string key = dr["key"].ToString();
                        tb.key = key;
                        byte[] bs = dr["value"] as Byte[];
                        tb.valueJson = System.Text.Encoding.GetEncoding("UNICODE").GetString(bs);
                        list.Add(tb);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("数据读取报错：" + ex.Message);
            }
            finally
            {
                if (sqlite != null)
                {
                    sqliteCmd.Dispose();
                    sqlite.Close();
                }
            }
        }
    }
}

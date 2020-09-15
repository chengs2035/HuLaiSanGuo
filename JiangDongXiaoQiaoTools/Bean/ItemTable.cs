using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiangDongXiaoQiaoTools.Bean
{
    class ItemTable
    {
       public string key { get; set; }
       public SQLiteBlob value { get; set; }
       public string valueJson { get; set; }

    }
}

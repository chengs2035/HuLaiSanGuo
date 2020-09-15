using JiangDongXiaoQiaoTools.Bean;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JiangDongXiaoQiaoTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //全局配置
            gbMain.IsEnabled = false;

           


        }
        private string xqDb = "";

        System.Data.SQLite.SQLiteConnection sqlite = null;

        private void btnSelectXQPath_Click(object sender, RoutedEventArgs e)
        {
            gbMain.IsEnabled = false;

            CommonOpenFileDialog openfiledialog = new CommonOpenFileDialog();

            openfiledialog.IsFolderPicker = true;//选择文件夹

            if (openfiledialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string xqPath = openfiledialog.FileName;
                bool isFlag = true;
                //判断目录是否是小乔的目录，通过简单的使用小乔的基本特征进行判断
                if (!File.Exists(xqPath + "\\wow_helper.exe"))
                {
                    isFlag = false;
                }
                else if (!File.Exists(xqPath + "\\cef_extensions.pak"))
                {
                    isFlag = false;
                }

                if (isFlag)
                {
                    lblXQPath.Content = xqPath;
                }
                else
                {
                    lblXQPath.Content = "路径选择错误，请重新选择";
                    return;
                }
                 gbMain.IsEnabled = true;
                //加载用户数据，并且默认选择一个
                loadUser(xqPath);
            }
        }
        /**
         * 连接数据库
         * 
         */
        private void connDB(string dbpath) {

            sqlite = new System.Data.SQLite.SQLiteConnection(string.Format("Data Source={0};UTF8Encoding=True;Version=3",dbpath));
            try
            {
                sqlite.Open();
            }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                throw new System.Data.SQLite.SQLiteException("数据库连接异常：请关闭小乔后重试！异常具体信息："+ex.Message);
            }
            finally {
                //conn success.
                sqlite.Close();
            }
        }
        private void getUserInfo() {
            //将获取到的数据，加载到下拉框中
            DataTable dt = new DataTable();
            SQLiteCommand sqliteCmd=sqlite.CreateCommand();
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
                        tb.valueJson=System.Text.Encoding.GetEncoding("UNICODE").GetString(bs);
                        list.Add(tb);
                    }
                }
                for (int i = 0, l= list.Count; i < l; i++){
                    ItemTable table = list[i];
                    if (table.key != null && table.key.Contains("zUsers")) {
                        //zUsers用户表
                        //转换
                        
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally {
                if (sqlite != null) {
                    sqliteCmd.Dispose();
                    sqlite.Close();
                }
            }
        }
        /**
         * 载入小乔数据库
         * 
         */
        private bool loadUser(string xqPath) {
            //
            string xqDbPath=xqPath +"\\cache\\Local Storage\\http_127.0.0.1_20126.localstorage";
            if (!File.Exists(xqDbPath)) {
                throw new Exception("小乔路径下找不到对应的数据库，请确定已经使用过小乔，并且更新了矿号！");
            }
            //1.尝试连接数据库。
            connDB(xqDbPath);
            //数据库连接没问题，重新打开一次，并且读取用户数据
            getUserInfo();
            return true;
        }
    }
}

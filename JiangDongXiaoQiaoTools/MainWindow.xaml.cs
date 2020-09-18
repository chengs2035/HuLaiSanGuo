using JiangDongXiaoQiaoTools.Bean;
using JiangDongXiaoQiaoTools.DBHelp;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
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
            opDB = new OperDB();
        }
        private string xqDb = "";

        private OperDB opDB = null;

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

        /// <summary>
        /// 获取用户信息下拉框
        /// </summary>
        private void getUserInfo() {
            //将获取到的数据，加载到下拉框中
            try
            {
                List<ItemTable> list = opDB.getDBItemTable();
                string zUsersStr = opDB.getDBItemValue("zUsers");
                List<TUser> tusers = JsonConvert.DeserializeObject<List<TUser>>(zUsersStr);
                cbxUser.ItemsSource = tusers;
                cbxUser.DisplayMemberPath = "UserName";//显示出来的值
                cbxUser.SelectedValuePath = "roleId";//实际选中后获取的结果的值
                cbxUser.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /**
         * 载入小乔数据库
         * 
         */
        private bool loadUser(string xqPath) {
            //猜测的数据库
            string guestFileName = "";
            
            string[] files=Directory.GetFiles(string.Format("{0}\\cache\\Local Storage\\",xqPath));
            if (files == null || files.Length == 0)
            {
                throw new Exception("小乔路径下找不到对应的数据库，请确定已经使用过小乔，并且更新了矿号！");
            }
            else {
                for (int i = 0; i < files.Length; i++) {
                    guestFileName = files[i];
                    if (guestFileName.EndsWith("localstorage")) {
                        break;
                    }
                }
            }
           // string xqDbPath = xqPath + "\\cache\\Local Storage\\"+ guestFileName;

            if (!File.Exists(guestFileName)) {
                throw new Exception("小乔路径下找不到对应的数据库，请确定已经使用过小乔，并且更新了矿号！");
            }
            //1.尝试连接数据库。
            opDB.connDB(guestFileName);
            //执行数据备份操作
            
            File.Copy(guestFileName, string.Format("{0}.{1}.{2}",guestFileName ,System.DateTime.UtcNow.Ticks,"bak"));

            //数据库连接没问题，重新打开一次，并且读取用户数据
            getUserInfo();
            
            return true;
        }


        /// <summary>
        /// 当用户被改变的时候，需要加载对应用户下的矿号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool flag = false;
            if (cbxUser.SelectedItem == null) {
                //没有选择的时候，禁用保存按钮，禁用级别范围选择
                btnSave.IsEnabled = flag;
                cbx0130.IsEnabled = flag;
                cbx3140.IsEnabled = flag;
                cbx4150.IsEnabled = flag;
                cbx50plus.IsEnabled = flag;
                lbMineAccounts.ItemsSource = null;

                lbMineAccounts.Items.Clear();
                return;
            }

            //lbMineAccounts
            //1.读取用户角色id。
            TUser tuser = (TUser)cbxUser.SelectedItem;
            
            //2.根据角色id获取矿号
            string queryStr = string.Format("SubUser_{0}_{1}", tuser.platform, tuser.roleId);
            string subUserJson = opDB.getDBItemValue(queryStr);
            
            //3.将列表清空，并且加载矿号进入。
            List<SubUser> subUsers = JsonConvert.DeserializeObject<List<SubUser>>(subUserJson);
            
            if (subUsers != null && subUsers.Count > 0)
            {
                flag = true;
            }
            else {
                flag = false;
            }

            btnSave.IsEnabled = flag;
            cbx0130.IsEnabled = flag;
            cbx3140.IsEnabled = flag;
            cbx4150.IsEnabled = flag;
            cbx50plus.IsEnabled = flag;

            lbMineAccounts.ItemsSource = null;

            lbMineAccounts.Items.Clear();
            
            lbMineAccounts.ItemsSource = subUsers;

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (lbMineAccounts==null || lbMineAccounts.Items == null || lbMineAccounts.Items.Count == 0) return;
            CheckBox cbx = (CheckBox)sender;
            string tag = cbx.Tag.ToString();
            //CheckBox checkbox = (CheckBox)control;
            
            var query = from SubUser item in lbMineAccounts.Items
                        select item;
             
            if ("1".Equals(tag))
            {   
                //1-30级
                query = from SubUser item in lbMineAccounts.Items
                        where item.l > 1 && item.l < 31
                        select item;
            }
            else if ("2".Equals(tag)) {
                //31-40
                //1-30级
                query = from SubUser item in lbMineAccounts.Items
                        where item.l > 30 && item.l < 41
                        select item;
            }
            else if ("3".Equals(tag))
            {
                //41-50
                query = from SubUser item in lbMineAccounts.Items
                        where item.l > 40 && item.l < 51
                        select item;
            }
            else if ("4".Equals(tag))
            {
                //50以上
                query = from SubUser item in lbMineAccounts.Items
                        where item.l > 50
                        select item;
            }
            foreach (var item in query)
            {
                SubUser user = (SubUser)item;

                user.IsSelected = cbx.IsChecked.Value;
            }
            lbMineAccounts.Items.Refresh();


        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("请确认是否保存", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                var query = from SubUser item in lbMineAccounts.Items
                            where item.l > 1 && item.l < 31 && item.IsSelected == true
                            select new
                            {
                                u = item.u,
                                l = item.l
                            };
                string jsonSubUser = JsonConvert.SerializeObject(query);

                //1.读取用户角色id。
                TUser tuser = (TUser)cbxUser.SelectedItem;

                //2.根据角色id获取矿号
                string queryStr = string.Format("SubUser_{0}_{1}", tuser.platform, tuser.roleId);
                try
                {
                    if (opDB.updateItemTable(queryStr, jsonSubUser) > 0)
                    {
                        MessageBox.Show("保存成功拉，可以打开小乔看看了");
                        getUserInfo();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}

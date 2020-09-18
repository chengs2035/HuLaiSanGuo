using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiangDongXiaoQiaoTools.Bean
{
    public class TUser
    {
        public string server { get; set; }
        public string roleId { get; set; }
        public string deviceId { get; set; }
        public string authorCode { get; set; }
        public string userId { get; set; }
        public string gameId { get; set; }
        public string clientId { get; set; }
        //"platform":"QQ","clientVer":"v5.3.2","serverId":614,"name":"MtSky"}
        public string platform { get; set; }
        public string clientVer { get; set; }
        public string serverId { get; set; }
        public string name { get; set; }

        public string UserName
        {
            get
            {
                return string.Format("{0}:{1}:{2}",name, serverId, platform);
            }
        }
    }
}

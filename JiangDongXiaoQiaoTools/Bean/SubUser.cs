using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiangDongXiaoQiaoTools.Bean
{
    class SubUser
    {
        public SubUser() {
            IsSelected = true;
        }
        //{"u":5001789101,"l":12}
        /// <summary>
        /// user role id
        /// </summary>
        public string u { get; set; }
        /// <summary>
        /// user level 
        /// </summary>
        public int l { get; set; }
        /// <summary>
        /// 选中
        /// </summary>
        public bool IsSelected { get; set; }
        public string ul
        {
            get
            {
                return string.Format("{0}:{1}级", u, l);
            }
        }
        public override string ToString()
        {
            return string.Format("{0}:{1}级", u, l);
        }
    }
}

using DevExpress.XtraEditors.Controls;
using QuanLyBanVe.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanVe
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Localizer.Active = new VietHoaXtraMessageBox("&Hủy bỏ", "&Hủy", "&Chấp nhận", "&Không", "&Đồng ý", "&Thử lại", "&Có");


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new frmLogin());
        }
    }
}

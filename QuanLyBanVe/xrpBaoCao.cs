using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using QuanLyBanVe.Data;
using System.Collections.Generic;

namespace QuanLyBanVe
{
    public partial class xrpBaoCao : DevExpress.XtraReports.UI.XtraReport
    {
        public xrpBaoCao()
        {
            InitializeComponent();
        }
        public void InitData(List<SalesTicket> data)
        {
            objectDataSource1.DataSource = data;
        }
        public string BaoCao
        {
            get { return xrBaoCao.Text; }
            set { xrBaoCao.Text = value; }
        }
    }
}

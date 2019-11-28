using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Docking2010;
using QuanLyBanVe.Data;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Localization;
using QuanLyBanVe.Class;

namespace QuanLyBanVe
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        KhuVuiChoiDbContext db = new KhuVuiChoiDbContext();
        bool co = false;
        User user;
        List<SalesTicket> listVeTemp = new List<SalesTicket>();


        public frmMain(User user)
        {
            this.user = user;
            // Việt hóa gridview trong Dev
            GridLocalizer.Active = new VietHoaGridview();

            InitializeComponent();
            windowsUIButtonPanel1.AllowGlyphSkinning = true;
            windowsUIButtonPanel1.AppearanceButton.Pressed.BackColor = Color.DarkGray;
            windowsUIButtonPanel1.UseButtonBackgroundImages = false;
            windowsUIButtonPanel1.ButtonInterval = 4;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.Text = "(　-_･) ︻デ═一    ▸ Xin chào " + user.UserName + ". Chúc bạn điểm thật cao";
            DanhSachLoaiVe();
            CreateButtonVe();
            LayBillNoMoiNhat();

            deTuNgay.EditValue = DateTime.Today;
            deDenNgay.EditValue = DateTime.Today;

            //phân quyền
            if (user.IdGroup == "2")
            {
                windowsUIButtonPanel1.Buttons[3].Properties.Visible = false;
                ribbonControl1.Visible = false;
            }


        }

        private void WindowsUIButtonPanel1_ButtonClick(object sender, ButtonEventArgs e)
        {
            string tag = ((WindowsUIButton)e.Button).Tag.ToString();
            switch (tag)
            {
                case "Ad0":
                    navigationFrame1.SelectedPage = navigationPageTrangChu;
                    break;
                case "Ad1":
                    navigationFrame1.SelectedPage = navigationPageBanVe;
                    CreateButtonVe();
                    break;
                case "Ad2":
                    navigationFrame1.SelectedPage = navigationPageBaoCao;
                    break;
                case "Ad3":
                    navigationFrame1.SelectedPage = navigationPageLoaiVe;
                    break;
                case "Ad4":
                    DialogResult dlr = XtraMessageBox.Show("Bạn có chắc chắn muốn thoát khỏi chương trình?", "Thông báo"
                           , MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlr == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                    else
                    {
                        return;
                    }
                    break;
            }
        }

        #region Loại vé
        void DanhSachLoaiVe()
        {
            WaitDialogForm wForm = new WaitDialogForm("Vui lòng chờ trong giây lát", "Đang xử lý dữ liệu ...");
            try
            {
                wForm.Show();
                var ds = db.TicketTypes.OrderBy(x => x.IdTicket).Select(x => x).ToList();
                gcLoaiVe.DataSource = ds;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi DanhSachLoaiVe:\n " + ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                wForm.Close();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        void HienThiLoaiVe()
        {
            try
            {
                TicketType row = (gcLoaiVe.FocusedView as GridView).GetRow(gvLoaiVe.FocusedRowHandle) as TicketType;

                if (row != null)
                {
                    txtMaVe.Text = row.IdTicket.ToString();
                    txtTenVe.Text = row.NameTicket.ToString();
                    txtGiaVe.Text = row.PriceTicket.ToString();
                    btnAddLoaiVe.Enabled = true;
                    btnSaveLoaiVe.Enabled = true;
                    btnDeleteLoaiVe.Enabled = true;
                }
                co = false;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi HienThiLoaiVe:\n " + ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void BtnAddLoaiVe_Click(object sender, EventArgs e)
        {
            txtMaVe.Text = "";
            txtTenVe.Text = "";
            txtGiaVe.Text = "0";
            txtTenVe.Focus();
            btnAddLoaiVe.Enabled = false;
            btnSaveLoaiVe.Enabled = true;
            btnDeleteLoaiVe.Enabled = false;
            co = true;
        }
        private void BtnSaveLoaiVe_Click(object sender, EventArgs e)
        {
            if (txtTenVe.Text == "")
            {
                XtraMessageBox.Show("Tên vé không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenVe.Focus();
                txtTenVe.SelectAll();
            }
            else if (txtGiaVe.Text == "")
            {
                XtraMessageBox.Show("Giá vé không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaVe.Focus();
                txtGiaVe.SelectAll();
            }
            else
            {
                WaitDialogForm wForm = new WaitDialogForm("Vui lòng chờ trong giây lát", "Đang xử lý dữ liệu ...");
                try
                {
                    wForm.Show();

                    TicketType kh = (gcLoaiVe.FocusedView as GridView).GetRow(gvLoaiVe.FocusedRowHandle) as TicketType;
                    if (co == false)
                    {
                        //nếu có thì update
                        if (kh != null)
                        {
                            int trThaiCu = kh.Sta ?? 0;
                            //update
                            kh.NameTicket = txtTenVe.Text;
                            kh.PriceTicket = decimal.Parse(txtGiaVe.Text);
                            kh.Sta = 1;
                            db.SaveChanges();
                            wForm.Close();

                            if (trThaiCu == 0)
                            {
                                XtraMessageBox.Show("Kích hoạt thành công");
                            }
                            else
                            {
                                XtraMessageBox.Show("Cập nhật thành công");
                            }
                        }
                    }
                    //nếu chưa có thì insert
                    else
                    {
                        //insert
                        kh = new TicketType();
                        kh.NameTicket = txtTenVe.Text;
                        kh.PriceTicket = decimal.Parse(txtGiaVe.Text);
                        kh.Sta = 1;
                        db.TicketTypes.Add(kh);
                        db.SaveChanges();

                        wForm.Close();

                        XtraMessageBox.Show("Thêm mới thành công");

                        Refresh();
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Lỗi BtnSave_Click:\n " + ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    wForm.Close();

                    DanhSachLoaiVe();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

            }
        }
        void Refresh()
        {
            txtMaVe.Text = "";
            txtTenVe.Text = "";
            txtGiaVe.Text = "0";
            txtTenVe.Focus();
            btnAddLoaiVe.Enabled = true;
            btnSaveLoaiVe.Enabled = true;
            btnDeleteLoaiVe.Enabled = true;
            co = true;
        }
        private void BtnDeleteLoaiVe_Click(object sender, EventArgs e)
        {
            TicketType kh = (gcLoaiVe.FocusedView as GridView).GetRow(gvLoaiVe.FocusedRowHandle) as TicketType;
            if (kh == null)
            {
                XtraMessageBox.Show("Chưa chọn vé", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                if (XtraMessageBox.Show("Bạn có chắc chắn muốn hủy hiệu lực mã vé này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    WaitDialogForm wForm = new WaitDialogForm("Vui lòng chờ trong giây lát", "Đang xử lý dữ liệu ...");
                    try
                    {
                        wForm.Show();

                        if (kh.Sta == 0)
                        {
                            wForm.Close();
                            XtraMessageBox.Show("Vé này đã không còn hiệu lực, không thể hủy");
                        }
                        else
                        {
                            kh.Sta = 0;
                            db.SaveChanges();
                            wForm.Close();
                            XtraMessageBox.Show("Hủy hiệu lực thành công");
                        }

                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show("Lỗi BtnDelete_Click:\n " + ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    finally
                    {
                        wForm.Close();

                        DanhSachLoaiVe();

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }
        }
        private void GvLoaiVe_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            HienThiLoaiVe();
        }
        private void GvLoaiVe_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (e.Column.FieldName == "Sta" && e.ListSourceRowIndex != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                int trangThai = Convert.ToInt32(e.Value);
                switch (trangThai)
                {
                    case 0:
                        e.DisplayText = "In-Active";
                        break;
                    case 1:
                        e.DisplayText = "Active";
                        break;
                }
            }
        }
        private void GvLoaiVe_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            //Doi mau cell cua colummn Status, neu co gia tri In-Actived thi co mau đỏ
            if (e.RowHandle >= 0)
            {
                string sta = view.GetRowCellDisplayText(e.RowHandle, view.Columns["Sta"]);
                if (sta == "In-Active")
                {
                    e.Appearance.BackColor = Color.Red;
                }
            }
        }
        private void TxtGiaVe_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        #endregion

        #region Bán vé
        void LayBillNoMoiNhat()
        {
            var bill = db.SalesTickets.OrderByDescending(x => x.Id).Select(x => x).FirstOrDefault();
            if (bill == null)
            {
                txtBillNo.Text = 1.ToString("000000");
            }
            else
            {
                int so = Int32.Parse(bill.BillNo);
                txtBillNo.Text = (so + 1).ToString("000000");
            }
        }
        void CreateButtonVe()
        {
            flowLayoutPanel1.Controls.Clear();
            var ds = db.TicketTypes.Where(x => x.Sta != 0).OrderBy(x => x.IdTicket).Select(x => x).ToList();
            if (ds.Count > 0)
            {
                Button btn;
                for (int i = 0; i < ds.Count; i++)
                {
                    btn = new Button();
                    btn.Font = new Font("Times New Roman", 12.0f, FontStyle.Bold);
                    btn.BackColor = Color.Orange;
                    btn.Height = 70;
                    btn.Width = 110;
                    btn.Name = ds[i].IdTicket.ToString();
                    btn.Text = ds[i].NameTicket;

                    btn.Click += Btn_Click;
                    flowLayoutPanel1.Controls.Add(btn);
                }
            }
        }
        private void Btn_Click(object sender, EventArgs e)
        {
            string billNo = txtBillNo.Text.Trim();
            Button btn = (Button)sender;
            int idTicket = Int32.Parse(btn.Name);
            string nameTicket = btn.Text;
            decimal price = db.TicketTypes.Where(x => x.IdTicket == idTicket).Select(x => x.PriceTicket).FirstOrDefault() ?? 0;
            var tk = listVeTemp.Where(x => x.BillNo == billNo && x.IdTicket == idTicket).FirstOrDefault();
            if (tk == null)
            {
                SalesTicket s = new SalesTicket
                {
                    BillNo = billNo,
                    CreateDate = DateTime.Now,
                    SeqNo = listVeTemp.Count + 1,
                    IdTicket = idTicket,
                    NameTicket = nameTicket,
                    PriceTicket = price,
                    Quantity = Int32.Parse(txtSoLuong.Text.Trim()),
                    TotalAmount = Int32.Parse(txtSoLuong.Text.Trim()) * price,
                    UserId = user.UserId,
                    Sta = 1
                };
                listVeTemp.Add(s);
            }
            else
            {
                tk.Quantity = tk.Quantity + Int32.Parse(txtSoLuong.Text.Trim());
                tk.TotalAmount = tk.TotalAmount + (Int32.Parse(txtSoLuong.Text.Trim()) * price);
            }
            gcLuoi.DataSource = listVeTemp;
            gcLuoi.RefreshDataSource();
            txtSoLuong.Text = "1";
        }

        private void btnXoaVe_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            SalesTicket p = (gcLuoi.FocusedView as GridView).GetRow(gvLuoi.FocusedRowHandle) as SalesTicket;
            if (p != null)
            {
                listVeTemp.Remove(p);
                gcLuoi.DataSource = listVeTemp;
                gcLuoi.RefreshDataSource();
            }
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            foreach (var item in listVeTemp)
            {
                db.SalesTickets.Add(item);
                db.SaveChanges();
            }
            XtraMessageBox.Show("Thanh toán thành công");
            LayBillNoMoiNhat();
            listVeTemp = new List<SalesTicket>();
            gcLuoi.DataSource = listVeTemp;
            gcLuoi.RefreshDataSource();
        }
        #endregion

        #region Báo cáo
        protected virtual void XemVaIn(DateTime tuNgay, DateTime denNgay)
        {
            WaitDialogForm wForm = new WaitDialogForm("Vui lòng chờ trong giây lát", "Đang xử lý dữ liệu ...");
            try
            {
                wForm.Show();
                List<SalesTicket> list = db.SalesTickets.AsEnumerable().Where(x => x.CreateDate >= tuNgay && x.CreateDate <= denNgay).ToList();
                wForm.Close();

                var r = new xrpBaoCao();
                r.BaoCao = "BÁO CÁO BILL TỪ NGÀY " + tuNgay.ToString("dd/MM/yyyy") + " ĐẾN NGÀY " + denNgay.ToString("dd/MM/yyyy");

                r.InitData(list);

                //r.Bands[BandKind.Detail].Controls.Add(CopyGridControl(gcSanPham));
                // Disable margins warning.
                r.ShowPrintMarginsWarning = false;
                r.Margins = new System.Drawing.Printing.Margins(40, 35, 20, 15);
                // tên mặc định save report
                r.DisplayName = "Báo cáo";
                r.CreateDocument();
                documentViewer1.DocumentSource = r;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi XemVaIn:\n " + ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                wForm.Close();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void btnXemBaoCao_Click(object sender, EventArgs e)
        {
            XemVaIn(DateTime.Parse(deTuNgay.EditValue.ToString().Trim()), DateTime.Parse(deDenNgay.EditValue.ToString().Trim()));
        }

        #endregion

    }
}
using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using System.Runtime.InteropServices;
using QuanLyBanVe.Properties;
using QuanLyBanVe.Data;
using System.Linq;

namespace QuanLyBanVe
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        KhuVuiChoiDbContext db = new KhuVuiChoiDbContext();
        public frmLogin()
        {
            InitializeComponent();

            #region load lại file kiểm tra có lưu hay không lưu
            try
            {
                bool remember = Settings.Default.Remember;
                string userId = Settings.Default.UserName;
                if (remember)
                {
                    chkNhoMK.Checked = true;
                    txtUserID.Text = userId;
                    txtPassword.Focus();
                }
                else
                {
                    chkNhoMK.Checked = false;
                    txtUserID.Focus();
                }
            }
            catch { }
            #endregion
        }


        //class InternetConnection
        //{
        //    [DllImport("wininet.dll")]
        //    private extern static bool InternetGetConnectedState(out int description, int reservedValuine);
        //    public static bool IsConnectedToInternet()
        //    {
        //        int desc;
        //        return InternetGetConnectedState(out desc, 0);
        //    }
        //}

        private void frmLogin_Load(object sender, EventArgs e)
        {

            if (txtUserID.Text.Trim().Length <= 0)
            {
                txtUserID.Focus();
            }
            else
            {
                //set focus trong Edittext devexpress
                BeginInvoke(new MethodInvoker(delegate { txtPassword.Focus(); }));
            }

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUserID.Text == "")
            {
                XtraMessageBox.Show("Tài khoản không bỏ trống.", "Thông báo");
                txtUserID.Focus();
            }
            else if (txtPassword.Text == "")
            {
                XtraMessageBox.Show("Mật khẩu không bỏ trống.", "Thông báo");
                txtPassword.Focus();
            }
            else
            {
                WaitDialogForm wForm = new WaitDialogForm("Vui lòng chờ trong giây lát", "Đang tải dữ liệu ...");
                try
                {
                    db = new KhuVuiChoiDbContext();
                    wForm.Show();
                    string userId = txtUserID.Text.Trim();
                    string pass = txtPassword.Text.Trim();

                    // call service - everything's fine
                    var user = db.Users.Where(x => x.UserId == userId && x.Password == pass && x.Sta != 0)
                                       .Select(x => x).FirstOrDefault();

                    if (user == null)
                    {
                        wForm.Close();
                        XtraMessageBox.Show("Đăng nhập không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtPassword.Text = "";
                        txtUserID.Focus();
                        txtUserID.SelectAll();
                    }
                    else
                    {

                        frmMain frm = new frmMain(user);
                        Visible = false;
                        frm.Activate();

                        wForm.Close();

                        frm.ShowDialog();
                        #region kiểm tra file lưu id
                        if (chkNhoMK.Checked)
                        {
                            Settings.Default.UserName = txtUserID.Text;
                            Settings.Default.Remember = true;
                            Settings.Default.Save();
                        }
                        #endregion
                        this.Close();

                    }
                }

                catch (Exception ex)
                {
                    XtraMessageBox.Show("Lỗi:\n " + ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    wForm.Close();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        private void chkNhoMK_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNhoMK.Checked)
            {
                chkNhoMK.Text = "Không nhớ tài khoản";
                btnLogin.Focus();
            }
            else
            {
                chkNhoMK.Text = "Nhớ tài khoản";
                btnLogin.Focus();
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void txtUserID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPassword.Focus();
            }
        }

    }
}
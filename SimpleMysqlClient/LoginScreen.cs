using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleMysqlClient
{
    public partial class LoginScreen : Form
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void LoginScreen_Load(object sender, EventArgs e)
        {
            tbDbHost.Text = "localhost";
            tbDbPort.Text = "3306";
            tbDbUser.Text = "root";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                AV.DatabaseMySQL db = Global.app.sql;
                db.Host = tbDbHost.Text;
                db.Port = ushort.Parse(tbDbPort.Text);
                db.User = tbDbUser.Text;
                db.Password = tbDbPass.Text;
                db.Database = tbDbBase.Text;

                db.Connect(false);
                if(db.Connected)
                {
                    Global.app.OpenConsole();
                    Global.app.context.MainForm = Global.app.formConsole;
                    this.Close();
                }
            }
            catch(Exception exc)
            {
                MessageBox.Show($"Исключение при попытке подключиться к БД: {exc.Message}");
            }
        }
    }
}

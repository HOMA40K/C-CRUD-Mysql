using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AV;

namespace SimpleMysqlClient
{
    public static class Global
    {
        public static SimpleMyClientApp app { get; private set; } = null;

        public static void InitializeApp()
        {
            if (app != null) return;
            app = new SimpleMyClientApp();
        }

        public static void AlignCenterLabel(System.Windows.Forms.Label lbl, bool adjust_y = true)
        {
            if (adjust_y) lbl.Location = new Point((lbl.Parent.Width / 2) - (lbl.Width / 2), (lbl.Parent.Height / 2) - (lbl.Height / 2));
            else lbl.Location = new Point((lbl.Parent.Width / 2) - (lbl.Width / 2), lbl.Location.Y);
        }

        public static string TimeStr
        {
            get
            {
                DateTime now = DateTime.Now;
                return $"({now:HH:mm:ss})";
            }
        }
    }


    public class SimpleMyClientApp
    {
        public LoginScreen formLogin { get; private set; }
        public MySQLClient formConsole { get; private set; }
        public ApplicationContext context { get; private set; }
        public MsgConsole console { get; private set; } = new MsgConsole();

        /* Event Stuff */
        public event EventHandler<MsgConsoleEventArgs> Message_Pushed;
        void OnMessage_Pushed(MsgConsoleEventArgs e) => Message_Pushed?.Invoke(this, e);
        private void MsgConsole_Message_Pushed(object sender, MsgConsoleEventArgs e) => OnMessage_Pushed(e);
        /* Event Stuff - END */

        public AV.DatabaseMySQL sql;

        public SimpleMyClientApp()
        {
            sql = new AV.DatabaseMySQL();
            formLogin = new LoginScreen();
            context = new ApplicationContext(formLogin);

            console.Message_Pushed += MsgConsole_Message_Pushed;
        }

        public void OpenConsole()
        {
            if (formConsole == null || formConsole.IsDisposed)
            {
                formConsole = new MySQLClient();
                formConsole.Show();
            }
            else formConsole.BringToFront();
        }

        public void CloseConsole()
        {
            formConsole = null;
        }
    }
}

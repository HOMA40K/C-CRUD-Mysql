using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleMysqlClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Global.InitializeApp();
            try
            {
                Application.Run(Global.app.context);
            }
            catch(Exception e)
            {
                MessageBox.Show($"Fatal exception: {e.Message}");
            }
            Global.app.sql.Dispose();
        }
    }
}

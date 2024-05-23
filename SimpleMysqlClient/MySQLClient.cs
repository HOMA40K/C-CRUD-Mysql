using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AV;

namespace SimpleMysqlClient
{
    public partial class MySQLClient : Form
    {
        int displaymarker = -1;
        List<ConsoleEntry> msgs = new List<ConsoleEntry>();

        public MySQLClient()
        {
            InitializeComponent();
        }

        private void MySQLClient_Load(object sender, EventArgs e)
        {
            lbDbName.Text = $"{Global.app.sql.Host} : {(Global.app.sql.Database.Length > 0 ? Global.app.sql.Database : "Глобальный доступ")}";
            Global.AlignCenterLabel(lbDbName, false);

            Global.app.Message_Pushed += App_MessagePushed;
            displaymarker = Global.app.console.GetDisplayMarker();

            LoadMessages();
            DisplayAllMessages();
        }

        void LoadMessages()
        {
            if (msgs.Count == 0) msgs = Global.app.console.GetAllMessagesAfterID(displaymarker);
            else msgs = Global.app.console.GetAllMessagesAfterID(msgs.Last().ID);
        }

        void DisplayAllMessages()
        {
            foreach (ConsoleEntry msg in msgs)
            {
                DisplayMessage(msg);
            }
        }

        void DisplayMessage(ConsoleEntry msg)
        {
            msgs.Add(msg);

            string prefix = msg.ID + ". ";

            switch (msg.Level)
            {
                case ConsoleEntry.Severity.Ok:
                    rtbConsole.SelectionBackColor = Color.White;
                    rtbConsole.SelectionColor = Color.Black;
                    break;
                case ConsoleEntry.Severity.Notify:
                    rtbConsole.SelectionBackColor = Color.FromArgb(199,255,199);
                    rtbConsole.SelectionColor = Color.Black;
                    break;
                case ConsoleEntry.Severity.Warning:
                    //prefix += "Предупреждение: ";
                    rtbConsole.SelectionBackColor = Color.LightYellow;
                    rtbConsole.SelectionColor = Color.Black;
                    break;
                case ConsoleEntry.Severity.Error:
                    //prefix += "Ошибка: ";
                    rtbConsole.SelectionBackColor = Color.IndianRed;
                    rtbConsole.SelectionColor = Color.WhiteSmoke;
                    break;
                case ConsoleEntry.Severity.Critical:
                    //prefix += "Критическая ошибка: ";
                    rtbConsole.SelectionBackColor = Color.Red;
                    rtbConsole.SelectionColor = Color.WhiteSmoke;
                    break;
            }

            //if (msg.Sender.Length > 0) prefix += "(" + msg.Sender + ") ";

            rtbConsole.AppendText(msg.Message + "\n");
            rtbConsole.ScrollToCaret();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //tbCmdInput.Clear();
            msgs.Clear();
            rtbConsole.Clear();
        }

        private void btnExecuteStatement_Click(object sender, EventArgs e)
        {
            if (tbCmdInput.Text.Length == 0) return;
            try
            {
                Global.app.console.PushMessage($">> {tbCmdInput.Text}", ConsoleEntry.Severity.Ok);
                if(dgvRecordView.Columns.Count>0)
                {
                    dgvRecordView.Rows.Clear();
                    dgvRecordView.Columns.Clear();
                }

                int result = -1;
                int reccount = 0;
                string query = tbCmdInput.Text;
                using (MySqlCommand cmd = new MySqlCommand(query, Global.app.sql.connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    bool doonce = false;

                    result = reader.RecordsAffected;
                    while (reader.Read())
                    {
                        reccount++;
                        if(!doonce)
                        {
                            //int fc = reader.FieldCount;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string col_name = reader.GetName(i);
                                dgvRecordView.Columns.Add($"field_{i}", col_name);
                            }
                            doonce = true;
                        }

                        List<string> fields = new List<string>(reader.FieldCount);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string val = !reader.IsDBNull(i) ? reader.GetString(i) : "<null>"; //if (val.Length == 0) val = "<null>";
                            fields.Add(val);
                        }
                        dgvRecordView.Rows.Add(fields.ToArray());
                    }
                }

                Global.app.console.PushMessage($"{Global.TimeStr} Query OK.{(result != -1 ? $" Affected {result} records." : "")}" +
                    $"{(reccount>0 ? $" Retrieved {reccount} records." : "")}", ConsoleEntry.Severity.Notify);
                tbCmdInput.Clear();
            }
            catch(MySqlException exc)
            {
                Global.app.console.PushMessage($"{Global.TimeStr} MySQL Error: {exc.Message}", ConsoleEntry.Severity.Error);
            }
            catch(Exception exc)
            {
                Global.app.console.PushMessage($"{Global.TimeStr} Application Error: {exc.Message}\n{exc.StackTrace}", ConsoleEntry.Severity.Critical);
            }
        }

        private void App_MessagePushed(object sender, MsgConsoleEventArgs e)
        {
            DisplayMessage(Global.app.console.GetMessage(e.StartID));
            Global.app.console.SetDisplayMarker(e.StartID);
        }

        private void MySQLClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global.app.Message_Pushed -= App_MessagePushed;
            Global.app.CloseConsole();
        }

        private void MySQLClient_Resize(object sender, EventArgs e)
        {
            Global.AlignCenterLabel(lbDbName, false);
        }

        private void btnClearInput_Click(object sender, EventArgs e)
        {
            tbCmdInput.Clear();
        }
    }
}

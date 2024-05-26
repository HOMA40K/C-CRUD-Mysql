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
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;

namespace SimpleMysqlClient
{    
    public partial class MySQLClient : Form
    {
        private string table = "traumas";
        int displaymarker = -1;
        List<ConsoleEntry> msgs = new List<ConsoleEntry>();
        private List<string> fields = new List<string>();

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

            string querry = "SHOW TABLES";
            List<string> tables = new List<string>();
            using (MySqlCommand cmd = new MySqlCommand(querry, Global.app.sql.connection))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        try
                        {
                            tables.Add(reader.GetString(i));
                        }
                        catch (Exception exc)
                        {
                            Global.app.console.PushMessage($"{Global.TimeStr} Ошибка при получении списка таблиц: {exc.Message}", ConsoleEntry.Severity.Error);
                        }
                    }                    

                }
                
            }
            comboBox1.Items.AddRange(tables.ToArray());
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
            string table_name = tbCmdInput.Text.Split(' ')[2];
            
            Global.app.console.PushMessage($"{Global.TimeStr} Недопустимая таблица для выполнения запроса. {table_name}", ConsoleEntry.Severity.Error);
            try
            {
                Global.app.console.PushMessage($">> {tbCmdInput.Text}", ConsoleEntry.Severity.Ok);
                if (dgvRecordView.Columns.Count > 0)
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
                        if (!doonce)
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string col_name = reader.GetName(i);
                                dgvRecordView.Columns.Add($"field_{i}", col_name);
                            }
                            doonce = true;
                        }

                        fields = new List<string>(reader.FieldCount);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string val;
                            if (reader.IsDBNull(i))
                            {
                                val = "<null>";
                            }
                            else
                            {
                                var fieldType = reader.GetFieldType(i);
                                if (fieldType == typeof(string))
                                {
                                    val = reader.GetString(i);
                                }
                                else if (fieldType == typeof(int))
                                {
                                    val = reader.GetInt32(i).ToString();
                                }
                                else if (fieldType == typeof(DateTime))
                                {
                                    val = reader.GetDateTime(i).ToString();
                                }
                                else if (fieldType == typeof(bool))
                                {
                                    val = reader.GetBoolean(i).ToString();
                                }
                                else if (fieldType == typeof(decimal))
                                {
                                    val = reader.GetDecimal(i).ToString();
                                }
                                else
                                {
                                    val = reader.GetValue(i).ToString();
                                }
                            }
                            fields.Add(val);
                        }
                        dgvRecordView.Rows.Add(fields.ToArray());
                    }
                }

                Global.app.console.PushMessage($"{Global.TimeStr} Query OK.{(result != -1 ? $" Affected {result} records." : "")}" +
                    $"{(reccount > 0 ? $" Retrieved {reccount} records." : "")}", ConsoleEntry.Severity.Notify);
                tbCmdInput.Clear();
            }
            catch (MySqlException exc)
            {
                Global.app.console.PushMessage($"{Global.TimeStr} MySQL Error: {exc.Message}", ConsoleEntry.Severity.Error);
            }
            catch (Exception exc)
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

        private int GetNewID()
        {
            int highest = 0;
            foreach (DataGridViewRow row in dgvRecordView.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    int id = int.Parse(row.Cells[0].Value.ToString());
                    if (id > highest) highest = id;
                }
            }
            return highest + 1;
        }
        private void RefreshDataGridView()
        {
            string query = $"SELECT * FROM {table}"; // Adjust the query to match your table structure
            using (MySqlCommand cmd = new MySqlCommand(query, Global.app.sql.connection))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                dgvRecordView.Rows.Clear();
                dgvRecordView.Columns.Clear();

                bool doonce = false;
                while (reader.Read())
                {
                    if (!doonce)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string col_name = reader.GetName(i);
                            DataGridViewColumn column = new DataGridViewTextBoxColumn
                            {
                                Name = $"field_{i}",
                                HeaderText = col_name,
                                ReadOnly = false
                            };
                            dgvRecordView.Columns.Add(column);
                        }
                        doonce = true;
                    }

                    List<string> fields = new List<string>(reader.FieldCount);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string val;
                        if (reader.IsDBNull(i))
                        {
                            val = "<null>";
                        }
                        else
                        {
                            var fieldType = reader.GetFieldType(i);
                            if (fieldType == typeof(string))
                            {
                                val = reader.GetString(i);
                            }
                            else if (fieldType == typeof(int))
                            {
                                val = reader.GetInt32(i).ToString();
                            }
                            else if (fieldType == typeof(DateTime))
                            {
                                val = reader.GetDateTime(i).ToString();
                            }
                            else if (fieldType == typeof(bool))
                            {
                                val = reader.GetBoolean(i).ToString();
                            }
                            else if (fieldType == typeof(decimal))
                            {
                                val = reader.GetDecimal(i).ToString();
                            }
                            else
                            {
                                val = reader.GetValue(i).ToString();
                            }
                        }
                        fields.Add(val);
                    }
                    dgvRecordView.Rows.Add(fields.ToArray());
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Generate a new ID or get it from a user input (here, assumed to be a new unique ID)
                int id = GetNewID(); // Implement this function to generate or fetch a new unique ID

                string name = "DATA"; // Replace with actual data or get from user input
                int age = 0; // Replace with actual data or get from user input
                string trauma = "DATA"; // Replace with actual data or get from user input

                string query = $"INSERT INTO {table} (ID, name, age, trauma) VALUES (@id, @name, @age, @trauma)";

                using (MySqlCommand cmd = new MySqlCommand(query, Global.app.sql.connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@age", age);
                    cmd.Parameters.AddWithValue("@trauma", trauma);

                    cmd.ExecuteNonQuery();
                }

                Global.app.console.PushMessage($"{Global.TimeStr} Record inserted successfully with ID {id}.", ConsoleEntry.Severity.Notify);

                // Optionally, refresh the DataGridView to show the new data
                RefreshDataGridView();
            }
            catch (MySqlException exc)
            {
                Global.app.console.PushMessage($"{Global.TimeStr} MySQL Error: {exc.Message}", ConsoleEntry.Severity.Error);
            }
            catch (Exception exc)
            {
                Global.app.console.PushMessage($"{Global.TimeStr} Application Error: {exc.Message}\n{exc.StackTrace}", ConsoleEntry.Severity.Critical);
            }
        }

        private void dgvRecordView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // This event is triggered when editing of a cell ends
            string columnName = "";
            string collumnIndex = "";
            string newValue = "";
            try
            {
                columnName = dgvRecordView.Columns[e.ColumnIndex].HeaderText.ToString();
                collumnIndex = dgvRecordView.Rows[e.RowIndex].Cells[0].Value.ToString();
                newValue = dgvRecordView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            catch (Exception exc)
            {
                Global.app.console.PushMessage($"{Global.TimeStr} Ошибка при редактировании ячейки: {exc.Message}", ConsoleEntry.Severity.Error);
            }
            string querry = $"UPDATE {table} SET {columnName} = '{newValue}' WHERE ID = {collumnIndex}";
            using (MySqlCommand cmd = new MySqlCommand(querry, Global.app.sql.connection))
            {
                cmd.ExecuteNonQuery();
            }

            Global.app.console.PushMessage($"{Global.TimeStr} Cell editing ended in column '{columnName}' with new value '{newValue}' ID is {collumnIndex}", ConsoleEntry.Severity.Error);
            
        }
        private int checkInt(string value)
        {
            Regex regex = new Regex("^[0-9]\\d*$");
            if (regex.IsMatch(value))
            {
                return int.Parse(value);
            }
            else
            {
                return -1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string row = IDselect.Text;
            if(checkInt(row) == -1)
            {
                Global.app.console.PushMessage($"{Global.TimeStr} Недопустимое значение ID", ConsoleEntry.Severity.Error);
                return;
            }
            else
            {
                string query = $"DELETE FROM {table} WHERE ID = {row}";

                using (MySqlCommand cmd = new MySqlCommand(query, Global.app.sql.connection))
                {
                    cmd.ExecuteNonQuery();
                }
                RefreshDataGridView();
            }
        }

        private void dgvRecordView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            table = comboBox1.Text;
            RefreshDataGridView();
        }

    }
}

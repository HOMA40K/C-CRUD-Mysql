using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace AV
{
    public class AVMySqlException : Exception
    {
        public AVMySqlException(string message) : base(message) { }
    }

    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class RecordValue
    {
        public Record Parent;
        public object Value;
        public bool Ignore;

        public RecordValue(Record r)
        {
            Parent = r;
            Value = null;
            Ignore = true;
        }

        public RecordValue(Record r, object val, bool ignore)
        {
            Parent = r;
            Value = val;
            Ignore = ignore;
        }

        public Column GetColumn() => Parent?.Parent.Columns[Parent.Values.FindIndex(x => x == this)];

        private string DebuggerDisplay => string.Format("[{0}] = {1}", GetColumn()?.Name ?? "<null>", Value ?? "NULL");
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Record
    {
        public Table Parent { get; private set; }
        public List<RecordValue> Values { get; private set; }

        public Record(Table pParent)
        {
            Parent = pParent;
            Values = new List<RecordValue>(Parent.Columns.Count);

            foreach (Column c in Parent.Columns)
            {
                //if ((c.Properties & Column.ColumnFlags.NotNull) == 0 && c.Default == null) Values.Add(null);
                Values.Add(new RecordValue(this, c.Default, true));
            }
        }

        public Record(Table pParent, IEnumerable<object> init)
        {
            if (init.Count() != pParent.Columns.Count) throw new AVMySqlException(string.Format("Column count mismatch in initializer for table {0}", pParent.Name));

            Parent = pParent;
            Values = new List<RecordValue>(pParent.Columns.Count);
            foreach(object val in init) Values.Add(new RecordValue(this, val, false));
        }

        public Record(Table pParent, IEnumerable<object> values, IEnumerable<string> fields) : this(pParent)
        {
            if (fields.Count() > values.Count()) throw new AVMySqlException("Fewer fields than values in Record constructor!");
            for(int i = 0; i<fields.Count();i++)
            {
                int index = pParent.Columns.FindIndex(x => x.Name == fields.ElementAt(i));
                if (index != -1)
                {
                    Values[index].Value = values.ElementAt(i);
                    Values[index].Ignore = false;
                }
            }
        }

        public object this[string column]
        {
            get
            {
                int index = Parent.Columns.FindIndex(x => x.Name == column);
                return (index != -1) ? Values[index].Value : null;
            }
            set
            {
                int index = Parent.Columns.FindIndex(x => x.Name == column);
                if (index != -1) { Values[index].Value = (value is DBNull ? null : value); Values[index].Ignore = false; }
            }
        }

        public void IgnoreField(string column)
        {
            int index = Parent.Columns.FindIndex(x => x.Name == column);
            if (index != -1) Values[index].Ignore = true;
        }

        public void UnIgnoreField(string column)
        {
            int index = Parent.Columns.FindIndex(x => x.Name == column);
            if (index != -1) Values[index].Ignore = false;
        }

        public bool FieldIgnored(string column)
        {
            int index = Parent.Columns.FindIndex(x => x.Name == column);
            return (index!=-1) ? Values[index].Ignore : false;
        }

        public bool FullIgnore()
        {
            bool result = true;
            foreach (RecordValue v in Values) result &= v.Ignore;
            return result;
        }

        public bool GetBool(string column)
        {
            object r = this[column];
            return (r is ulong) ? (ulong)(this[column] ?? Convert.ToUInt64(0)) != 0 : (bool)r;
        }
        public int GetInt(string column) => (int)(this[column] ?? -1);


        private string DebuggerDisplay
        {
            get { return string.Format("{0} : {1} fields", Parent.Name, Values.Count); }
        }
    }

    public class Enumeration
    {
        public List<string> Symbols { get; set; } = new List<string>();

        public string PrepareCreateStatement()
        {
            string st = "ENUM (";
            foreach (string sym in Symbols) st += string.Format("'{0}'{1}", sym, sym != Symbols.Last() ? ", " : "");
            st += ")";

            return st;
        }

        public Enumeration Copy()
        {
            Enumeration result = new Enumeration();
            result.Symbols.AddRange(this.Symbols);
            return result;
        }
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Column
    {
        public enum DataType
        {
            Int, TinyInt, SmallInt, MediumInt, BigInt,
            Bit, Bool,
            Real, Double, Float, Decimal, Numeric,
            Date, Time, TimeStamp, DateTime, Year,

            Char, Varchar,
            Binary, Varbinary,
            TinyBlob, Blob, MediumBlob, LongBlob,
            TinyText, Text, MediumText, LongText,
            Enum, Set, JSON
        };

        public static Type ConvertSqlType(DataType type)
        {
            Type result = null;

            switch (type)
            {
                case DataType.Int:
                case DataType.TinyInt:
                case DataType.SmallInt:
                case DataType.MediumInt:
                    result = Type.GetType("int");
                    break;
                case DataType.BigInt:
                    result = Type.GetType("long");
                    break;
                case DataType.Bit:
                case DataType.Bool:
                    result = Type.GetType("bool");
                    break;
                case DataType.Real:
                case DataType.Double:
                    result = Type.GetType("double");
                    break;
                case DataType.Float:
                    result = Type.GetType("float");
                    break;
                case DataType.Decimal:
                case DataType.Numeric:
                    result = Type.GetType("decimal");
                    break;
                case DataType.Date:
                case DataType.Time:
                case DataType.TimeStamp:
                case DataType.DateTime:
                case DataType.Year:
                    result = Type.GetType("DateTime");
                    break;
                case DataType.Char: case DataType.Varchar: result = Type.GetType("string"); break;
                case DataType.Binary:
                case DataType.Varbinary:
                case DataType.TinyBlob:
                case DataType.Blob:
                case DataType.MediumBlob:
                case DataType.LongBlob: result = Type.GetType("Byte[]"); break;
                case DataType.TinyText:
                case DataType.Text:
                case DataType.MediumText:
                case DataType.LongText:
                    break;
                case DataType.Enum: case DataType.Set: result = Type.GetType("string"); break;
                case DataType.JSON: result = Type.GetType("string"); break;
                default:
                    break;
            }

            return result;
        }

        public static bool SqlTypeNeedsQuotes(DataType type) => type >= DataType.Date;
        /*{
            bool result = false;

            if(type < DataType.Date) 
            switch (type)
            {
                case DataType.Int: case DataType.TinyInt: case DataType.SmallInt: case DataType.MediumInt: case DataType.BigInt:
                case DataType.Bit: case DataType.Bool:
                case DataType.Real: case DataType.Double: case DataType.Float: case DataType.Decimal: case DataType.Numeric: result = false; break;
                case DataType.Date: case DataType.Time: case DataType.TimeStamp: case DataType.DateTime: case DataType.Year:
                case DataType.Char: case DataType.Varchar: case DataType.Binary: case DataType.Varbinary: case DataType.TinyBlob:
                case DataType.Blob: case DataType.MediumBlob: case DataType.LongBlob:
                case DataType.TinyText: case DataType.Text: case DataType.MediumText: case DataType.LongText:
                case DataType.Enum: case DataType.Set: case DataType.JSON: result = true; break;
                default: break;
            }

            return result;
        }*/

        [Flags]
        public enum ColumnFlags
        {
            NotNull = 1,
            AutoIncrement = 2,
            Unique = 4,
            PrimaryKey = 8
        }

        public int Index = -1;
        public Table Parent;
        public string Name = "";
        public DataType Data_Type = DataType.Int;
        public int Size = -1;

        public object Default = null;
        //bool NotNull = false;
        //bool Unique = false;
        public ColumnFlags Properties;
        public Enumeration EnumSymbols = new Enumeration();

        public Column(string name, DataType type)
        {
            Name = name;
            Data_Type = type;
        }

        public string PrepareCreateStatement()
        {
            string typestr = Data_Type != DataType.Enum ? Data_Type.ToString() : EnumSymbols.PrepareCreateStatement();
            return string.Format("`{0}` {1}{2} {3}{4}{5}{6}", Name, typestr, Size != -1 ? string.Format("({0})", Size) : "",
            (Properties & ColumnFlags.NotNull) != 0 ? "NOT NULL " : "",
            (Default != null) ? string.Format("DEFAULT {0} ", /*Default is string ? $"'{Default}'" :*/ Default) : "",
            (Properties & ColumnFlags.AutoIncrement) != 0 ? "AUTO_INCREMENT " : "",
            (Properties & ColumnFlags.PrimaryKey) == ColumnFlags.PrimaryKey ? "PRIMARY KEY" :
            (Properties & ColumnFlags.Unique) != 0 ? "UNIQUE" : "");
        }

        public Column Copy()
        {
            Column c = new Column(Name, Data_Type);
            c.Index = this.Index;
            c.Parent = this.Parent;
            c.Size = this.Size;
            c.Default = this.Default;
            c.Properties = this.Properties;
            c.EnumSymbols = this.EnumSymbols.Copy();
            return c;
        }

        private string DebuggerDisplay
        {
            get { return string.Format("{0} : {1} | {2}{3} {4}", Index, Name, Data_Type, (Size != -1) ? $"({Size})" : "", Properties); }
        }
    }

    public class ForeignKey
    {
        public enum RefAction
        {
            Cascade,
            SetNull,
            Restrict,
            NoAction,
            SetDefault
        }

        public static string ActionToSQL(RefAction r)
        {
            string result = "";
            switch(r)
            {
                case RefAction.Cascade: result = "CASCADE"; break;
                case RefAction.SetNull: result = "SET NULL"; break;
                case RefAction.Restrict: result = "RESTRICT"; break;
                case RefAction.NoAction: result = "NO ACTION"; break;
                case RefAction.SetDefault: result = "SET DEFAULT"; break;
            }
            return result;
        }

        public Column ConstrainedCol { get; private set; }
        public Column BaseCol { get; private set; }

        public RefAction OnDelete { get; set; }
        public RefAction OnUpdate { get; set; }

        public ForeignKey(Column pConstr, Column pBase, RefAction pOnDelete = RefAction.Restrict, RefAction pOnUpdate = RefAction.Restrict)
        {
            ConstrainedCol = pConstr;
            BaseCol = pBase;
            OnDelete = pOnDelete;
            OnUpdate = pOnUpdate;
        }

        public string PrepareStatement() => (ConstrainedCol != null && BaseCol != null) ? 
            string.Format("FOREIGN KEY ({0}) REFERENCES {1}({2}) {3} {4}", 
            ConstrainedCol.Name, BaseCol.Parent.Name, BaseCol.Name, 
            OnDelete!=RefAction.Restrict ? $"ON DELETE {ActionToSQL(OnDelete)}" : "",
            OnUpdate!=RefAction.Restrict ? $"ON UPDATE {ActionToSQL(OnUpdate)}" : "") : "";
    }

    public class UniqueKey
    {
        string Name;
        public List<Column> Cols { get; private set; } = new List<Column>();

        public UniqueKey(string pName, IEnumerable<Column> pCols) { Name = pName; Cols.AddRange(pCols); }

        public string PrepareStatement()
        {
            string result = "";
            if (Cols.Count > 0)
            {
                result = string.Format("UNIQUE KEY {0} (", Name);
                foreach (Column c in Cols) result += c.Name + (c != Cols.Last() ? ", " : "");
                result += ")";
            }
            return result;
        }
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; private set; } = new List<Column>();
        public DatabaseMySQL DB { get; private set; } = null;

        private Column primarykey = null;
        public Column PrimaryKey
        {
            get => primarykey;
            set
            {
                if (Columns.Find(x => x == value) != null) primarykey = value;
            }
        }

        public List<ForeignKey> ForeignKeys { get; private set; } = new List<ForeignKey>();
        public List<UniqueKey> UniqueKeys { get; private set; } = new List<UniqueKey>();

        public Table(string name) => Name = name;
        public Table(string name, DatabaseMySQL db) { Name = name; DB = db; }

        public Column this[string name]
        {
            get => Columns.Find(x => x.Name == name);
            set { Column c = Columns.Find(x => x.Name == name); if (c != null) c = value; }
        }

        public Column AddColumn(string pName, Column.DataType pDataType, Column.ColumnFlags pFlags = 0, int pSize = -1)
        {
            if (Columns.Find(x => x.Name == pName) != null) return null;

            Column newcol = new Column(pName, pDataType);
            newcol.Properties = pFlags;
            newcol.Size = (pDataType == Column.DataType.Varchar && pSize==-1 ? 255 : pSize);
            newcol.Parent = this;
            newcol.Index = Columns.Count;

            Columns.Add(newcol);
            if ((pFlags & Column.ColumnFlags.PrimaryKey) == Column.ColumnFlags.PrimaryKey) PrimaryKey = newcol;

            return newcol;
        }

        public void AddForeignKey(ForeignKey fk)
        {
            if (fk.ConstrainedCol.Parent != this) throw new ApplicationException(string.Format("Foreign Key constrained column does not belong to table {0}", this.Name));
            else ForeignKeys.Add(fk);
        }

        public void AddForeignKey(string pColumn, Column pBase, ForeignKey.RefAction pOnDelete = ForeignKey.RefAction.Restrict, ForeignKey.RefAction pOnUpdate = ForeignKey.RefAction.Restrict)
        {
            Column col = this[pColumn];
            if (col == null) throw new ApplicationException($"Column `{this.Name}.{pColumn}` not found! Unable to add foreign key.");
            ForeignKey key = new ForeignKey(col, pBase, pOnDelete, pOnUpdate);
            ForeignKeys.Add(key);
        }

        public bool Create(DatabaseMySQL db)
        {
            if (db == null) throw new ArgumentNullException("db");

            string query = string.Format("CREATE TABLE IF NOT EXISTS {0} (", Name);
            foreach (Column c in Columns) query += c.PrepareCreateStatement() + (c != Columns.Last() ? ", " : "");
            if (ForeignKeys.Count > 0)
            {
                query += ", ";
                foreach (ForeignKey k in ForeignKeys) query += k.PrepareStatement() + (k != ForeignKeys.Last() ? ", " : "");
            }
            if (UniqueKeys.Count > 0)
            {
                query += ", ";
                foreach (UniqueKey uk in UniqueKeys) query += uk.PrepareStatement() + (uk != UniqueKeys.Last() ? ", " : "");
            }
            query += ")";

            MySqlCommand cmd = new MySqlCommand(query, db.connection);
            int result = cmd.ExecuteNonQuery();

            return true;
        }

        public int Insert(Record r) => Insert(r, this.DB);
        public int Insert(Record r, DatabaseMySQL db)
        {
            int result = -1;
            if (r == null) return -1;

            if (r.Values.Count != this.Columns.Count) throw new AVMySqlException($"Column count mismatch in Insert for table {Name}");

            string columns = "";
            string values = "";
            int ctr = 0;
            foreach (Column c in Columns)
            {
                if (r.FieldIgnored(c.Name)) continue;
                if ((c.Properties & Column.ColumnFlags.PrimaryKey) == Column.ColumnFlags.PrimaryKey &&
                    (c.Properties & Column.ColumnFlags.AutoIncrement) == Column.ColumnFlags.AutoIncrement) { ctr++; continue; }
                string separator = (c != Columns.Last() ? ", " : "");
                columns += c.Name + separator;
                if (!Column.SqlTypeNeedsQuotes(c.Data_Type))
                {
                    string val = r[c.Name]?.ToString() ?? "NULL";
                    if (val.Length == 0) val = "''";
                    values += string.Format("{0}{1}", val, separator); // w/o quotes
                }
                else values += string.Format("'{0}'{1}", (r[c.Name]?.ToString() ?? ""), separator);
                ctr++;
            }

            string query = $"INSERT INTO {Name} ({columns}) VALUES ({values}); SELECT LAST_INSERT_ID();";

            using (MySqlCommand cmd = new MySqlCommand(query, db.connection)) result = Convert.ToInt32((ulong)(cmd.ExecuteScalar() ?? -1)); 

            return result;
        }

        public int Insert(IEnumerable<Record> r) => Insert(r, this.DB);
        public int Insert(IEnumerable<Record> r, DatabaseMySQL db)
        {
            if (r.Count() == 0) return -1;

            int result = -1;

            if (r.First().Values.Count != this.Columns.Count) throw new AVMySqlException($"Column count mismatch in Insert for table {Name}");

            string columns = "";
            string values = "";
            int ctr = 0;

            // Columns and values are handled separately in this case
            foreach (Column c in Columns)
            {
                if ((c.Properties.HasFlag(Column.ColumnFlags.PrimaryKey))) { ctr++; continue; }
                string separator = (c != Columns.Last() ? ", " : "");
                columns += c.Name + separator;
                ctr++;
            }

            ctr = 0;
            foreach(Record record in r)
            {
                values += "(";
                foreach (Column c in Columns)
                {
                    if (record.FieldIgnored(c.Name)) continue;
                    if ((c.Properties & Column.ColumnFlags.PrimaryKey) == Column.ColumnFlags.PrimaryKey &&
                        (c.Properties & Column.ColumnFlags.AutoIncrement) == Column.ColumnFlags.AutoIncrement) { ctr++; continue; }
                    string separator = (c != Columns.Last() ? ", " : "");
                    if (!Column.SqlTypeNeedsQuotes(c.Data_Type)) values += string.Format("{0}{1}", (record[c.Name]?.ToString() ?? "NULL"), separator); // w/o quotes
                    else values += string.Format("'{0}'{1}", (record[c.Name]?.ToString() ?? ""), separator);
                    ctr++;
                }
                string separator_r = (record != r.Last() ? ", " : "");
                values += $"){separator_r}";
            }

            string query = $"INSERT INTO {Name} ({columns}) VALUES {values}; SELECT LAST_INSERT_ID()";

            using (MySqlCommand cmd = new MySqlCommand(query, db.connection)) result = Convert.ToInt32((ulong)(cmd.ExecuteScalar() ?? -1));

            return result;
        }

        public string GetSelectQuery(string condition="", IEnumerable<Column> columns = null)
        {
            string query = "SELECT ";
            if (columns != null)
            {
                foreach (Column c in columns)
                {
                    query += c.Name + (c != columns.Last() ? ", " : "");
                }
            }
            else query += "*";

            query += string.Format(" FROM {0} {1}", this.Name, (condition.Length > 0) ? $"WHERE {condition}" : "");

            return query;
        }

        public Record[] Select(string condition="", IEnumerable<Column> columns=null)
        {
            List<Record> result = new List<Record>();

            using (MySqlCommand cmd = new MySqlCommand(GetSelectQuery(condition, columns), DB.connection))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Record r = new Record(this);
                    //for (int i = 0; i < reader.FieldCount; i++) r.Values.Add(reader.GetValue(i));
                    for (int i = 0; i < reader.FieldCount; i++) r.Values[i].Value = (reader.GetValue(i) is DBNull) ? null : reader.GetValue(i);
                    result.Add(r);
                }
            }
            return result.ToArray();
        }

        public Record SelectOne(string condition="", IEnumerable<Column> columns=null)
        {
            Record result = null;

            string query = GetSelectQuery(condition, columns) + " LIMIT 1";
            using (MySqlCommand cmd = new MySqlCommand(query, DB.connection))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    reader.Read();
                    result = new Record(this);
                    for (int i = 0; i < reader.FieldCount; i++) result.Values[i].Value = (reader.GetValue(i) is DBNull) ? null : reader.GetValue(i);
                }
            }

            return result;
        }

        // TODO: Fix column interpretation
        public Record[] SelectCustom(string condition, string fields)
        {
            List<Record> result = new List<Record>();
            string query = string.Format("SELECT {0} FROM {1}{2}",fields, this.Name, (condition.Length>0) ? $" WHERE {condition}" : "");

            using (MySqlCommand cmd = new MySqlCommand(query, DB.connection))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Record r = new Record(this);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string name = reader.GetName(i);
                        r[reader.GetName(i)] = reader.GetValue(i);
                    }
                    result.Add(r);
                }
            }
            return result.ToArray();
        }

        public object SelectMaxFieldValue(Column c, string condition="")
        {
            object result = null;
            using (MySqlCommand cmd = new MySqlCommand($"SELECT max({c.Name}) FROM {this.Name}{(condition.Length>0 ? $" WHERE {condition}" : "")}", DB.connection))
            {
                result = cmd.ExecuteScalar();
                if (result is DBNull) result = null;
            }
            return result;
        }

        public int Update(string condition, Record r)
        {
            int result = -1;
            string colvalues = "";

            foreach(RecordValue rval in r.Values)
            {
                Column col = rval.GetColumn();
                if (col == null) continue;
                string q = Column.SqlTypeNeedsQuotes(col.Data_Type) ? "'" : "";
                //string separator = rval != r.Values.Last() ? ", " : "";
                string separator = (colvalues.Length!=0) ? ", " : "";
                if (!rval.Ignore) colvalues += $"{separator}{col.Name}={q}{(rval.Value == null ? "NULL" : rval.Value)}{q}";
            }

            string query = $"UPDATE {Name} SET {colvalues} {(condition.Length > 0 ? string.Format("WHERE {0}", condition) : "")}";

            using (MySqlCommand cmd = new MySqlCommand(query, DB.connection)) result = cmd.ExecuteNonQuery();
            return result;
        }

        public void Delete(string condition)
        {
            if (condition == null || condition.Length == 0) throw new AVMySqlException("Empty condition in Table.Delete(), use DeletaAll() instead if you want delete all records!");
            string query = $"DELETE FROM {this.Name} WHERE {condition}";
            using (MySqlCommand cmd = new MySqlCommand(query, DB.connection)) cmd.ExecuteNonQuery();
        }

        public void DeleteAll()
        {
            string query = string.Format("SET FOREIGN_KEY_CHECKS = 0; DELETE IGNORE FROM {0}; SET FOREIGN_KEY_CHECKS = 1;", Name);

            using (MySqlCommand cmd = new MySqlCommand(query, DB.connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void RenameColumn(string oldname, string newname)
        {
            Column c = Columns.Find(x => x.Name == oldname);
            if (c == null) return;
            Column newc = c.Copy();
            newc.Name = newname;

            string query = $"ALTER TABLE {this.Name} CHANGE COLUMN `{oldname}` {newc.PrepareCreateStatement()}";

            using (MySqlCommand cmd = new MySqlCommand(query, DB.connection))
            {
                cmd.ExecuteNonQuery();
            }

            c.Name = newname;
        }

        public void AlterColumn(string column, Column new_specification)
        {
            int index = Columns.FindIndex(x => x.Name == column);
            if (index == -1) throw new AVMySqlException($"Column '{column}' not found for AlterColumn()");

            string query = $"ALTER TABLE {this.Name} CHANGE COLUMN `{column}` {new_specification.PrepareCreateStatement()}";

            using (MySqlCommand cmd = new MySqlCommand(query, DB.connection))
            {
                cmd.ExecuteNonQuery();
            }

            if (new_specification.Parent != this) new_specification.Parent = this;
            new_specification.Index = Columns[index].Index;
            Columns[index] = new_specification;
        }

        public void AlterAddColumn(string column)
        {
            Column c = Columns.Find(x => x.Name == column);
            if (c == null) throw new AVMySqlException($"Column '{column}' not found for AlterAddColumn()");

            string query = $"ALTER TABLE {this.Name} ADD COLUMN {c.PrepareCreateStatement()}";

            using (MySqlCommand cmd = new MySqlCommand(query, DB.connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void AlterRemoveColumn(string column)
        {
            Column c = Columns.Find(x => x.Name == column);
            if (c == null) throw new AVMySqlException($"Column '{column}' not found for AlterRemoveColumn()");

            string query = $"ALTER TABLE {this.Name} DROP COLUMN `{column}`";

            using (MySqlCommand cmd = new MySqlCommand(query, DB.connection))
            {
                cmd.ExecuteNonQuery();
            }

            Columns.Remove(c);
        }

        public void SyncPrimaryKey()
        {
            Column pk = Columns.Find(x => (x.Properties & Column.ColumnFlags.PrimaryKey) == Column.ColumnFlags.PrimaryKey);
            PrimaryKey = pk;
        }

        private string DebuggerDisplay
        {
            get { return string.Format("{0} : {1} columns | PK:'{2}' | FK #:{3} | UK #:{4}", Name, 
                Columns.Count, PrimaryKey?.Name ?? "not set", ForeignKeys.Count(), UniqueKeys.Count()); }
        }
    }

    public class DatabaseMySQL : IDisposable
    {
        public List<Table> Tables { get; private set; } = new List<Table>();

        public string Host;
        public ushort Port;
        public string User;
        public string Password;
        public string Database;

        private string connString;

        public bool Connected = false;

        public MySqlConnection connection { get; private set; } = null;
        public string Error { get; private set; }

        public DatabaseMySQL() { }

        public DatabaseMySQL(string pHost, ushort pPort, string pUser, string pPassword, string pDatabase)
        {
            Host = pHost;
            Port = pPort;
            User = pUser;
            Password = pPassword;
            Database = pDatabase;

            Connect();
        }

        protected void ComposeConnString()
        {
            connString = $"server={Host};user={User};database={Database};port={Port};password={Password};SSL Mode=none;CharSet=utf8";
        }

        public bool Connect(bool rebuildschema = true)
        {
            bool result = true;
            if (Connected) return result;

            ComposeConnString();

            try
            {
                connection = new MySqlConnection(connString);
                connection.Open();
                Error = "";
                Connected = true;

                if (rebuildschema) RebuildSchema();
            }
            catch (Exception e)
            {
                //Console.WriteLine($"Exception in MySqlConnection: {e}");
                Error = e.Message;
                result = false;
            }

            return result;
        }

        public void Disconnect()
        {
            connection.Close();
            Connected = false;
        }

        public void RestoreConnection()
        {
            //if (Connected && connection.State != System.Data.ConnectionState.Open) 
            Disconnect();
            Connect(false);
        }

        public bool TableExists(string name)
        {
            bool result = false;
            using (MySqlCommand check = new MySqlCommand($"show tables like '{name}'", connection)) result = check.ExecuteScalar() != null;
            return result;
        }

        public bool TableIsEmpty(Table t)
        {
            bool result = true;
            using (MySqlCommand check = new MySqlCommand(string.Format("SELECT NULL FROM {0} LIMIT 1", t.Name), connection)) result = check.ExecuteScalar() == null;
            return result;
        }

        public bool CreateTable(Table t)
        {
            return t.Create(this);
        }

        public List<string> GetTableNameList(string criteria = "")
        {
            List<string> result = new List<string>();

            string query = string.Format("SHOW TABLES{0}", criteria.Length > 0 ? $" LIKE '{criteria}'" : "");
            MySqlCommand cmd = new MySqlCommand("show tables", connection);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }

            return result;
        }

        public Table TableFromSchema(string name)
        {
            Table result = new Table(name, this);

            string col_query = string.Format("SHOW COLUMNS FROM {0}.{1}", this.Database, name);

            //Regex r = new Regex(@"(");
            //string s; s.Split()

            int ctr = 0;
            MySqlExecuteReader(connection, col_query, rdr =>
            {
                //Column c = new Column()
                string colname = rdr.GetString(0);
                string tpname = rdr.GetString(1); string[] tpsplit = tpname.Split(new char[] { '(', ')', ',' });
                Column.DataType tp = (Column.DataType)Enum.Parse(typeof(Column.DataType), tpsplit[0], true);
                Column c = new Column(colname, tp);
                c.Index = ctr;
                if (rdr.GetString(2) == "NO") c.Properties |= Column.ColumnFlags.NotNull;
                if (rdr.GetString(3)?.Contains("PRI") ?? false) c.Properties |= Column.ColumnFlags.PrimaryKey;
                if (rdr.GetString(5)?.Contains("auto_increment") ?? false) c.Properties |= Column.ColumnFlags.AutoIncrement;
                if (tpsplit.Count() > 1) // size or enum
                {
                    if(tp != Column.DataType.Enum) c.Size = int.Parse(tpsplit[1]);
                    else // enum values
                    {
                        int limit = tpsplit.Count() - 1;
                        for (int i = 1; i < limit; i++) if (tpsplit.Length > 0) c.EnumSymbols.Symbols.Add(tpsplit[i].Replace("'", ""));
                    }
                }

                c.Default = (rdr.IsDBNull(4)) ? "null" : rdr.GetString(4);

                c.Parent = result;
                result.Columns.Add(c);
                ctr++;
                return 0;
            });

            result.SyncPrimaryKey();

            return result;
        }

        public void DropAllTables()
        {
            if (!Connected) return;
            if (Tables.Count == 0) return;

            string query = "SET FOREIGN_KEY_CHECKS = 0; ";
            foreach(Table t in Tables)
            {
                query += string.Format("DROP TABLE IF EXISTS {0}; ", t.Name, Database);
            }
            query += "SET FOREIGN_KEY_CHECKS = 1;";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.ExecuteNonQuery();
            }

            Tables.Clear();
        }

        public void DropTable(Table t)
        {
            if (!Connected) return;
            if (t==null || Tables.Count == 0) return;

            string query = string.Format("DROP TABLE IF EXISTS {0}; ", t.Name);
            using (MySqlCommand cmd = new MySqlCommand(query, connection)) cmd.ExecuteNonQuery();
        }

        public void RenameTable(Table t, string newname)
        {
            if (!Connected) return;

            string query = string.Format("RENAME TABLE {0} TO {1}; ", t.Name, newname);
            using (MySqlCommand cmd = new MySqlCommand(query, connection)) cmd.ExecuteNonQuery();
            t.Name = newname;
        }

        public void RebuildSchema()
        {
            if (!Connected) return;
            Tables.Clear();

            List<string> tables = GetTableNameList();
            foreach (string tname in tables) Tables.Add(TableFromSchema(tname));

            foreach (Table t in Tables) RebuildForeignUniqueKeys(t);
        }

        public void RebuildForeignUniqueKeys(Table t)
        {
            Regex fk = new Regex(@"FOREIGN KEY \(`\b\w+\b`\) REFERENCES `\b\w+\b` \(`\b\w+\b`\)", RegexOptions.Compiled | RegexOptions.Multiline);
            Regex uk = new Regex(@"UNIQUE KEY `\b\w+\b` \(.*\)", RegexOptions.Compiled | RegexOptions.Multiline);
            Regex fk_value = new Regex(@"`\b\w+\b`", RegexOptions.Compiled);
            //fk.Split

            MySqlExecuteReader(connection, $"show create table {t.Name}", rdr =>
            {
                string create_query = rdr.GetString(1);
                MatchCollection matches = fk.Matches(create_query);

                foreach (Match match in matches)
                {
                    string fkline = create_query.Substring(match.Index, match.Length);
                    MatchCollection valmatches = fk_value.Matches(fkline);

                    if (valmatches.Count >= 3)
                    {
                        //Column c = t[valmatches[0].Value.Replace("`","")];
                        //Table t2 = this[valmatches[1].Value.Replace("`", "")];
                        //Column c2 = t2[valmatches[2].Value.Replace("`", "")];
                        ForeignKey k = new ForeignKey(t[valmatches[0].Value.Replace("`", "")], 
                            this[valmatches[1].Value.Replace("`", "")][valmatches[2].Value.Replace("`", "")]);
                        t.AddForeignKey(k);
                    }
                }

                MatchCollection umatches = uk.Matches(create_query);
                foreach(Match umatch in umatches)
                {
                    string ukline = create_query.Substring(umatch.Index, umatch.Length);
                    MatchCollection uvalmatches = fk_value.Matches(ukline);

                    if(uvalmatches.Count>0)
                    {
                        List<Column> ukcols = new List<Column>();
                        for (int i = 1; i < uvalmatches.Count; i++)
                        {
                            Column c = t[uvalmatches[i].Value.Replace("`", "")];
                            if (c != null) ukcols.Add(c);
                        }
                        UniqueKey ukey = new UniqueKey(uvalmatches[0].Value.Replace("`", ""), ukcols);
                        t.UniqueKeys.Add(ukey);
                    }
                }

                return 0;
            });
        }

        public static void MySqlExecuteReader(MySqlConnection conn, string query, Func<MySqlDataReader, int> read)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    read(reader);
                }
            }
        }

        public Table this[string name]
        {
            get => Tables.Find(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)); // ignore case
            set { Table t = Tables.Find(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)); if (t != null) t = value; }
        }

        /*public List<Record> QueryRaw(string query)
        {
            List<Record> result = new List<Record>();

            if (!Connected) return result;

            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
																																								
            {
                Record r = new Record();
                //for (int i = 0; i < reader.FieldCount; i++) r.Values.Add(reader.GetValue(i));
                for (int i = 0; i < reader.FieldCount; i++) r.Values[i] = reader.GetValue(i);

                result.Add(r);
            }

            return result.ToArray();
        }*/

        /*public List<string> GetTableNameList(string criteria="")
        {
            List<string> result = new List<string>();

            string query = string.Format("SHOW TABLES{0}", criteria.Length > 0 ? $" LIKE {criteria}" : "");
            MySqlCommand cmd = new MySqlCommand("show tables", connection);
            MySqlDataReader reader = cmd.ExecuteReader();
	  
		 

            string temp;
            while (reader.Read())
            {
                result.Add(reader.GetString(0));
            }

            return result;
        }*/
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (connection != null) connection.Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MySqlInterface() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    } 
}

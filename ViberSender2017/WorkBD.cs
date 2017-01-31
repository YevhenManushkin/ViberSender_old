namespace ViberSender2017
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public static class WorkBD
    {
        private static BindingSource bs = new BindingSource();
        private static SQLiteConnection connection = null;
        private static DataTable dt = new DataTable();
        private static SQLiteFactory factory = null;
        private static string path_config = (Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates)) + @"Users\" + Environment.UserName + @"\AppData\Roaming\ViberPC\config.db");
        private static SQLiteDataAdapter sql = new SQLiteDataAdapter();

        public static void ClearHistory(string phone)
        {
            try
            {
                string[] textArray1 = new string[] { "Data Source = ", path_config.Replace("config.db", ""), @"\", phone.Replace("+", ""), @"\viber.db" };
                connection.ConnectionString = string.Concat(textArray1);
                connection.Open();
                new SQLiteCommand(connection) { 
                    CommandText = "delete from Messages",
                    CommandType = CommandType.Text
                }.ExecuteNonQuery();
                connection.Close();
            }
            catch
            {
            }
        }

        public static BindingSource DownloadBD()
        {
            try
            {
                DataColumn column = new DataColumn {
                    AutoIncrementSeed = 1L,
                    AutoIncrement = true,
                    ColumnName = "№"
                };
                dt.Columns.Add(column);
                factory = (SQLiteFactory) DbProviderFactories.GetFactory("System.Data.SQLite");
                connection = (SQLiteConnection) factory.CreateConnection();
                connection.ConnectionString = "Data Source = " + path_config;
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection) {
                    CommandText = "SELECT * FROM Accounts",
                    CommandType = CommandType.Text
                };
                command.ExecuteNonQuery();
                sql.SelectCommand = command;
                sql.Fill(dt);
                bs.DataSource = dt;
                connection.Close();
            }
            catch
            {
            }
            return bs;
        }

        [DllImport("user32.dll", SetLastError=true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        public static void OffAccs()
        {
            try
            {
                connection.ConnectionString = "Data Source = " + path_config;
                connection.Open();
                new SQLiteCommand(connection) { 
                    CommandText = "UPDATE 'Accounts' SET 'IsValid'='0'",
                    CommandType = CommandType.Text
                }.ExecuteNonQuery();
                connection.Close();
            }
            catch
            {
            }
        }

        public static int NextAcc()
        {
            int result_index = 0;
            List<string> all_acc = new List<string>();
            try
            {
                connection.ConnectionString = "Data Source = " + path_config;
                connection.Open();
                SQLiteDataReader reader;
                SQLiteCommand command = new SQLiteCommand(connection)
                {
                    CommandText = "SELECT * FROM Accounts",
                    CommandType = CommandType.Text
                };
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    all_acc.Add(reader.GetString(0).ToString().Trim());
                }
                reader.Close();

                string current_id = String.Empty;
                command.CommandText = "SELECT * FROM Accounts WHERE isDefault = '1'";
                command.CommandType = CommandType.Text;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    current_id = reader.GetString(0).ToString().Trim();
                }
                reader.Close();

                int index = 0;
                for (int i = 0; i < all_acc.Count; i++)
                {
                    if (all_acc[i] == current_id)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == all_acc.Count - 1)
                {
                    SQLiteCommand isDefault0 = new SQLiteCommand("UPDATE Accounts  SET isDefault = 0 WHERE isDefault = 1", connection);
                    isDefault0.ExecuteNonQuery();
                    SQLiteCommand isDefault1 = new SQLiteCommand("UPDATE Accounts  SET isDefault = 1 WHERE ID = " + all_acc[0], connection);
                    isDefault1.ExecuteNonQuery();
                    result_index = 0;
                }
                else
                {
                    SQLiteCommand isDefault0 = new SQLiteCommand("UPDATE Accounts  SET isDefault = 0 WHERE isDefault = 1", connection);
                    isDefault0.ExecuteNonQuery();
                    SQLiteCommand isDefault1 = new SQLiteCommand("UPDATE Accounts  SET isDefault = 1 WHERE ID = " + all_acc[index + 1], connection);
                    isDefault1.ExecuteNonQuery();
                    result_index = index + 1;
                }
                connection.Close();
            }
            catch
            {
            }
            return result_index;
        }

        public static string CurAcc()
        {
            string result = String.Empty;
            try
            {
                connection.ConnectionString = "Data Source = " + path_config;
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection)
                {
                    CommandText = "SELECT * FROM Accounts WHERE isDefault = '1'",
                    CommandType = CommandType.Text
                };
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = reader.GetString(0).ToString().Trim();
                }
                reader.Close();
                connection.Close();
            }
            catch
            {
            }
            return result;
        }

        public static void SetAcc(string phone)
        {
            try
            {
                connection.ConnectionString = "Data Source = " + path_config;
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection) {
                    CommandText = "UPDATE 'Accounts' SET 'IsDefault'='0'",
                    CommandType = CommandType.Text
                };
                command.ExecuteNonQuery();
                command.CommandText = "UPDATE \"main\".\"Accounts\" SET \"IsDefault\"=1 WHERE \"ID\"=\"" + phone + "\"";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch
            {
            }
        }

        public static void SetLanguage(string phone)
        {
            try
            {
                string[] textArray1 = new string[] { "Data Source = ", path_config.Replace("config.db", ""), @"\", phone.Replace("+", ""), @"\viber.db" };
                connection.ConnectionString = string.Concat(textArray1);
                connection.Open();
                new SQLiteCommand(connection) { 
                    CommandText = "UPDATE \"main\".\"Settings\" SET \"SettingValue\" = 'ru' WHERE \"SettingTitle\" = 'UILanguage'",
                    CommandType = CommandType.Text
                }.ExecuteNonQuery();
                connection.Close();
            }
            catch
            {
            }
        }

        public static BindingSource WaitNewAcc()
        {
            connection.ConnectionString = "Data Source = " + path_config;
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection) {
                CommandText = "SELECT COUNT(*) FROM 'Accounts'",
                CommandType = CommandType.Text
            };
            int num = Convert.ToInt32(command.ExecuteScalar());
            while (FindWindow("Qt5QWindowOwnDCIcon", null) == IntPtr.Zero)
            {
                Thread.Sleep(100);
            }
            while (FindWindow("Qt5QWindowOwnDCIcon", null) != IntPtr.Zero)
            {
                Thread.Sleep(100);
                if (num < Convert.ToInt32(command.ExecuteScalar()))
                {
                    break;
                }
            }
            command.CommandText = "UPDATE 'Accounts' SET 'IsValid'='1'";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            command.CommandText = "SELECT * FROM Accounts";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            dt.Clear();
            sql.SelectCommand = command;
            sql.Fill(dt);
            bs.DataSource = dt;
            connection.Close();
            return bs;
        }
    }
}


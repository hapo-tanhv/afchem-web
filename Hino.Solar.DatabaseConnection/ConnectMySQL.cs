using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Hino.Solar.DatabaseConnection
{
    public class ConnectMySQL
    {
        public string ConnectionString { get; set; }
        #region Query
        public int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = commandType;
                    cmd.CommandText = query;

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // Thuc hien cac cau lenh khong tra ve du lieu  (co tham so truyen vao), Vidu: Insert, Update, Delete...
        public int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text, params object[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = commandType;
                    cmd.CommandText = query;

                    string[] querySplit = query.Split(' ');
                    string[] paramNames = querySplit.Where(x => x.StartsWith("@")).ToArray();

                    if (parameters.Length == paramNames.Length)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(paramNames[i], parameters[i]);
                        }
                        return cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        public DataTable ExecuteQuery(string query)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                    using (var dataAdapter = new SqlDataAdapter(cmd))
                    {
                        var dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }
        #endregion
    }
}

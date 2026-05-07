using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Hino.DatabaseConnector
{
    public class MySQLConnect
    {
        public string ConnectionString { get; set; }

        #region QUERY
        // Thuc hien cac cau lenh khong tra ve du lieu, Vidu: Insert, Update, Delete...
        public int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand())
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
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new MySqlCommand())
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
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                    using (var dataAdapter = new MySqlDataAdapter(cmd))
                    {
                        var dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }
        public DataSet ExecuteQueryMulti(string query, CommandType commandType = CommandType.Text)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = commandType;
                    cmd.CommandText = query;

                    conn.Open();
                    using (var dataAdapter = new MySqlDataAdapter(cmd))
                    {
                        var dataSet = new DataSet();
                        dataAdapter.Fill(dataSet);
                        return dataSet;
                    }
                }
            }
        }
        public object ExecuteScalarQuery(string query, CommandType commandType = CommandType.Text)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = commandType;
                    cmd.CommandText = query;

                    return cmd.ExecuteScalar();
                }
            }
        }
        #endregion

        #region STORE PROCEDURE

        public DataTable ExecuteStoreProcedure(string procedureName, object model)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procedureName;

                    var parameters = GenerateParameters(model);
                    foreach (var parameter in parameters)
                        cmd.Parameters.Add(parameter);

                    using (var dataAdapter = new MySqlDataAdapter(cmd))
                    {
                        var dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }
        private List<MySqlParameter> GenerateParameters(object model)
        {
            var paramList = new List<MySqlParameter>();
            Type modelType = model.GetType();
            var properties = modelType.GetProperties();
            foreach (var property in properties)
            {
                if (property.GetValue(model) == null)
                {
                    paramList.Add(new MySqlParameter(property.Name, DBNull.Value));
                }
                else
                {
                    paramList.Add(new MySqlParameter(property.Name, property.GetValue(model)));
                }
            }
            return paramList;

        }
        #endregion
    }
}

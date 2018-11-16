using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolTest
{
    class DBConnect
    {
        string Server = string.Empty;
        string ID = string.Empty;
        string Pass = string.Empty;
        string DBName = string.Empty;
        public string TextInputData()
        {
            string[] TextValue = System.IO.File.ReadAllLines(@"..\..\DBset.txt");
            if (TextValue.Length > 0)
            {
                for (int i = 0; i < TextValue.Length; i++)
                {
                    int index = TextValue[i].IndexOf(':');
                    TextValue[i] = TextValue[i].Substring(index + 1, TextValue[i].Length - index - 1);
                }
            }
            Server = TextValue[0];
            ID = TextValue[1];
            Pass = TextValue[2];
            DBName = TextValue[3];

            return DBName;
        }
        public string TextInputData2()
        {
            string[] TextValue = System.IO.File.ReadAllLines(@"..\..\DBset2.txt");
            if (TextValue.Length > 0)
            {
                for (int i = 0; i < TextValue.Length; i++)
                {
                    int index = TextValue[i].IndexOf(':');
                    TextValue[i] = TextValue[i].Substring(index + 1, TextValue[i].Length - index - 1);
                }
            }
            Server = TextValue[0];
            ID = TextValue[1];
            Pass = TextValue[2];
            DBName = TextValue[3];

            return DBName;
        }
        public DBConnect()
        {
            TextInputData();
           
        }
        public DataTable dbConn(string qry, string DB) // 디비 연결및  쿼리 전달
        {
            string connString;
            connString = String.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};Integrated Security=false; Pooling=false",
                                              Server, DB, ID, Pass);
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                string sql = qry;

                SqlDataAdapter da = new SqlDataAdapter();
                DataTable tbl = new DataTable();
                da.SelectCommand = new SqlCommand(sql, connection);
                try //추후 대체할것 찾아보기
                {    
                    da.Fill(tbl);
                }
                catch (SqlException)
                {
                    connection.Close();
                    da.Dispose();
                    return null;
                }
                connection.Close();
                da.Dispose();
                return tbl;
            }
        }

        public DataTable insertqry(string qry, string DB)
        {
            if (qry != null)
            {
                string connString;
                connString = String.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};Integrated Security=false",
                                                      Server, DB, ID, Pass);
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    string sql = qry;

                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable tbl = new DataTable();
                    da.InsertCommand = new SqlCommand(sql, connection);
                    da.InsertCommand.ExecuteNonQuery();
                    connection.Close();
                    da.Dispose();
                    return tbl;
                }
            }
            return null;
        }

        public DataTable Updateqry(string qry, string DB)
        {
            if (qry != null)
            {
                string connString;
                connString = String.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};Integrated Security=false",
                                                      Server, DB, ID, Pass);
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    string sql = qry;

                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable tbl = new DataTable();
                    da.UpdateCommand = new SqlCommand(sql, connection);
                    da.UpdateCommand.ExecuteNonQuery();
                    connection.Close();
                    da.Dispose();
                    return tbl;
                }
            }
            return null;
        }

        public void deleteqry(string qry, string DB)
        {
            string connString;
            connString = String.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};Integrated Security=false",
                                                  Server, DB, ID, Pass);
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                string sql = qry;

                SqlDataAdapter da = new SqlDataAdapter();
                DataTable tbl = new DataTable();
                da.DeleteCommand = new SqlCommand(sql, connection);
                da.DeleteCommand.ExecuteNonQuery();
                connection.Close();
                da.Dispose();
            }
        }

    }
}

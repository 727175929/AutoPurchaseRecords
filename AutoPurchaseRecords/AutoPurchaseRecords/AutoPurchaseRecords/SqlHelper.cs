using AutoPurchaseRecords.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutoPurchaseRecords
{
    public class SQLHelper
    {
        private string connStr;
        public string ConnStr
        {
            get
            {
                if (connStr == null) connStr = connStrFromXML("regedit.xml");
                return connStr;
            }
        }
        private SqlConnection connect = null;

        public SqlConnection getConnect()
        {
            connStr = connStrFromXML("regedit.xml");
            if (connect == null || connect.State == ConnectionState.Closed) connect = new SqlConnection(connStr);
            return connect;
        }

        /// <summary>
        /// 从xml文件读取连接字符串
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <returns></returns>
        private string connStrFromXML(string xmlpath)
        {
            string connStr = "", sname = "", dbname = "", user = "", pwd = "";
            var doc = new SafeXmlDocument();
            AES aes = new AES();
            try
            {
                doc.Load(xmlpath);
                foreach (XmlNode node in doc.ChildNodes)
                {
                    if (node.Name == "item")
                    {
                        foreach (XmlNode xn in node.ChildNodes)
                        {
                            if (xn.Name == "SName") sname = aes.Decrypt(xn.InnerText);
                            if (xn.Name == "DBName") dbname = aes.Decrypt(xn.InnerText);
                            if (xn.Name == "DBUser") user = aes.Decrypt(xn.InnerText);
                            if (xn.Name == "DBPass") pwd = aes.Decrypt(xn.InnerText);
                        }
                    }
                }
                connStr = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", sname, dbname, user, pwd);
            }
            catch (Exception e)
            {
                //throw e;
            }
            return connStr;
        }

        public int ExecuteNonQuery(string sql)
        {
            int result = -1;
            using (SqlConnection con = getConnect())
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                }
            }
            return result;
        }

        public DataTable ExecuteDataTable(string sql)
        {
            using (SqlConnection con = getConnect())
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if (ds.Tables.Count > 0)
                        return ds.Tables[0];
                    else
                        return null;
                }
            }
        }

        public SqlDataReader ExecuteReader(string sql)
        {
            SqlConnection con = getConnect();
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                con.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }

        }
        public object ExecuteScalar(string sql)
        {
            using (SqlConnection con = getConnect())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    return cmd.ExecuteScalar();
                }
            }
        }

        public DataSet ExecuteDataSet(string sql)
        {
            using (SqlConnection con = getConnect())
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
        }

    }
}

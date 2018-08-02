using DifPackage.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DifPackage.database
{
    class VersionDao
    {
        /*
         * 保存点名列表到数据库
         */
        public static void insertVersion(VersionInfo versionInfo)
        {
            object[] param = new object[2];
            param[0] = versionInfo.version;
            param[1] = versionInfo.path;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Package(");
            strSql.Append("version,path)");
            strSql.Append(" values (");
            strSql.Append("@version,@path)");

            int result = SQLiteDBHelper.ExecuteNonQuery(@"Data Source=" + "./Data/sqlite.data", strSql.ToString(), param);
            Console.WriteLine("插入版本：" + result);
        }

        /*
         * 根据特定条件获取点名信息
         */
        public static List<VersionInfo> getAllVersions()
        {
            List<VersionInfo> versionInfos = new List<VersionInfo>();

            string sql = "SELECT * FROM Package";

            IDataReader iDataReader = SQLiteDBHelper.ExecuteReader(@"Data Source=" + "./Data/sqlite.data", sql, null);

            if (iDataReader != null)
            {
                while (iDataReader.Read())
                {
                    string version = (string)iDataReader["version"];
                    string path = (string)iDataReader["path"];

                    VersionInfo versionInfo = new VersionInfo();
                    versionInfo.version = version;
                    versionInfo.path = path;

                    versionInfos.Add(versionInfo);
                }
            }
            return versionInfos;
        }
    }
}

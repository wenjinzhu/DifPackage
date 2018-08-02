using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DifPackage.util
{
    class MD5Util
    {
        /// <summary>  
        /// MD5加密  字符串
        /// </summary>  
        /// <param name="s">需要加密的字符串</param>  
        /// <returns></returns>  
        public static string EncryptMD5(string s)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var result = "";
            if (!string.IsNullOrWhiteSpace(s))
            {
                result = BitConverter.ToString(md5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(s.Trim())));
            }
            return result;
        }

        /// <summary>  
        /// MD5加密  文件
        /// </summary>  
        /// <param name="s">需要加密的文件路径</param>  
        /// <returns></returns> 
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}

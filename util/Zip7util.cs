using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DifPackage.util
{
    class Zip7util
    {
        /// <summary>
        /// 压缩ZIP文件
        /// 支持多文件和多目录，或是多文件和多目录一起压缩
        /// </summary>
        /// <param name="list">待压缩的文件或目录集合</param>
        /// <param name="strZipName">压缩后的文件名</param>
        /// <param name="isDirStruct">是否按目录结构压缩</param>
        /// <returns>成功：true/失败：false</returns>
        public static bool CompressMulti(List<string> list, string strZipName, bool isDirStruct)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (string.IsNullOrEmpty(strZipName))
            {
                throw new ArgumentNullException(strZipName);
            }
            try
            {
                //设置编码，解决压缩文件时中文乱码
                using (var zip = new Ionic.Zip.ZipFile(Encoding.UTF8))
                {
                    foreach (var path in list)
                    {
                        //取目录名称
                        var fileName = Path.GetFileName(path);
                        //如果是目录
                        if (Directory.Exists(path))
                        {
                            //按目录结构压缩
                            if (isDirStruct)
                            {
                                zip.AddDirectory(path, fileName);
                            }
                            else
                            {
                                //目录下的文件都压缩到Zip的根目录
                                zip.AddDirectory(path);
                            }
                        }
                        if (File.Exists(path))
                        {
                            zip.AddFile(path);
                        }
                    }
                    //压缩
                    zip.Save(strZipName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

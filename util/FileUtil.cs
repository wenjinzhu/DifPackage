using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DifPackage.util
{
    class FileUtil
    {
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <returns></returns>
        public static bool Exists(string filePath)
        {
            if (filePath == null || filePath.Trim() == "")
            {
                return false;
            }

            if (File.Exists(filePath))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <returns></returns>
        public static bool CreateDir(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return true;
        }


        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool CreateFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                    fs.Dispose();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 修改文件夹名称
        /// </summary>
        /// <param name="srcFolderPath">原文件全路径</param>
        /// <param name="destFolderPath">新文件全路径</param>
        /// <returns></returns>
        public static bool ModifyFolderName(string srcFolderPath, string destFolderPath)
        {
            try
            {
                if (System.IO.Directory.Exists(srcFolderPath))
                {
                    System.IO.DirectoryInfo folder = new System.IO.DirectoryInfo(srcFolderPath);
                    folder.MoveTo(destFolderPath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 读文件内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static string Read(string filePath, Encoding encoding)
        {
            if (!Exists(filePath))
            {
                return null;
            }
            //将文件信息读入流中
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return new StreamReader(fs, encoding).ReadToEnd();
            }
        }

        /// <summary>
        /// 读取文件的一行内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static string ReadLine(string filePath, Encoding encoding)
        {
            if (!Exists(filePath))
            {
                return null;
            }
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return new StreamReader(fs, encoding).ReadLine();
            }
        }


        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">文件内容</param>
        /// <returns></returns>
        public static bool Write(string filePath, string content)
        {
            if (!Exists(filePath) || content == null)
            {
                return false;
            }

            //将文件信息读入流中
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                lock (fs)//锁住流
                {
                    if (!fs.CanWrite)
                    {
                        return false;
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(content);
                    fs.Write(buffer, 0, buffer.Length);
                    return true;
                }
            }
        }


        /// <summary>
        /// 写入一行
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static bool WriteLine(string filePath, string content)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate | FileMode.Append))
            {
                lock (fs)
                {
                    if (!fs.CanWrite)
                    {
                        throw new System.Security.SecurityException("文件filePath=" + filePath + "是只读文件不能写入!");
                    }

                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(content);
                    sw.Dispose();
                    sw.Close();
                    return true;
                }
            }
        }


        public static bool CopyDir(DirectoryInfo fromDir, string toDir)
        {
            return CopyDir(fromDir, toDir, fromDir.FullName);
        }


        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="fromDir">被复制的目录路径</param>
        /// <param name="toDir">复制到的目录路径</param>
        /// <returns></returns>
        public static bool CopyDir(string fromDir, string toDir)
        {
            if (fromDir == null || toDir == null)
            {
                Console.WriteLine("参数为空");
                return false;
            }

            if (fromDir == toDir)
            {
                Console.WriteLine("目录相同");
                return false;
            }

            if (!Directory.Exists(fromDir))
            {
                Console.WriteLine("目录不存在");
                return false;
            }

            DirectoryInfo dir = new DirectoryInfo(fromDir);
            return CopyDir(dir, toDir, dir.FullName);
        }


        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="fromDir">被复制的目录路径</param>
        /// <param name="toDir">复制到的目录路径</param>
        /// <param name="rootDir">被复制的根目录路径</param>
        /// <returns></returns>
        private static bool CopyDir(DirectoryInfo fromDir, string toDir, string rootDir)
        {
            string filePath = string.Empty;
            foreach (FileInfo f in fromDir.GetFiles())
            {
                filePath = toDir + f.FullName.Substring(rootDir.Length);
                string newDir = filePath.Substring(0, filePath.LastIndexOf("\\"));
                CreateDir(newDir);
                File.Copy(f.FullName, filePath, true);
            }

            foreach (DirectoryInfo dir in fromDir.GetDirectories())
            {
                CopyDir(dir, toDir, rootDir);
            }

            return true;
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件的完整路径</param>
        /// <returns></returns>
        public static bool DeleteFile(string filePath)
        {
            if (Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }


        public static void DeleteDir(DirectoryInfo dir)
        {
            if (dir == null)
            {
                Console.WriteLine("目录不存在");
                return;
            }

            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                DeleteDir(d);
            }

            foreach (FileInfo f in dir.GetFiles())
            {
                DeleteFile(f.FullName);
            }

            dir.Delete();

        }

        /*
         * 删除目录下的所有文件
         */
        public static void DelectDir(string srcPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch
            {
                Console.WriteLine("删除文件失败");
            }
        }


        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dir">指定目录路径</param>
        /// <param name="onlyDir">是否只删除目录</param>
        /// <returns></returns>
        public static bool DeleteDir(string dir, bool onlyDir)
        {
            if (dir == null || dir.Trim() == "")
            {
                throw new NullReferenceException("目录dir=" + dir + "不存在");
            }

            if (!Directory.Exists(dir))
            {
                return false;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.GetFiles().Length == 0 && dirInfo.GetDirectories().Length == 0)
            {
                Directory.Delete(dir);
                return true;
            }


            if (!onlyDir)
            {
                return false;
            }
            else
            {
                DeleteDir(dirInfo);
                return true;
            }

        }


        /// <summary>
        /// 在指定的目录中查找文件
        /// </summary>
        /// <param name="dir">目录路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static bool FindFile(string dir, string fileName)
        {
            if (dir == null || dir.Trim() == "" || fileName == null || fileName.Trim() == "" || !Directory.Exists(dir))
            {
                return false;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            return FindFile(dirInfo, fileName);

        }

        public static bool CopyFile(string srcFile, string dstFile)
        {
            try
            {
                File.Copy(srcFile, dstFile, true);
                return true;
            }
            catch
            {
                Console.WriteLine("复制文件失败");
                return false;
            }
        }

        /*
         * 查找文件
         */
        public static bool FindFile(DirectoryInfo dir, string fileName)
        {
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                if (File.Exists(d.FullName + "\\" + fileName))
                {
                    return true;
                }
                FindFile(d, fileName);
            }

            return false;
        }

        /*
         * 读取txt内容
         */
        public static string ReadTxtMessage(string filePath)
        {
            StreamReader sr = new StreamReader(filePath, Encoding.Default);
            string content = "";
            StringBuilder stringBuilder = new StringBuilder();
            while ((content = sr.ReadLine()) != null)
            {
                stringBuilder.Append(content.ToString()).Append("\r\n");
            }

            return content;
        }

        /*
         * 获取目录下的所有子文件
         */
        public static List<string> Director(string dir, List<string> paths)
        {
            DirectoryInfo d = new DirectoryInfo(dir);
            FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                if (fsinfo is DirectoryInfo)
                {
                    Director(fsinfo.FullName, paths);
                }
                else
                {
                    paths.Add(fsinfo.FullName);
                }
            }

            return paths;
        }
    }
}

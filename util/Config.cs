using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DifPackage.util
{
    class Config
    {
        // 临时文件存储路径
        public static string tempPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Temp\\";
        // 全量包的存放路径
        public static string fullPackagePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\FullPackage\\";
        // 增量包的存放路径
        public static string incrementPackagePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\IncrementPackage\\";
    }
}

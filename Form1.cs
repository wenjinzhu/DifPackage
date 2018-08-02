using DifPackage.database;
using DifPackage.model;
using DifPackage.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DifPackage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void updateStatus(string status)
        {
            this.label4.Text = status;
        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                this.textBox1.Text = foldPath;
            }
        }

        private void btBuild_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "" || this.textBox1.Text == null)
            {
                MessageBox.Show("请填写新版本路径");
                return;
            }
            if (this.textBox2.Text == "" || this.textBox2.Text == null)
            {
                MessageBox.Show("请输入版本号");
                return;
            }

            Regex reg = new Regex(@"^[1-9]{1}\.[0-9]{1}\.[0-9]{1}$");
            if (! reg.IsMatch(this.textBox2.Text))
            {
                MessageBox.Show("当前版本号不符合规则");
                return;
            }
            
            List<VersionInfo> oldVersions = VersionDao.getAllVersions();
            /*
             * 有历史版本，则生成对应的增量包
             * 1. 生成的差分包为.zip的压缩包
             * 2. 包的命名规则：旧版本号_最新版本号.zip
             * 3. 每个旧版本都需要与最新版本对比，并生成一个独立的差分包
             */

            string newVersionBasePath = this.textBox2.Text;
            if (oldVersions.Count > 0)
            {
                string lastVersion = oldVersions[0].version.Replace("\\.","");
                string newVersion = newVersionBasePath.Replace("\\.", "");
                if (int.Parse(lastVersion) > int.Parse(newVersion))
                {
                    MessageBox.Show("当前版本小于数据库中的最后一个版本");
                    return;
                }

                StringBuilder builder = new StringBuilder();
                // 将新版本所有文件复制到fullPackage文件夹下，用于后续下一个版本更新
                FileUtil.CopyDir(this.textBox1.Text, Config.fullPackagePath + newVersionBasePath);
                // 将新版本信息插入数据库
                VersionInfo versionInfo = new VersionInfo();
                versionInfo.version = newVersionBasePath;
                versionInfo.path = Config.fullPackagePath + newVersionBasePath;
                VersionDao.insertVersion(versionInfo);
                // 将新版本所有文件复制到temp临时文件夹
                string newVersionTempaPath = Config.tempPath + newVersionBasePath;
                FileUtil.CopyDir(this.textBox1.Text, newVersionTempaPath);

                foreach (VersionInfo info in oldVersions)
                {
                    string pathA = newVersionTempaPath;
                    string pathB = info.path;

                    System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(pathA);
                    System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(pathB);

                    IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
                    IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

                    FileCompare myFileCompare = new FileCompare();

                    var queryCommonFiles = list1.Intersect(list2, myFileCompare);

                    if (queryCommonFiles.Count() > 0)
                    {
                        foreach (var v in queryCommonFiles)
                        {
                            FileUtil.DeleteFile(v.FullName);
                        }
                    }
                    else
                    {
                        Console.WriteLine("There are no common files in the two folders.");
                    }

                    // 将比较后的整个文件夹打包成zip包
                    string zipName = info.path.Substring(info.path.LastIndexOf("\\") + 1) + "_" + newVersionBasePath + ".zip";
                    List<string> paths = new List<string>();
                    paths.Add(newVersionTempaPath);
                    string zipPath = Config.incrementPackagePath + "\\" + zipName;
                    bool zipSuccess = Zip7util.CompressMulti(paths, zipPath, true);
                    if (zipSuccess)
                    {
                        builder.Append(zipName + "差分包生成成功\r\n");
                        updateStatus(builder.ToString());
                    }
                }
            }
            else
            {
                // 将新版本所有文件复制到fullPackage文件夹下，用于后续下一个版本更新
                FileUtil.CopyDir(this.textBox1.Text, Config.fullPackagePath + newVersionBasePath);
                VersionInfo info = new VersionInfo();
                info.version = newVersionBasePath;
                info.path = Config.fullPackagePath + newVersionBasePath;
                VersionDao.insertVersion(info);
                updateStatus(newVersionBasePath + "版本插入成功");
            }
        }
    }

    class FileCompare : IEqualityComparer<System.IO.FileInfo>
    {
        public FileCompare() { }

        public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
        {
            string f1MD5 = MD5Util.GetMD5HashFromFile(f1.FullName);
            string f2MD5 = MD5Util.GetMD5HashFromFile(f2.FullName);

            return (f1.Name == f2.Name && f1.Length == f2.Length && f1MD5.Equals(f2MD5));
        }

        public int GetHashCode(System.IO.FileInfo fi)
        {
            string MD5 = MD5Util.GetMD5HashFromFile(fi.FullName);
            string s = String.Format("{0}{1}{2}", fi.Name, fi.Length, MD5);
            return s.GetHashCode();
        }
    }
}

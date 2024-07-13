using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;

namespace Tool_Get_Name_File_Xml
{
    public partial class Form1 : Form
    {
        private readonly HashSet<string> processedFiles = new HashSet<string>();

        public Form1()
        {
            InitializeComponent();
            AllowDrop = true;
            DragEnter += Form1_DragEnter;
            DragDrop += Form1_DragDrop;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                foreach (string filePath in files)
                {
                    ProcessFile(filePath);
                }
            }
        }

        private void ProcessFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Tệp không tồn tại.");
                return;
            }

            if (!processedFiles.Add(filePath))
            {
                MessageBox.Show("Tệp đã được xử lý trước đó.");
                return;
            }

            try
            {
                string tempZipFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(filePath) + ".zip");
                File.Copy(filePath, tempZipFilePath, overwrite: true);

                string extractPath = Path.Combine(Path.GetDirectoryName(filePath), "xml");
                Directory.CreateDirectory(extractPath);
                ZipFile.ExtractToDirectory(tempZipFilePath, extractPath);

                string listItemFilePath = Path.Combine(extractPath, "ListItem.txt");
                WriteToFile(listItemFilePath, Directory.GetFiles(extractPath)
                    .Select(file => Path.GetFileNameWithoutExtension(file) + ".swf"));

                MessageBox.Show("Đã ghi danh sách tên tệp vào tệp ListItem.txt");

                File.Delete(tempZipFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý tệp: " + ex.Message);
            }
        }

        private void WriteToFile(string filePath, IEnumerable<string> lines)
        {
            try
            {
                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi ghi vào tệp: " + ex.Message);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }
    }
}

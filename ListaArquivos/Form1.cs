using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace ListaArquivos
{
    public partial class Form1 : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=VOSTRO;Initial Catalog=diretorio;Integrated Security=True");
        SqlCommand sqlCommand = new SqlCommand();

        private static string[] files, directories;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            dgvArquivos.Rows.Clear();
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                files = Directory.GetFiles(folder.SelectedPath);
                directories = Directory.GetDirectories(folder.SelectedPath.ToString());

                foreach (string directory in directories)
                {
                    DateTime created_at = File.GetLastAccessTime(directory);
                    dgvArquivos.Rows.Add(Path.GetFileName(directory), null, null, created_at);
                }

                foreach (string file in files)
                {
                    DateTime created_at = File.GetLastWriteTime(file);
                    dgvArquivos.Rows.Add(Path.GetFileName(file), Path.GetExtension(file), sizeOfFile(file), created_at);
                }
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            conn.Open();
            foreach (string directory in directories)
            {
                string nameDir = Path.GetFileNameWithoutExtension(directory);
                DateTime created_atDir = File.GetLastAccessTime(directory);

                sqlCommand.CommandText = "IF NOT EXISTS (SELECT * FROM arquivos WHERE name = '" + nameDir + "') INSERT INTO arquivos(name, type, size, created_at) VALUES ('" + nameDir + "', 'pasta', '', '" + created_atDir + "')";
                sqlCommand.ExecuteNonQuery();
            }
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                string ext = Path.GetExtension(file);
                long size = new FileInfo(file).Length;
                DateTime created_at = File.GetLastAccessTime(file);

                sqlCommand.CommandText = "IF NOT EXISTS (SELECT * FROM arquivos WHERE name = '" + name + "') INSERT INTO arquivos(name, type, size, created_at) VALUES ('" + name + "', '" + ext + "', '" + size + "', '" + created_at + "')";
                sqlCommand.ExecuteNonQuery();
            }
            conn.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlCommand.Connection = conn;
        }

        public string sizeOfFile(string file)
        {
            FileInfo fi = new FileInfo(file);

            long size = fi.Length;
            long result = 0;
            int i = 0;

            while (size > 1023)
            {
                result = size;
                size = (size / 1024);
                i++;
            }

            if (i < 1)
                return result + " Kb";
            if (i < 2)
                return result + " KB";
            if (i < 3)
                return result + " MB";
            else return fi.Length + "";
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private readonly string connstr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
        SqlDataAdapter adapter;
        DataSet ds = new DataSet();
        DataTable dt1;
        SqlCommandBuilder builder;
        public Form1()
        {
            
            InitializeComponent();
            CreateTable();
            adapter = new SqlDataAdapter(@"
            SELECT * FROM Books;
            SELECT * FROM Authors;",connstr);
            adapter.Fill(ds);
            builder = new SqlCommandBuilder(adapter);
            dataGridView1.DataSource = ds.Tables[0];
            
            dt1 = ds.Tables[0];
        }
        public void CreateTable()
        {
            using  (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                    IF DB_ID('Library') IS NULL
                    CREATE DATABASE Library
                    ", conn);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand(@"
                IF OBJECT_ID('Books','U') IS NULL
                CREATE TABLE Books(
                id INT PRIMARY KEY IDENTITY,
                name NVARCHAR(50),
                authorsId INt,
                photo VARBINARY(MAX)
                );
                ",conn);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand(@"
                IF OBJECT_ID('Authors','U') IS NULL
                CREATE TABLE Authors(
                id INT PRIMARY KEY IDENTITY,
                name NVARCHAR(50)
                );
                ", conn);
                cmd.ExecuteNonQuery();
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var OFD = new OpenFileDialog();
            OFD.Filter = "Image Files|*.jpg;*.png;*.jpeg";
            var selCh = dataGridView1.SelectedCells[0];
            
            if (selCh != null)
            {
                int rNum = selCh.RowIndex;
                if (rNum != -1 && rNum < dt1.Rows.Count)
                {
                    if (OFD.ShowDialog() == DialogResult.OK)
                    {

                        var bytes = File.ReadAllBytes(OFD.FileName);
                        using (var ms = new MemoryStream(bytes))
{
    pictureBox1.Image = new Bitmap(Image.FromStream(ms));
}




                        dt1.Rows[rNum]["photo"] = bytes;
                        adapter.Update(dt1);

                    }
                }
            }

            }
        
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

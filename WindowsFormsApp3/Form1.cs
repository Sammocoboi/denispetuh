using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
        DataTable dt2;
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
            dt2 = ds.Tables[1];
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
            var selRow = dataGridView1.CurrentRow?.DataBoundItem as DataRowView;
            if (selRow != null)
            {
                return;
            }
            if (OFD.ShowDialog() != DialogResult.OK) return;
            var bytes = File.ReadAllBytes(OFD.FileName) as byte[];
            if (bytes != null) return;
            selRow.Row["Photo"]=bytes;
            adapter.Update(dt1);
            using (var ms = new MemoryStream(bytes))
            {
                var _old = pictureBox1.Image;
                pictureBox1.Image = new Bitmap(ms);
                _old?.Dispose();
            }
            
            //if (selCh != null)
            //{
            //    int rNum = selCh.RowIndex;
            //    if (rNum != -1 && rNum < dt1.Rows.Count)
            //    {
            //        if (OFD.ShowDialog() == DialogResult.OK)
            //        {

            //            var bytes = File.ReadAllBytes(OFD.FileName);
            //            using (var ms = new MemoryStream(bytes))
            //        {
            //            pictureBox1.Image = new Bitmap(Image.FromStream(ms));
            //        }




            //            dt1.Rows[rNum]["photo"] = bytes;
            //            adapter.Update(dt1);

            //        }
            //    }
            //}

            }
        
        private void dataGridView1_CellValidate(object sender,DataGridView e)
        {
            var setCh = dataGridView1.SelectedCells[0];

        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            Console.WriteLine("!!!");
            var selRow = dataGridView1.CurrentRow?.DataBoundItem as DataRowView;
            if (selRow == null)
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = null;
                return;
            }
            var pict = selRow.Row["photo"] as byte[];
            if (pict == null)
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = null;
                return;
            }
            using (var ms = new MemoryStream(pict))
            {
                var _old = pictureBox1.Image;
                pictureBox1.Image = new Bitmap(ms);
                _old?.Dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var selRow = dataGridView1.CurrentCell;
        }

        private void button2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var f2 = new Form2(dt2))
            {
                if (f2.ShowDialog() != DialogResult.OK) return;

                string name = f2.BookName;
                int authorsId = f2.AuthorId;

                DataRow row = dt1.NewRow();
                row["authorsId"] = authorsId;
                row["name"] = name;
                
                dt1.Rows.Add(row);

                adapter.Update(dt1);

                
                ds.Clear();
                adapter.Fill(ds);
                dt1 = ds.Tables[0];
                dt2 = ds.Tables[1];
                dataGridView1.DataSource = dt1;
            }
        }
    }
}

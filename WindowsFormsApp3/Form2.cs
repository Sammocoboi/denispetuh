using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form2 : Form
    {
        private DataTable authors;

        public string BookName => textBox2.Text;
        public int AuthorId { get; private set; }

        public Form2(DataTable authors,DataRowView drv = null)
        {
            InitializeComponent();
            if (drv != null)
            {
                textBox1.
            }
            this.authors = authors;
            comboBox2.DataSource = authors;
            comboBox2.DisplayMember = "name";
            comboBox2.ValueMember = "id";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.BackColor = Color.Red;
                isValid = false;
            }
            else
            {
                textBox2.BackColor = SystemColors.Window;
            }

            if (comboBox2.SelectedValue == null)
            {
                comboBox2.BackColor = Color.Red;
                isValid = false;
            }
            else
            {
                comboBox2.BackColor = SystemColors.Window;
            }

            if (!isValid) return;

            AuthorId = Convert.ToInt32(comboBox2.SelectedValue);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace otel
{
    public partial class Form8 : Form
    {
        public string Customer_name
        {
            get { return txtCustomerName.Text; }
            set { txtCustomerName.Text = value; }
        }

        public string Customer_surname
        {
            get { return txtCustomerSurname.Text; }
            set { txtCustomerSurname.Text = value; }
        }

        public string Customer_phone
        {
            get { return txtCustomerPhone.Text; }
            set { txtCustomerPhone.Text = value; }
        }

        public string Customer_address
        {
            get { return txtCustomerAddress.Text; }
            set { txtCustomerAddress.Text = value; }
        }


        public Form8()
        {
            InitializeComponent();
            this.Text = "Add Customer";
            txtCustomerPhone.KeyPress += TxtCustomerPhone_KeyPress;
            txtCustomerPhone.TextChanged += TxtCustomerPhone_TextChanged;
        }

        private void TxtCustomerPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }

        }
        private void TxtCustomerPhone_TextChanged(object sender, EventArgs e)
        {

            string text = txtCustomerPhone.Text.Replace("-", "");

            if (text.Length > 3 && text.Length <= 6)
            {
                txtCustomerPhone.Text = text.Insert(3, "-");
            }
            else if (text.Length > 6)
            {
                txtCustomerPhone.Text = text.Insert(3, "-").Insert(7, "-");
            }


            txtCustomerPhone.SelectionStart = txtCustomerPhone.Text.Length;
        }   

        private void Form8_Load(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) ||
                string.IsNullOrWhiteSpace(txtCustomerSurname.Text) ||
                string.IsNullOrWhiteSpace(txtCustomerPhone.Text) ||
                string.IsNullOrWhiteSpace(txtCustomerAddress.Text))
            {
                MessageBox.Show("Tüm alanları doldurmanız gerekmektedir.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;

            this.Close();
        }
    }
}

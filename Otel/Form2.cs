﻿using System;
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
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();

            if (!Form1.IsAdmin)
            {
                // Admin değilse Button1'i devre dışı bırak
                button1.Enabled = false;  // Buton tıklanamaz
                label1.Text = "Personel";
            }
            else
            {
                label1.Text = "Admin";
            }

            this.Text = "Fatih Hotel";

        }



        private void button1_Click(object sender, EventArgs e)
        {
            Form1 newform = new Form1();
            newform.Show();
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form3 newform1 = new Form3();
            newform1.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
            this.Close();
        }
    }
}

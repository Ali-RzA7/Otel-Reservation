using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace otel
{
    public partial class Form1 : Form
    {

        SqlConnection connect = DatabaseHelper.GetConnection();

        public static string UserName;
        public static bool IsAdmin;
        public Form1()
        {
            InitializeComponent();

            this.Icon = new System.Drawing.Icon("C:/Users/user/Downloads/download.ico");
            this.Text = "Login Page";


            // TextBox'ın kenarlarını yuvarlamak için
            var myTextBox = new TextBox();
            myTextBox.Width = 200;
            myTextBox.Height = 40;
            myTextBox.Location = new Point(50, 50);

            // TextBox'ı bir panel içine yerleştiriyoruz
            var panel = new Panel();
            panel.Size = myTextBox.Size;
            panel.Location = new Point(50, 50);
            panel.BackColor = Color.Transparent;

            // Panelin köşelerini yuvarlıyoruz
            panel.Region = new Region(new Rectangle(0, 0, panel.Width, panel.Height));
            myTextBox.Parent = panel;

            // TextBox'ı panele ekliyoruz
            Controls.Add(panel);


        }
        private void LoginButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                SqlCommand loginCommand = new SqlCommand(
                    "SELECT * FROM Users WHERE UserName=@username AND Password=@password",
                    connect
                );

                loginCommand.Parameters.AddWithValue("@username", UsernameText.Text);
                loginCommand.Parameters.AddWithValue("@password", PasswordText.Text);

                SqlDataAdapter da = new SqlDataAdapter(loginCommand);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    // Statik değişkenlere kullanıcı bilgilerini atayın
                    UserName = dt.Rows[0]["UserName"].ToString();
                    IsAdmin = (bool)dt.Rows[0]["Authority"];

                    // Form2'yi aç
                    Form2 newForm2 = new Form2();
                    newForm2.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Hatalı kullanıcı adı veya şifre.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

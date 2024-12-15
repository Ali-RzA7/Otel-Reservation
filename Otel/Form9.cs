using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace otel
{
    public partial class Form9 : Form
    {
        SqlConnection connect = DatabaseHelper.GetConnection();
        private int roomID;
        public static decimal Price;
        public Form ParentForm { get; set; }
        public static string nameService;

        // Servis isimleri ve fiyatları
        private readonly string[] services = new string[]
        {
            "Oda Servisi ",
            "Çamaşırhane Servisi",
            "Spor Salonu",
            "Spa ve Masaj",
            "Yüzme Havuzu",
            "Açık Büfe Servisi"
        };

        // Servis fiyatları
        private readonly decimal[] servicePrices = new decimal[]
        {
            100.0m,  // Oda Servisi fiyatı
            80.0m,  // Çamaşırhane Servisi fiyatı
            150.0m,  // Spor Salonu fiyatı
            500.0m, // Spa ve Masaj fiyatı
            200.0m,  // Yüzme Havuzu fiyatı
            600.0m   // Açık Büfe Servisi fiyatı
        };

        // RoomID parametre olarak alınıyor
        public Form9(int roomID)
        {
            InitializeComponent();
            this.roomID = roomID;  // Parametreyi alıp sınıf seviyesinde tutuyoruz.
        }

        // Form yüklendiğinde servisleri listele
        private void Form9_Load_1(object sender, EventArgs e)
        {
            priceLabel.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            cmbPaymentMethod.Visible = false;
            textBox1.Visible = false;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);

            LoadServices();  // Servisleri listele
        }

        // Sabit servis isimlerini ListBox'a ekle
        private void LoadServices()
        {
            listBox1.Items.Clear();  // ListBox'ı temizle
            listBox1.Items.AddRange(services);  // Servisleri ekle
        }


        private void AddPayment(string paymentMethod, decimal paymentPrice, string paymentDescription, string roomId)
        {
            try
            {
                using (SqlConnection connect = DatabaseHelper.GetConnection())
                {
                    connect.Open();

                    string query = @"
                    INSERT INTO Payment (PaymentDate, PaymentMethod, PaymentPrice, PaymentDescription, RoomID)
                    VALUES (@paymentDate, @paymentMethod, @paymentPrice, @paymentDescription, @roomId)";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        cmd.Parameters.AddWithValue("@paymentDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                        cmd.Parameters.AddWithValue("@paymentPrice", paymentPrice);
                        cmd.Parameters.AddWithValue("@paymentDescription", paymentDescription);
                        cmd.Parameters.AddWithValue("@roomId", roomId);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Ödeme başarılı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        // Seçilen servisi kaydet
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedService = listBox1.SelectedItem.ToString();
                decimal selectedPrice = GetServicePrice(selectedService);
                priceLabel.Visible = true;

                if (ParentForm != null && ParentForm is Form7)
                {

                    // Form7'den geldiğini anladık, burada işlemler yapabiliriz.
                }
                else
                {
                    label1.Visible = true;
                    label2.Visible = true;
                    cmbPaymentMethod.Visible = true;
                    textBox1.Visible = true;
                }


                // Fiyatı priceLabel üzerinde göster
                priceLabel.Text = "Fiyat: " + selectedPrice.ToString("C");  // Para birimi formatında göster
            }
        }

        // Seçilen servisin fiyatını döndüren metod
        private decimal GetServicePrice(string serviceName)
        {
            int index = Array.IndexOf(services, serviceName);
            return index >= 0 ? servicePrices[index] : 0.0m;
        }

        // Servisi veritabanına kaydetmek için butona tıklama işlemi
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir servis seçin.");
                return;
            }

            string selectedService = listBox1.SelectedItem.ToString();
            decimal selectedPrice = GetServicePrice(selectedService);

            try
            {
                // Veritabanı bağlantısını açıyoruz
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                // Servisi kaydetmek için SQL sorgusu
                string query = "INSERT INTO Service (RoomID, ServiceName, ServicePrice) VALUES (@roomID, @serviceName, @servicePrice)";
                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@roomID", roomID);  // Parametre olarak gelen RoomID
                cmd.Parameters.AddWithValue("@serviceName", selectedService);  // Seçilen servis adı
                cmd.Parameters.AddWithValue("@servicePrice", selectedPrice);  // Servis fiyatı

                nameService = selectedService;

                // Komutu çalıştır
                cmd.ExecuteNonQuery();

                if (ParentForm != null && ParentForm is Form7)
                {

                }
                else
                {
                    string paymentMethod = cmbPaymentMethod.SelectedItem != null ? cmbPaymentMethod.SelectedItem.ToString() : "Cash";
                    string paymentDescription = textBox1.Text;

                    string Room = roomID.ToString();

                    AddPayment(paymentMethod, selectedPrice, paymentDescription, Room);

                }


                MessageBox.Show("Servis başarıyla kaydedildi.");
                Price = selectedPrice;
                this.Close();





            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }
    }
}

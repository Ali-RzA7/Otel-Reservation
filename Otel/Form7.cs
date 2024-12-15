using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace otel
{
    public partial class Form7 : Form
    {
        SqlConnection connect = DatabaseHelper.GetConnection();
        public int Id { get; set; }
        private string selectedRoomId;

        public Form7(string roomId)
        {
            InitializeComponent();
            this.Text = "Add Rezervation";
            dtpReleaseDate.MinDate = dtpEntryDate.Value.AddDays(1).Date;
            selectedRoomId = roomId;
            label1.Text = $"Room Number: {selectedRoomId}";
            Id = 1;

            dtpReleaseDate.ValueChanged += (s, e) => UpdatePrice();

            UpdatePrice();
        }

        private void UpdatePrice()
        {
            string entryDate = dtpEntryDate.Value.ToString("yyyy-MM-dd");
            string releaseDate = dtpReleaseDate.Value.ToString("yyyy-MM-dd");

            decimal totalPrice = CalculateTotalPrice(entryDate, releaseDate, selectedRoomId);
            labelPrice.Text = totalPrice.ToString();
        }

        private decimal CalculateTotalPrice(string entryDate, string releaseDate, string roomId)
        {
            DateTime entry = DateTime.Parse(entryDate);
            DateTime release = DateTime.Parse(releaseDate);
            int days = (release - entry).Days;

            if (days <= 0)
            {
                return 0;
            }

            decimal roomPrice = GetRoomPrice(roomId);
            return (roomPrice * days) + Form9.Price;
        }

        private decimal GetRoomPrice(string roomId)
        {
            try
            {
                using (SqlConnection connect = DatabaseHelper.GetConnection())
                {
                    connect.Open();
                    string query = "SELECT RoomPrice FROM Room WHERE RoomID = @roomId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        cmd.Parameters.AddWithValue("@roomId", roomId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                return 0;
            }
        }




        private int GetRoomCapacity(string roomId)
        {
            int capacity = 0;
            string query = "SELECT RoomCapacity FROM Room WHERE RoomID = @RoomID";

            using (SqlConnection connect = DatabaseHelper.GetConnection())  // connectionString yerine doğru bağlantıyı kullanın.
            {
                SqlCommand command = new SqlCommand(query, connect);
                command.Parameters.AddWithValue("@RoomID", roomId); // Oda numarasını string olarak geçiriyoruz.

                try
                {
                    connect.Open();
                    // ExecuteScalar() null dönebilir, bu yüzden null kontrolü yapalım.
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        // Eğer result null değilse, kapasiteyi int'e dönüştür
                        capacity = Convert.ToInt32(result);
                    }
                    else
                    {
                        // Eğer sorgudan sonuç dönmediyse, kapasiteyi 0 olarak ayarlayabiliriz veya hata mesajı verebiliriz.
                        MessageBox.Show("Oda bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return capacity;
        }


        private int GetCurrentCustomerCount()
        {
            return dataGridView1.Rows.Count;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            int roomCapacity = GetRoomCapacity(selectedRoomId);
            int currentCustomerCount = GetCurrentCustomerCount();

            if (currentCustomerCount >= roomCapacity)
            {
                MessageBox.Show("Oda kapasitesine ulaşıldı. Daha fazla müşteri eklenemez.", "Kapasite Dolduruldu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {

                Form8 form8 = new Form8();
                DialogResult dialogResult = form8.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    dataGridView1.Rows.Add(Id++,
                        form8.Customer_name,
                        form8.Customer_surname,
                        form8.Customer_phone,
                        form8.Customer_address);
                }
            }
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("Id", "Id");
            dataGridView1.Columns.Add("Name", "Name");
            dataGridView1.Columns.Add("Surname", "Surname");
            dataGridView1.Columns.Add("Phone", "Phone");
            dataGridView1.Columns.Add("Address", "Address");
        }

        private int InsertCustomer(string name, string surname, string phone, string address)
        {
            // Müşteri zaten var mı diye kontrol et
            string checkQuery = "SELECT CustomerID FROM Customer WHERE CustomerName = @Name AND CustomerSurname = @Surname AND CustomerPhone = @Phone";

            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@Name", name);
                checkCommand.Parameters.AddWithValue("@Surname", surname);
                checkCommand.Parameters.AddWithValue("@Phone", phone);

                try
                {
                    connection.Open();
                    // Eğer müşteri varsa, müşteri ID'sini döndür
                    var result = checkCommand.ExecuteScalar();
                    if (result != null)
                    {
                        // Müşteri zaten var, ID'yi döndür
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        // Müşteri yok, yeni müşteri ekle
                        string insertQuery = "INSERT INTO Customer (CustomerName, CustomerSurname, CustomerPhone, CustomerAddress) OUTPUT INSERTED.CustomerID VALUES (@Name, @Surname, @Phone, @Address)";
                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@Name", name);
                        insertCommand.Parameters.AddWithValue("@Surname", surname);
                        insertCommand.Parameters.AddWithValue("@Phone", phone);
                        insertCommand.Parameters.AddWithValue("@Address", address);

                        // Yeni müşteri ekle ve ID'yi döndür
                        int customerId = Convert.ToInt32(insertCommand.ExecuteScalar());
                        return customerId;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Müşteri kaydedilemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1; // Hata durumunda -1 döndürüyoruz
                }
            }
        }

        private void InsertReservation(DateTime entryDate, DateTime releaseDate, string roomId, int customerId)
        {

            try
            {
                using (SqlConnection connect = DatabaseHelper.GetConnection())
                {
                    connect.Open();

                    string query = @"
                        INSERT INTO Reservation (EntryDate, ReleaseDate, Status, CustomerID, RoomID)
                        VALUES (@entryDate, @releaseDate, @status, @customerId, @roomId)";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        cmd.Parameters.AddWithValue("@entryDate", entryDate);
                        cmd.Parameters.AddWithValue("@releaseDate", releaseDate);
                        cmd.Parameters.AddWithValue("@status", "1");
                        cmd.Parameters.AddWithValue("@customerId", customerId);
                        cmd.Parameters.AddWithValue("@roomId", roomId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }

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


        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {


                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Satırda boş hücre olup olmadığını kontrol et (opsiyonel)
                    string customerName = row.Cells["Name"].Value.ToString();
                    string customerSurname = row.Cells["Surname"].Value.ToString();
                    string customerPhone = row.Cells["Phone"].Value.ToString();
                    string customerAddress = row.Cells["Address"].Value.ToString();
                    DateTime entryDate = dtpEntryDate.Value;
                    DateTime releaseDate = dtpReleaseDate.Value;

                    try
                    {
                        // 1. Müşteri bilgisini Customer tablosuna kaydet
                        int customerId = InsertCustomer(customerName, customerSurname, customerPhone, customerAddress);



                        // 2. Rezervasyon bilgisini Reservation tablosuna kaydet
                        InsertReservation(entryDate, releaseDate, selectedRoomId, customerId);


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata: " + ex.Message, "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

                try
                {
                    string paymentMethod = cmbPaymentMethod.SelectedItem != null ? cmbPaymentMethod.SelectedItem.ToString() : "Cash"; // "N/A" varsayılan değer

                    string paymentDescription = txtPaymentDescription.Text;

                    string EntryDate = dtpEntryDate.Value.ToString("yyyy-MM-dd");
                    string ReleaseDate = dtpReleaseDate.Value.ToString("yyyy-MM-dd");

                    decimal paymentPrice = CalculateTotalPrice(EntryDate, ReleaseDate, selectedRoomId);

                    AddPayment(paymentMethod, paymentPrice, paymentDescription, selectedRoomId);
                    string queryUpdateRoom = @"
                        UPDATE Room
                        SET RoomAvailable = 0
                        WHERE RoomID = @roomId";

                    // 'connect' nesnesi sınıf seviyesinde tanımlandığı için, burada 'connect' nesnesini kullanacağız.
                    using (SqlCommand cmdUpdateRoom = new SqlCommand(queryUpdateRoom, connect))
                    {
                        // Bağlantıyı kontrol et, eğer bağlantı açık değilse aç
                        if (connect.State == ConnectionState.Closed)
                        {
                            connect.Open();
                        }

                        cmdUpdateRoom.Parameters.AddWithValue("@roomId", selectedRoomId);
                        cmdUpdateRoom.ExecuteNonQuery();
                    }



                    // Servisi kaydetmek için SQL sorgusu
                    string serviceName = Form9.nameService;
                    decimal servicePrice = Form9.Price;

                    // Servis adı veya fiyatının boş olmadığını ve geçerli bir fiyat olduğunu kontrol et
                    if (string.IsNullOrEmpty(serviceName) || servicePrice <= 0)
                    {


                    }
                    else
                    {

                        string query = "INSERT INTO Service (RoomID, ServiceName, ServicePrice) VALUES (@roomID, @serviceName, @servicePrice)";

                        using (SqlCommand cmd = new SqlCommand(query, connect))
                        {
                            if (connect.State == ConnectionState.Closed)
                            {
                                connect.Open();
                            }
                            cmd.Parameters.AddWithValue("@roomID", selectedRoomId);  // Parametre olarak gelen RoomID
                            cmd.Parameters.AddWithValue("@serviceName", serviceName);  // Seçilen servis adı
                            cmd.Parameters.AddWithValue("@servicePrice", servicePrice);  // Servis fiyatı

                            // Komutu çalıştır
                            cmd.ExecuteNonQuery();
                        }

                    }




                    MessageBox.Show("Rezervasyon başarılı.");
                }
                catch(Exception  ex){
                    MessageBox.Show("Hata", ex.Message);
                }
                // PaymentMethod combobox'ının boş olup olmadığını kontrol et

            }
            else
            {
                MessageBox.Show("İşlem Yapılmadı");
            }
            Form4 form4 = new Form4();
            form4.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int number = Convert.ToInt32(selectedRoomId);
            Form9 newForm9 = new Form9(number)
            {
                ParentForm = this // Form7 referansını gönderiyoruz
            };
            newForm9.FormClosed += (s, args) =>
            {
                // Form9 kapandıktan sonra çalışacak fonksiyon
                UpdatePrice();
            };

            // Form9'u gösteriyoruz.
            newForm9.ShowDialog();
        }
    }
}

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

namespace otel
{
    public partial class Form4 : Form
    {
        private System.Windows.Forms.Timer reservationTimer;
        SqlConnection connect = DatabaseHelper.GetConnection();

        private int currentFloor = 1;
        public Form4()
        {
            InitializeComponent();
            this.Text = "Rezervaiton Page";
            LoadRoomsByFloor(currentFloor);
            button2.Enabled = false;
            UpdateRoomAvailability();
            SetupTimer();
        }
        private void SetupTimer()
        {
            reservationTimer = new System.Windows.Forms.Timer();
            reservationTimer.Interval = 30000; // Her 60 saniyede bir çalışır
            reservationTimer.Tick += (sender, e) => UpdateRoomAvailability(); // Metodu çağırır
            reservationTimer.Start(); // Timer'ı başlatır
        }

        private void OpenReservationForm(string roomId)
        {
            Form7 reservationForm = new Form7(roomId);
            reservationForm.Show();
            this.Close();
        }

        private void ReservationDetailsForm(string roomId)
        {
            Form6 reservationForm = new Form6(roomId);
            reservationForm.Show();
            this.Close();
        }

        private void UpdateRoomAvailability()
        {
            // Çıkış tarihi için kontrol edilen tarih
            DateTime currentDate = DateTime.Now;

            try
            {
                if (connect.State == ConnectionState.Closed)
                    connect.Open();

                // SQL komutu: Odanın ve rezervasyonun durumunu güncelle
                string query = @"

                     UPDATE Room
                     SET RoomAvailable = 1
                     WHERE RoomID IN (
                         SELECT RoomID FROM Reservation
                         WHERE ReleaseDate <= GetDate() AND Status = 1
                     );
 
                     UPDATE Reservation
                     SET Status = 0
                     WHERE ReleaseDate <= GetDate() AND Status = 1;
                     ";

                using (SqlCommand command = new SqlCommand(query, connect))
                {
                    command.Parameters.AddWithValue("@currentDate", currentDate);

                    // Komut çalıştır
                    int rowsAffected = command.ExecuteNonQuery();

                    // Güncelleme sonrası mesaj
                    //MessageBox.Show($"{rowsAffected} satır güncellendi.");
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj göster
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                    connect.Close();
            }

            // Odaları güncel kat için yeniden yükle
            LoadRoomsByFloor(currentFloor);
        }





        private void LoadRoomsByFloor(int floor)
        {
            try
            {
                flowLayoutPanel1.Controls.Clear(); // Mevcut odaları temizle

                connect.Open();

                // SQL sorgusu ile kat numarasına göre odaları çek
                string query = @"
                    SELECT RoomID, RoomAvailable 
                    FROM Room 
                    WHERE LEFT(RoomID, 1) = @floor";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@floor", floor.ToString()); // Kat bilgisi parametre olarak ekleniyor

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string roomId = reader["RoomID"].ToString();
                    bool isAvailable = Convert.ToBoolean(reader["RoomAvailable"]);

                    // Buton oluştur
                    Button roomButton = new Button();
                    roomButton.Text = roomId;
                    roomButton.Width = 80;
                    roomButton.Height = 50;

                    // Doluluk durumuna göre renk belirle
                    if (isAvailable)
                    {
                        roomButton.BackColor = Color.Green; // Boş oda
                        roomButton.ForeColor = Color.White;
                        roomButton.Click += (sender, e) => OpenReservationForm(roomId);
                    }
                    else
                    {
                        roomButton.BackColor = Color.Red; // Dolu oda
                        roomButton.ForeColor = Color.White;
                        roomButton.Click += (sender, e) => ReservationDetailsForm(roomId);
                    }

                    flowLayoutPanel1.Controls.Add(roomButton); // Butonu ekle
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                connect.Close();
            }
        }





        private void button1_Click(object sender, EventArgs e)
        {
            if (currentFloor < 9)
            {
                currentFloor++;
                
                LoadRoomsByFloor(currentFloor);
                label1.Text = "Kat: " + currentFloor.ToString();
            }

            // Eğer son kat ise ileri butonunu devre dışı bırak
            if (currentFloor == 9)
            {
                button1.Enabled = false;
                label1.Text = "Kat: " + currentFloor.ToString();
            }

            // Geri butonunu etkinleştir
            button2.Enabled = true;

        }





        private void button2_Click(object sender, EventArgs e)
        {
            if (currentFloor > 1)
            {
                currentFloor--;
                LoadRoomsByFloor(currentFloor);
                label1.Text = "Kat: " + currentFloor.ToString();
            }

            // Eğer ilk kat ise geri butonunu devre dışı bırak
            if (currentFloor == 1)
            {
                button2.Enabled = false;
                label1.Text = "Kat: " + currentFloor.ToString();
            }

            // İleri butonunu etkinleştir
            button1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Close();
        }
    }
}

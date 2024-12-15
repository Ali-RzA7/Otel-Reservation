using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.IO;

namespace otel
{
    public partial class Form3 : Form
    {
        SqlConnection connect = DatabaseHelper.GetConnection();
        public Form3()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        }
        public void BringRecord()
        {
            connect.Open();

            string bring = "SELECT * FROM Customer WHERE 1=1"; // Başlangıç şartı, dinamik eklemeyi kolaylaştırır

            SqlCommand komut = new SqlCommand(bring, connect);

            // Dinamik koşulları ekleyerek sorguyu oluşturun
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                bring += " AND CustomerName = @pname";
                komut.Parameters.AddWithValue("@pname", textBox1.Text);
            }

            if (!string.IsNullOrWhiteSpace(textBox2.Text))
            {
                bring += " AND CustomerSurname = @psurname";
                komut.Parameters.AddWithValue("@psurname", textBox2.Text);
            }

            if (!string.IsNullOrWhiteSpace(textBox3.Text))
            {
                bring += " AND CustomerPhone = @pphone";
                komut.Parameters.AddWithValue("@pphone", textBox3.Text);
            }

            if (!string.IsNullOrWhiteSpace(textBox4.Text))
            {
                bring += " AND CustomerAddress = @paddress";
                komut.Parameters.AddWithValue("@paddress", textBox4.Text);
            }

            // Sorguyu yeniden ayarla
            komut.CommandText = bring;

            SqlDataAdapter ad = new SqlDataAdapter(komut);

            DataTable dt = new DataTable();
            ad.Fill(dt);

            dataGridView1.DataSource = dt;

            connect.Close();


        }

        public void DeleteRecord()
        {
            connect.Open();

            // Önce ilgili CustomerID'nin Reservation tablosundaki durumunu kontrol edelim
            string checkReservationQuery = @"
        SELECT COUNT(*) 
        FROM Reservation 
        WHERE CustomerID = (SELECT CustomerID FROM Customer WHERE CustomerName = @pname AND CustomerSurname = @psurname)
        AND Status != 0"; // Status 0 olmayan rezervasyonları sorguluyoruz
            SqlCommand checkReservationCommand = new SqlCommand(checkReservationQuery, connect);
            checkReservationCommand.Parameters.AddWithValue("@pname", textBox1.Text);
            checkReservationCommand.Parameters.AddWithValue("@psurname", textBox2.Text);

            int activeReservationCount = (int)checkReservationCommand.ExecuteScalar(); // Aktif olmayan rezervasyon sayısını alıyoruz

            if (activeReservationCount > 0)
            {
                MessageBox.Show("Bu müşterinin aktif rezervasyonu olduğu için silme işlemi yapılamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connect.Close();
                return; // Silme işlemine devam etmiyoruz
            }

            // Silinecek veriyi Deleted tablosuna ekleyelim
            string insertDeletedQuery = @"
        INSERT INTO Deleted (CustomerID, CustomerName, CustomerSurname, CustomerPhone, CustomerAddress)
        SELECT CustomerID, CustomerName, CustomerSurname, CustomerPhone, CustomerAddress
        FROM Customer
        WHERE CustomerName = @pname OR CustomerSurname = @psurname OR CustomerPhone = @pphone OR CustomerAddress = @paddress";
            SqlCommand insertDeletedCommand = new SqlCommand(insertDeletedQuery, connect);
            insertDeletedCommand.Parameters.AddWithValue("@pname", textBox1.Text);
            insertDeletedCommand.Parameters.AddWithValue("@psurname", textBox2.Text);
            insertDeletedCommand.Parameters.AddWithValue("@pphone", textBox3.Text);
            insertDeletedCommand.Parameters.AddWithValue("@paddress", textBox4.Text);

            insertDeletedCommand.ExecuteNonQuery(); // Silinen veriyi Deleted tablosuna ekleyelim

            // Şimdi müşteri kaydını silelim
            string deleteQuery = "DELETE FROM Customer WHERE CustomerName=@pname OR CustomerSurname=@psurname OR CustomerPhone=@pphone OR CustomerAddress=@paddress";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery, connect);
            deleteCommand.Parameters.AddWithValue("@pname", textBox1.Text);
            deleteCommand.Parameters.AddWithValue("@psurname", textBox2.Text);
            deleteCommand.Parameters.AddWithValue("@pphone", textBox3.Text);
            deleteCommand.Parameters.AddWithValue("@paddress", textBox4.Text);

            deleteCommand.ExecuteNonQuery(); // Silme işlemi

            connect.Close();

            MessageBox.Show("Veri başarıyla silindi ve Deleted tablosuna kaydedildi.");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            BringRecord();

        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DeleteRecord();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            Form2 form2 = new Form2();
            form2.Show();
            this.Close();
        }

        private void export_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable customerData;

                // Eğer DataGridView boşsa tüm verileri veritabanından çek
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("Tüm veriler dışa aktarılıyor...", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    customerData = GetCustomerData();
                }
                else
                {
                    // DataGridView'de veri varsa mevcut veriyi DataTable olarak çek
                    customerData = (DataTable)dataGridView1.DataSource;
                }

                if (customerData.Rows.Count > 0)
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                        saveFileDialog.FileName = "CustomerData.xlsx";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            using (ExcelPackage package = new ExcelPackage())
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Customers");

                                // Sütun başlıklarını ekle
                                for (int i = 0; i < customerData.Columns.Count; i++)
                                {
                                    worksheet.Cells[1, i + 1].Value = customerData.Columns[i].ColumnName;
                                }

                                // Verileri ekle
                                for (int i = 0; i < customerData.Rows.Count; i++)
                                {
                                    for (int j = 0; j < customerData.Columns.Count; j++)
                                    {
                                        worksheet.Cells[i + 2, j + 1].Value = customerData.Rows[i][j];
                                    }
                                }

                                // Excel dosyasını kaydet
                                FileInfo fileInfo = new FileInfo(saveFileDialog.FileName);
                                package.SaveAs(fileInfo);

                                MessageBox.Show("Veriler başarıyla Excel dosyasına aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Dışa aktarılacak veri bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddCustomerToDatabase(string name, string email, string phone, string address)
        {
            try
            {
                string query = "INSERT INTO Customer (CustomerName, CustomerSurname, CustomerPhone, CustomerAddress) VALUES (@Name, @Email, @Phone, @Address)";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Address", address);

                    if (connect.State == ConnectionState.Closed)
                    {
                        connect.Open();
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanına veri eklerken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }
        private DataTable GetCustomerData()
        {
            DataTable dataTable = new DataTable();


            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM Customer"; // Tüm veriyi çek
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
            }

            return dataTable;
        }

        private void import_Click(object sender, EventArgs e)
        {
            try
            {
                // Excel dosyasını seçmek için SaveFileDialog açıyoruz
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

                        // Excel dosyasını açıyoruz
                        using (ExcelPackage package = new ExcelPackage(fileInfo))
                        {
                            // İlk sayfayı alıyoruz (ilk sayfayı varsayıyoruz)
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                            // Excel sayfasındaki satır sayısını alıyoruz
                            int rowCount = worksheet.Dimension.Rows;

                            // Excel'deki her satırı okuyoruz
                            for (int row = 2; row <= rowCount; row++) // 2. satırdan başlıyoruz, çünkü 1. satır başlık satırı
                            {
                                string name = worksheet.Cells[row, 1].Text;
                                string email = worksheet.Cells[row, 2].Text;
                                string phone = worksheet.Cells[row, 3].Text;
                                string address = worksheet.Cells[row, 4].Text;

                                // Veriyi SQL Server'a ekliyoruz
                                AddCustomerToDatabase(name, email, phone, address);
                            }

                            MessageBox.Show("Veriler başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}

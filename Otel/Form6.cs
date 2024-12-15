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
    public partial class Form6 : Form
    {
        SqlConnection connect = DatabaseHelper.GetConnection();
        private int roomId;

        public Form6(string roomId)
        {
            InitializeComponent();
            int roomıd = Convert.ToInt32(roomId);
            this.roomId = roomıd;
            label1.Text = $"Room Number: {this.roomId}";
            LoadReservationDetails();
        }

        private void LoadReservationDetails()
        {
            try
            {
                connect.Open();

                // SQL sorgusu: Odaya ait aktif (Status = 1) rezervasyonları al
                string query = @"
                    SELECT c.CustomerName, c.CustomerSurname, c.CustomerPhone, c.CustomerAddress, r.EntryDate, r.ReleaseDate
                    FROM Reservation r
                    JOIN Customer c ON r.CustomerID = c.CustomerID
                    WHERE r.RoomID = @roomId
                    AND r.Status = 1
                    ORDER BY r.EntryDate DESC;";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@roomId", roomId);

                SqlDataReader reader = cmd.ExecuteReader();

                // DataTable oluşturuyoruz
                DataTable dt = new DataTable();
                dt.Load(reader);

                dataGridView1.AllowUserToAddRows = false;

                // Eğer DataTable'da veri varsa, DataGridView'ı dolduruyoruz
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row.ItemArray.All(field => field == DBNull.Value || field.ToString().Trim() == ""))
                        {
                            row.Delete();
                        }
                    }
                    dt.AcceptChanges();
                    dataGridView1.DataSource = dt;
                    dataGridView1.ReadOnly = true;
                }
                else
                {
                    MessageBox.Show("No active reservation found for the selected room.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            finally
            {
                connect.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
            this.Close();
        }

        private void Form6_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int number = Convert.ToInt32(roomId);
            Form9 newForm9 = new Form9(number);
            newForm9.ShowDialog();

        }
    }



}
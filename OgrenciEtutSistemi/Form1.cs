using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace OgrenciEtutSistemi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-KIMUOA0\SQLEXPRESS;Initial Catalog=DbEtüt;Integrated Security=True");

        // Ders listesi
        void dersListesi()
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from TBLDERS", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            comboBoxders.ValueMember = "DERSID";
            comboBoxders.DisplayMember = "DERSAD";
            comboBoxders.DataSource = dt;
        }

        // Dersleri getir
        void dersleriGetir()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT DERSID, DERSAD FROM TBLDERS", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            comboBoxOGRTDERS.ValueMember = "DERSID";
            comboBoxOGRTDERS.DisplayMember = "DERSAD";
            comboBoxOGRTDERS.DataSource = dt;
        }

        // Etüt listesi
        void etutListesi()
        {
            SqlDataAdapter da3 = new SqlDataAdapter("execute Etut", baglanti);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);
            dataGridView1.DataSource = dt3;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
        }

        //Ogrenci listesi
        void ogrenciListesiGetir()
        {
            try
            {
                // Öğrenci isim ve soyisimlerini getirmek için SQL sorgusu
                SqlDataAdapter da = new SqlDataAdapter("SELECT OGRID, (AD + ' ' + SOYAD) AS OGRENCI FROM TBLOGRENCI", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // ComboBox ayarları
                comboBoxOGRAD.ValueMember = "OGRID";       // Öğrenci ID'si (arka planda)
                comboBoxOGRAD.DisplayMember = "OGRENCI"; // Öğrenci isim ve soyisim (görünen)
                comboBoxOGRAD.DataSource = dt;           // Veriyi ComboBox'a bağla
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            dersListesi();
            comboBoxders.SelectedIndex = -1; // Başlangıçta hiçbir ders seçili olmasın
            comboBoxogretmen.SelectedIndex = -1;
            etutListesi();
            dersleriGetir();  // Dersleri combobox'a getir
            comboBoxOGRTDERS.SelectedIndex = -1;  // Başlangıçta hiçbir ders seçili olmasın
            ogrenciListesiGetir();  // Öğrenci listesini comboBox'a getir
            comboBoxOGRAD.SelectedIndex = -1;
        }

        private void comboBoxders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxders.SelectedValue != null)
            {
                SqlDataAdapter da2 = new SqlDataAdapter(
                    "SELECT OGRTID, (AD + ' ' + SOYAD) AS OGRETMEN FROM TBLOGRETMEN WHERE BRANSID = @p1", baglanti);
                da2.SelectCommand.Parameters.AddWithValue("@p1", comboBoxders.SelectedValue);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                comboBoxogretmen.ValueMember = "OGRTID";
                comboBoxogretmen.DisplayMember = "OGRETMEN"; // Birleştirilmiş ad ve soyad
                comboBoxogretmen.DataSource = dt2;
            }
        }

        private void buttonEtutOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                // Veritabanına bağlantıyı aç
                baglanti.Open();

                // Etüt ekleme komutunu oluştur
                SqlCommand komut = new SqlCommand("insert into TblEtut (DERSID, OGRETMENID, TARIH, SAAT) values (@p1, @p2, @p3, @p4)", baglanti);
                komut.Parameters.AddWithValue("@p1", comboBoxders.SelectedValue);
                komut.Parameters.AddWithValue("@p2", comboBoxogretmen.SelectedValue);
                komut.Parameters.AddWithValue("@p3", maskedTextBoxtarih.Text);
                komut.Parameters.AddWithValue("@p4", maskedTextBoxsaat.Text);
                komut.ExecuteNonQuery();

                // Başarı mesajı göster
                MessageBox.Show("Etüt Oluştu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Etüt listesini güncelle
                etutListesi();
            }
            catch (Exception ex)
            {
                // Hata mesajını göster
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapat
                baglanti.Close();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            textBoxetutid.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
        }

        private void buttonetutVer_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxOGRAD.SelectedValue == null)
                {
                    MessageBox.Show("Lütfen bir öğrenci seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                baglanti.Open();
                SqlCommand komut = new SqlCommand("update TBLETUT set OGRENCIID=@P1, DURUM=@P2 where ID=@P3", baglanti);
                komut.Parameters.AddWithValue("@P1", comboBoxOGRAD.SelectedValue);  // Seçilen öğrencinin ID'si
                komut.Parameters.AddWithValue("@P2", "True");
                komut.Parameters.AddWithValue("@P3", textBoxetutid.Text);  // Etüt ID
                komut.ExecuteNonQuery();
                MessageBox.Show("Etüt Öğrenciye Verilmiştir.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
                etutListesi();  // Etüt listesi yenilensin
            }
        }

        private void btnFotoYukle_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog1.FileName;
        }

        private void btnOgrenciEkle_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("insert into TBLOGRENCI(AD ,SOYAD, FOTOGRAF,SINIF,TELEFON,MAIL) values (@p1,@p2,@p3,@p4,@p5,@p6)", baglanti);
                komut.Parameters.AddWithValue("@p1", textBoxAD.Text);
                komut.Parameters.AddWithValue("@p2", textBoxSOYAD.Text);
                komut.Parameters.AddWithValue("@p3", pictureBox1.ImageLocation);
                komut.Parameters.AddWithValue("@p4", textBoxSINIF.Text);
                komut.Parameters.AddWithValue("@p5", maskedTextBoxTEL.Text);
                komut.Parameters.AddWithValue("@p6", textBoxMAIL.Text);
                komut.ExecuteNonQuery();
                MessageBox.Show("Ögrenci sisteme kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void btnDersEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDERSADI.Text))
            {
                MessageBox.Show("Lütfen ders adını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                baglanti.Open();
                SqlCommand kontrolKomut = new SqlCommand("select count(*) from TBLDERS where DERSAD = @p1", baglanti);
                kontrolKomut.Parameters.AddWithValue("@p1", textBoxDERSADI.Text);
                int dersSayisi = (int)kontrolKomut.ExecuteScalar();

                if (dersSayisi > 0)
                {
                    MessageBox.Show("Bu ders zaten mevcut.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    SqlCommand komut = new SqlCommand("insert into TBLDERS (DERSAD) values (@p1)", baglanti);
                    komut.Parameters.AddWithValue("@p1", textBoxDERSADI.Text);
                    komut.ExecuteNonQuery();
                    MessageBox.Show("Ders başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Ders listesini güncelle
                    dersListesi();  // Ders listesini yenile
                    dersleriGetir();  // Ders combobox'ını yenile
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
                textBoxDERSADI.Clear();  // TextBox'ı temizle
            }
        }

        private void btnOgretmenEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxOGRTAD.Text) || string.IsNullOrEmpty(textBoxOGRTSOYAD.Text) || comboBoxOGRTDERS.SelectedValue == null)
            {
                MessageBox.Show("Lütfen tüm bilgileri doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("insert into TBLOGRETMEN (AD, SOYAD, BRANSID) values (@p1, @p2, @p3)", baglanti);
                komut.Parameters.AddWithValue("@p1", textBoxOGRTAD.Text);
                komut.Parameters.AddWithValue("@p2", textBoxOGRTSOYAD.Text);
                komut.Parameters.AddWithValue("@p3", comboBoxOGRTDERS.SelectedValue);  // Seçilen dersin ID'si
                komut.ExecuteNonQuery();
                MessageBox.Show("Öğretmen başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Öğretmen listesini güncelle
                dersleriGetir();  // Öğretmen combobox'ını yenile
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
                textBoxOGRTAD.Clear();
                textBoxOGRTSOYAD.Clear();
                comboBoxOGRTDERS.SelectedIndex = -1;  // Seçimi temizle
            }
        }
    }
}


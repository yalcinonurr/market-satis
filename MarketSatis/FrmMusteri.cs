#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MarketSatis
{
    public partial class FrmMusteri : Form
    {
        private DataGridView dgv;
        private TextBox txtAdi, txtSoyadi, txtTelefon, txtEposta, txtAdres, txtSemt, txtSehir, txtAra;
        private Button btnEkle, btnGuncelle, btnSil, btnTemizle;
        private int seciliId = 0;

        public FrmMusteri()
        {
            InitializeComponent();
            ArayuzuOlustur();
            Listele();
        }

        private void ArayuzuOlustur()
        {
            Text = "Müşteri İşlemleri";
            ClientSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;

            // Sol sütun
            var lblAdi = new Label { Text = "Adı:", Left = 20, Top = 23, AutoSize = true };
            txtAdi = new TextBox { Left = 100, Top = 20, Width = 200, MaxLength = 50 };

            var lblSoyadi = new Label { Text = "Soyadı:", Left = 20, Top = 58, AutoSize = true };
            txtSoyadi = new TextBox { Left = 100, Top = 55, Width = 200, MaxLength = 50 };

            var lblTelefon = new Label { Text = "Telefon:", Left = 20, Top = 93, AutoSize = true };
            txtTelefon = new TextBox { Left = 100, Top = 90, Width = 200, MaxLength = 20 };

            var lblEposta = new Label { Text = "E-posta:", Left = 20, Top = 128, AutoSize = true };
            txtEposta = new TextBox { Left = 100, Top = 125, Width = 200, MaxLength = 100 };

            // Orta sütun
            var lblSemt = new Label { Text = "Semt/İlçe:", Left = 330, Top = 23, AutoSize = true };
            txtSemt = new TextBox { Left = 410, Top = 20, Width = 200, MaxLength = 50 };

            var lblSehir = new Label { Text = "Şehir:", Left = 330, Top = 58, AutoSize = true };
            txtSehir = new TextBox { Left = 410, Top = 55, Width = 200, MaxLength = 50 };

            var lblAdres = new Label { Text = "Adres:", Left = 330, Top = 93, AutoSize = true };
            txtAdres = new TextBox
            {
                Left = 410,
                Top = 90,
                Width = 200,
                Height = 58,
                Multiline = true,
                MaxLength = 255
            };

            // Butonlar (sağ sütun)
            btnEkle = new Button { Text = "Ekle", Left = 650, Top = 18, Width = 120, Height = 30 };
            btnGuncelle = new Button { Text = "Güncelle", Left = 780, Top = 18, Width = 100, Height = 30 };
            btnSil = new Button { Text = "Sil", Left = 650, Top = 56, Width = 120, Height = 30 };
            btnTemizle = new Button { Text = "Temizle", Left = 780, Top = 56, Width = 100, Height = 30 };

            // Arama
            var lblAra = new Label { Text = "Ara (ad, soyad, telefon):", Left = 20, Top = 168, AutoSize = true };
            txtAra = new TextBox { Left = 180, Top = 165, Width = 250 };

            dgv = new DataGridView
            {
                Left = 20,
                Top = 200,
                Width = 860,
                Height = 380,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            btnEkle.Click += BtnEkle_Click;
            btnGuncelle.Click += BtnGuncelle_Click;
            btnSil.Click += BtnSil_Click;
            btnTemizle.Click += (s, e) => Temizle();
            txtAra.TextChanged += (s, e) => Listele();
            dgv.CellClick += Dgv_CellClick;

            Controls.AddRange(new Control[]
            {
                lblAdi, txtAdi, lblSoyadi, txtSoyadi, lblTelefon, txtTelefon,
                lblEposta, txtEposta, lblSemt, txtSemt, lblSehir, txtSehir,
                lblAdres, txtAdres,
                btnEkle, btnGuncelle, btnSil, btnTemizle,
                lblAra, txtAra, dgv
            });
        }

        private void Listele()
        {
            string arama = "%" + txtAra.Text.Trim() + "%";
            var tablo = Baglanti.Getir(
                @"SELECT musteri_id, adi, soyadi, telefon, eposta, adres, semt_ilce, sehir
                  FROM musteri
                  WHERE adi LIKE @a OR soyadi LIKE @a OR telefon LIKE @a
                  ORDER BY adi, soyadi",
                new MySqlParameter("@a", arama));
            dgv.DataSource = tablo;

            dgv.Columns["musteri_id"].Visible = false;
            dgv.Columns["adi"].HeaderText = "Adı";
            dgv.Columns["soyadi"].HeaderText = "Soyadı";
            dgv.Columns["telefon"].HeaderText = "Telefon";
            dgv.Columns["eposta"].HeaderText = "E-posta";
            dgv.Columns["adres"].HeaderText = "Adres";
            dgv.Columns["semt_ilce"].HeaderText = "Semt/İlçe";
            dgv.Columns["sehir"].HeaderText = "Şehir";
            dgv.ClearSelection();
        }

        private void Temizle()
        {
            seciliId = 0;
            txtAdi.Clear();
            txtSoyadi.Clear();
            txtTelefon.Clear();
            txtEposta.Clear();
            txtAdres.Clear();
            txtSemt.Clear();
            txtSehir.Clear();
            dgv.ClearSelection();
            txtAdi.Focus();
        }

        private bool AlanlarGecerliMi()
        {
            if (string.IsNullOrWhiteSpace(txtAdi.Text) || string.IsNullOrWhiteSpace(txtSoyadi.Text))
            {
                MessageBox.Show("Ad ve soyad boş olamaz.");
                return false;
            }
            string eposta = txtEposta.Text.Trim();
            if (eposta != "" && (!eposta.Contains("@") || !eposta.Contains(".")))
            {
                MessageBox.Show("E-posta adresi geçerli görünmüyor.");
                return false;
            }
            return true;
        }

        private static string BosIseNull(string s)
        {
            s = s.Trim();
            return s == "" ? null : s;
        }

        private MySqlParameter[] Parametreler(bool idDahil)
        {
            var liste = new System.Collections.Generic.List<MySqlParameter>
            {
                new MySqlParameter("@adi", txtAdi.Text.Trim()),
                new MySqlParameter("@soyadi", txtSoyadi.Text.Trim()),
                new MySqlParameter("@telefon", (object)BosIseNull(txtTelefon.Text) ?? DBNull.Value),
                new MySqlParameter("@eposta", (object)BosIseNull(txtEposta.Text) ?? DBNull.Value),
                new MySqlParameter("@adres", (object)BosIseNull(txtAdres.Text) ?? DBNull.Value),
                new MySqlParameter("@semt", (object)BosIseNull(txtSemt.Text) ?? DBNull.Value),
                new MySqlParameter("@sehir", (object)BosIseNull(txtSehir.Text) ?? DBNull.Value)
            };
            if (idDahil) liste.Add(new MySqlParameter("@id", seciliId));
            return liste.ToArray();
        }

        private static string Metin(DataGridViewRow satir, string kolon)
        {
            var v = satir.Cells[kolon].Value;
            return v == null || v == DBNull.Value ? "" : v.ToString();
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var satir = dgv.Rows[e.RowIndex];
            seciliId = Convert.ToInt32(satir.Cells["musteri_id"].Value);
            txtAdi.Text = Metin(satir, "adi");
            txtSoyadi.Text = Metin(satir, "soyadi");
            txtTelefon.Text = Metin(satir, "telefon");
            txtEposta.Text = Metin(satir, "eposta");
            txtAdres.Text = Metin(satir, "adres");
            txtSemt.Text = Metin(satir, "semt_ilce");
            txtSehir.Text = Metin(satir, "sehir");
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (!AlanlarGecerliMi()) return;
            try
            {
                Baglanti.Calistir(
                    @"INSERT INTO musteri (adi, soyadi, telefon, eposta, adres, semt_ilce, sehir)
                      VALUES (@adi, @soyadi, @telefon, @eposta, @adres, @semt, @sehir)",
                    Parametreler(false));
                Listele();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliId == 0)
            {
                MessageBox.Show("Önce listeden bir müşteri seçin.");
                return;
            }
            if (!AlanlarGecerliMi()) return;
            try
            {
                Baglanti.Calistir(
                    @"UPDATE musteri SET adi=@adi, soyadi=@soyadi, telefon=@telefon,
                                         eposta=@eposta, adres=@adres,
                                         semt_ilce=@semt, sehir=@sehir
                      WHERE musteri_id=@id",
                    Parametreler(true));
                Listele();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (seciliId == 0)
            {
                MessageBox.Show("Önce listeden bir müşteri seçin.");
                return;
            }
            if (MessageBox.Show("Seçili müşteri silinsin mi?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                Baglanti.Calistir(
                    "DELETE FROM musteri WHERE musteri_id=@id",
                    new MySqlParameter("@id", seciliId));
                Listele();
                Temizle();
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                MessageBox.Show("Bu müşterinin satış kayıtları olduğu için silinemez.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}
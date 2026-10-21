#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MarketSatis
{
    public partial class FrmUrun : Form
    {
        private DataGridView dgv;
        private TextBox txtAd, txtBarkod;
        private NumericUpDown nudFiyat;
        private ComboBox cmbKategori;
        private Button btnEkle, btnGuncelle, btnSil, btnTemizle;
        private int seciliId = 0;

        public FrmUrun()
        {
            InitializeComponent();
            ArayuzuOlustur();
            KategorileriYukle();
            Listele();
        }

        private void ArayuzuOlustur()
        {
            Text = "Ürün İşlemleri";
            ClientSize = new Size(760, 500);
            StartPosition = FormStartPosition.CenterScreen;

            var lblAd = new Label { Text = "Ürün Adı:", Left = 20, Top = 23, AutoSize = true };
            txtAd = new TextBox { Left = 100, Top = 20, Width = 220 };

            var lblBarkod = new Label { Text = "Barkod:", Left = 20, Top = 58, AutoSize = true };
            txtBarkod = new TextBox { Left = 100, Top = 55, Width = 220, MaxLength = 50 };

            var lblFiyat = new Label { Text = "Fiyat (TL):", Left = 20, Top = 93, AutoSize = true };
            nudFiyat = new NumericUpDown
            {
                Left = 100,
                Top = 90,
                Width = 220,
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1000000,
                ThousandsSeparator = true
            };

            var lblKategori = new Label { Text = "Kategori:", Left = 350, Top = 23, AutoSize = true };
            cmbKategori = new ComboBox
            {
                Left = 420,
                Top = 20,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnEkle = new Button { Text = "Ekle", Left = 350, Top = 70, Width = 120, Height = 30 };
            btnGuncelle = new Button { Text = "Güncelle", Left = 480, Top = 70, Width = 120, Height = 30 };
            btnSil = new Button { Text = "Sil", Left = 350, Top = 108, Width = 120, Height = 30 };
            btnTemizle = new Button { Text = "Temizle", Left = 480, Top = 108, Width = 120, Height = 30 };

            dgv = new DataGridView
            {
                Left = 20,
                Top = 155,
                Width = 720,
                Height = 325,
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
            dgv.CellClick += Dgv_CellClick;

            Controls.AddRange(new Control[]
            {
                lblAd, txtAd, lblBarkod, txtBarkod, lblFiyat, nudFiyat,
                lblKategori, cmbKategori,
                btnEkle, btnGuncelle, btnSil, btnTemizle, dgv
            });
        }

        private void KategorileriYukle()
        {
            var tablo = Baglanti.Getir(
                "SELECT kategori_id, kategori_adi FROM kategori ORDER BY kategori_adi");
            cmbKategori.DataSource = tablo;
            cmbKategori.DisplayMember = "kategori_adi";
            cmbKategori.ValueMember = "kategori_id";
            cmbKategori.SelectedIndex = -1;
        }

        private void Listele()
        {
            var tablo = Baglanti.Getir(
                @"SELECT u.urun_id, u.urun_adi, u.barkod, u.fiyat,
                         u.kategori_id, k.kategori_adi
                  FROM urun u
                  JOIN kategori k ON u.kategori_id = k.kategori_id
                  ORDER BY u.urun_adi");
            dgv.DataSource = tablo;

            dgv.Columns["urun_id"].Visible = false;
            dgv.Columns["kategori_id"].Visible = false;
            dgv.Columns["urun_adi"].HeaderText = "Ürün Adı";
            dgv.Columns["barkod"].HeaderText = "Barkod";
            dgv.Columns["fiyat"].HeaderText = "Fiyat (TL)";
            dgv.Columns["fiyat"].DefaultCellStyle.Format = "N2";
            dgv.Columns["kategori_adi"].HeaderText = "Kategori";
            dgv.ClearSelection();
        }

        private void Temizle()
        {
            seciliId = 0;
            txtAd.Clear();
            txtBarkod.Clear();
            nudFiyat.Value = 0;
            cmbKategori.SelectedIndex = -1;
            dgv.ClearSelection();
            txtAd.Focus();
        }

        private bool AlanlarGecerliMi()
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text))
            {
                MessageBox.Show("Ürün adı boş olamaz.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtBarkod.Text))
            {
                MessageBox.Show("Barkod boş olamaz.");
                return false;
            }
            if (nudFiyat.Value <= 0)
            {
                MessageBox.Show("Fiyat 0'dan büyük olmalıdır.");
                return false;
            }
            if (cmbKategori.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir kategori seçin.");
                return false;
            }
            return true;
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var satir = dgv.Rows[e.RowIndex];
            seciliId = Convert.ToInt32(satir.Cells["urun_id"].Value);
            txtAd.Text = satir.Cells["urun_adi"].Value.ToString();
            txtBarkod.Text = satir.Cells["barkod"].Value.ToString();
            nudFiyat.Value = Convert.ToDecimal(satir.Cells["fiyat"].Value);
            cmbKategori.SelectedValue = Convert.ToInt32(satir.Cells["kategori_id"].Value);
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (!AlanlarGecerliMi()) return;
            try
            {
                Baglanti.Calistir(
                    @"INSERT INTO urun (urun_adi, barkod, fiyat, kategori_id)
                      VALUES (@ad, @barkod, @fiyat, @kat)",
                    new MySqlParameter("@ad", txtAd.Text.Trim()),
                    new MySqlParameter("@barkod", txtBarkod.Text.Trim()),
                    new MySqlParameter("@fiyat", nudFiyat.Value),
                    new MySqlParameter("@kat", Convert.ToInt32(cmbKategori.SelectedValue)));
                Listele();
                Temizle();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Bu barkod zaten kayıtlı.");
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
                MessageBox.Show("Önce listeden bir ürün seçin.");
                return;
            }
            if (!AlanlarGecerliMi()) return;
            try
            {
                Baglanti.Calistir(
                    @"UPDATE urun SET urun_adi=@ad, barkod=@barkod,
                                      fiyat=@fiyat, kategori_id=@kat
                      WHERE urun_id=@id",
                    new MySqlParameter("@ad", txtAd.Text.Trim()),
                    new MySqlParameter("@barkod", txtBarkod.Text.Trim()),
                    new MySqlParameter("@fiyat", nudFiyat.Value),
                    new MySqlParameter("@kat", Convert.ToInt32(cmbKategori.SelectedValue)),
                    new MySqlParameter("@id", seciliId));
                Listele();
                Temizle();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Bu barkod başka bir üründe kullanılıyor.");
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
                MessageBox.Show("Önce listeden bir ürün seçin.");
                return;
            }
            if (MessageBox.Show("Seçili ürün silinsin mi?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                Baglanti.Calistir(
                    "DELETE FROM urun WHERE urun_id=@id",
                    new MySqlParameter("@id", seciliId));
                Listele();
                Temizle();
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                MessageBox.Show("Bu ürün satışlarda kullanıldığı için silinemez.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}
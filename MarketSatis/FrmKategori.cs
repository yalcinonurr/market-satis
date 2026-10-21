using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MarketSatis
{
    public partial class FrmKategori : Form
    {
        private DataGridView dgv;
        private TextBox txtAd, txtKod;
        private Button btnEkle, btnGuncelle, btnSil, btnTemizle;
        private int seciliId = 0;

        public FrmKategori()
        {
            InitializeComponent();
            ArayuzuOlustur();
            Listele();
        }

        private void ArayuzuOlustur()
        {
            Text = "Kategori İşlemleri";
            ClientSize = new Size(620, 460);
            StartPosition = FormStartPosition.CenterScreen;

            var lblAd = new Label { Text = "Kategori Adı:", Left = 20, Top = 23, AutoSize = true };
            txtAd = new TextBox { Left = 120, Top = 20, Width = 200 };

            var lblKod = new Label { Text = "Kategori Kodu:", Left = 20, Top = 58, AutoSize = true };
            txtKod = new TextBox { Left = 120, Top = 55, Width = 200 };

            btnEkle = new Button { Text = "Ekle", Left = 350, Top = 18, Width = 110 };
            btnGuncelle = new Button { Text = "Güncelle", Left = 470, Top = 18, Width = 110 };
            btnSil = new Button { Text = "Sil", Left = 350, Top = 53, Width = 110 };
            btnTemizle = new Button { Text = "Temizle", Left = 470, Top = 53, Width = 110 };

            dgv = new DataGridView
            {
                Left = 20,
                Top = 100,
                Width = 580,
                Height = 340,
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
                lblAd, txtAd, lblKod, txtKod,
                btnEkle, btnGuncelle, btnSil, btnTemizle, dgv
            });
        }

        private void Listele()
        {
            var tablo = Baglanti.Getir(
                "SELECT kategori_id, kategori_adi, kategori_kodu FROM kategori ORDER BY kategori_adi");
            dgv.DataSource = tablo;

            dgv.Columns["kategori_id"].Visible = false;
            dgv.Columns["kategori_adi"].HeaderText = "Kategori Adı";
            dgv.Columns["kategori_kodu"].HeaderText = "Kategori Kodu";
            dgv.ClearSelection();
        }

        private void Temizle()
        {
            seciliId = 0;
            txtAd.Clear();
            txtKod.Clear();
            dgv.ClearSelection();
            txtAd.Focus();
        }

        private bool AlanlarGecerliMi()
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text) || string.IsNullOrWhiteSpace(txtKod.Text))
            {
                MessageBox.Show("Kategori adı ve kodu boş olamaz.");
                return false;
            }
            return true;
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var satir = dgv.Rows[e.RowIndex];
            seciliId = Convert.ToInt32(satir.Cells["kategori_id"].Value);
            txtAd.Text = satir.Cells["kategori_adi"].Value.ToString();
            txtKod.Text = satir.Cells["kategori_kodu"].Value.ToString();
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (!AlanlarGecerliMi()) return;
            try
            {
                Baglanti.Calistir(
                    "INSERT INTO kategori (kategori_adi, kategori_kodu) VALUES (@ad, @kod)",
                    new MySqlParameter("@ad", txtAd.Text.Trim()),
                    new MySqlParameter("@kod", txtKod.Text.Trim().ToUpper()));
                Listele();
                Temizle();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Bu kategori kodu zaten kayıtlı.");
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
                MessageBox.Show("Önce listeden bir kategori seçin.");
                return;
            }
            if (!AlanlarGecerliMi()) return;
            try
            {
                Baglanti.Calistir(
                    "UPDATE kategori SET kategori_adi=@ad, kategori_kodu=@kod WHERE kategori_id=@id",
                    new MySqlParameter("@ad", txtAd.Text.Trim()),
                    new MySqlParameter("@kod", txtKod.Text.Trim().ToUpper()),
                    new MySqlParameter("@id", seciliId));
                Listele();
                Temizle();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Bu kategori kodu başka bir kategoride kullanılıyor.");
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
                MessageBox.Show("Önce listeden bir kategori seçin.");
                return;
            }
            if (MessageBox.Show("Seçili kategori silinsin mi?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                Baglanti.Calistir(
                    "DELETE FROM kategori WHERE kategori_id=@id",
                    new MySqlParameter("@id", seciliId));
                Listele();
                Temizle();
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                MessageBox.Show("Bu kategoriye bağlı ürünler var, önce onları silin veya başka kategoriye taşıyın.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}
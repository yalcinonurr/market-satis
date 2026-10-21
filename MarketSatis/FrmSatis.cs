#nullable disable
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MarketSatis
{
    public partial class FrmSatis : Form
    {
        private DataGridView dgv;
        private TextBox txtBarkod;
        private ComboBox cmbUrun, cmbMusteri, cmbOdeme;
        private NumericUpDown nudAdet;
        private DateTimePicker dtpTarih;
        private Label lblFiyat, lblToplam;
        private Button btnKaydet, btnSil, btnYenile;
        private DataTable urunTablosu;
        private int seciliSatisId = 0;

        public FrmSatis()
        {
            InitializeComponent();
            ArayuzuOlustur();
            UrunleriYukle();
            MusterileriYukle();
            Listele();
            FiyatiGuncelle();
        }

        private void ArayuzuOlustur()
        {
            Text = "Satış İşlemleri";
            ClientSize = new Size(940, 640);
            StartPosition = FormStartPosition.CenterScreen;

            var lblBarkod = new Label { Text = "Barkod (Enter):", Left = 20, Top = 23, AutoSize = true };
            txtBarkod = new TextBox { Left = 130, Top = 20, Width = 250 };

            var lblUrun = new Label { Text = "Ürün:", Left = 20, Top = 58, AutoSize = true };
            cmbUrun = new ComboBox { Left = 130, Top = 55, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblMusteri = new Label { Text = "Müşteri:", Left = 20, Top = 93, AutoSize = true };
            cmbMusteri = new ComboBox { Left = 130, Top = 90, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblAdet = new Label { Text = "Adet:", Left = 420, Top = 23, AutoSize = true };
            nudAdet = new NumericUpDown { Left = 530, Top = 20, Width = 120, Minimum = 1, Maximum = 10000, Value = 1 };

            var lblOdeme = new Label { Text = "Ödeme Yöntemi:", Left = 420, Top = 58, AutoSize = true };
            cmbOdeme = new ComboBox { Left = 530, Top = 55, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbOdeme.Items.AddRange(new object[] { "Nakit", "Kredi Kartı", "Banka Kartı", "Havale/EFT", "Yemek Kartı" });
            cmbOdeme.SelectedIndex = 0;

            var lblTarih = new Label { Text = "Tarih:", Left = 420, Top = 93, AutoSize = true };
            dtpTarih = new DateTimePicker
            {
                Left = 530,
                Top = 90,
                Width = 150,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd.MM.yyyy HH:mm",
                Value = DateTime.Now
            };

            var lblFiyatBaslik = new Label { Text = "Birim Fiyat:", Left = 20, Top = 135, AutoSize = true };
            lblFiyat = new Label { Text = "0,00 TL", Left = 130, Top = 135, AutoSize = true, Font = new Font(Font, FontStyle.Bold) };

            var lblToplamBaslik = new Label { Text = "Toplam:", Left = 420, Top = 135, AutoSize = true };
            lblToplam = new Label { Text = "0,00 TL", Left = 530, Top = 130, AutoSize = true, Font = new Font(Font.FontFamily, 14, FontStyle.Bold) };

            btnKaydet = new Button { Text = "Satışı Kaydet", Left = 720, Top = 18, Width = 190, Height = 40 };
            btnSil = new Button { Text = "Seçili Satışı Sil", Left = 720, Top = 68, Width = 190, Height = 32 };
            btnYenile = new Button { Text = "Yenile", Left = 720, Top = 108, Width = 190, Height = 32 };

            dgv = new DataGridView
            {
                Left = 20,
                Top = 175,
                Width = 900,
                Height = 445,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            txtBarkod.KeyDown += TxtBarkod_KeyDown;
            cmbUrun.SelectedIndexChanged += (s, e) => FiyatiGuncelle();
            nudAdet.ValueChanged += (s, e) => FiyatiGuncelle();
            btnKaydet.Click += BtnKaydet_Click;
            btnSil.Click += BtnSil_Click;
            btnYenile.Click += (s, e) =>
            {
                UrunleriYukle();
                MusterileriYukle();
                Listele();
                FiyatiGuncelle();
            };
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                seciliSatisId = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["satis_id"].Value);
            };

            Controls.AddRange(new Control[]
            {
                lblBarkod, txtBarkod, lblUrun, cmbUrun, lblMusteri, cmbMusteri,
                lblAdet, nudAdet, lblOdeme, cmbOdeme, lblTarih, dtpTarih,
                lblFiyatBaslik, lblFiyat, lblToplamBaslik, lblToplam,
                btnKaydet, btnSil, btnYenile, dgv
            });
        }

        private void UrunleriYukle()
        {
            urunTablosu = Baglanti.Getir(
                @"SELECT urun_id, barkod, fiyat,
                         CONCAT(urun_adi, ' (', barkod, ')') AS urun_goster
                  FROM urun ORDER BY urun_adi");
            cmbUrun.DataSource = urunTablosu;
            cmbUrun.DisplayMember = "urun_goster";
            cmbUrun.ValueMember = "urun_id";
            cmbUrun.SelectedIndex = -1;
        }

        private void MusterileriYukle()
        {
            var tablo = Baglanti.Getir(
                @"SELECT musteri_id,
                         CONCAT(adi, ' ', soyadi, IFNULL(CONCAT(' - ', telefon), '')) AS musteri_goster
                  FROM musteri ORDER BY adi, soyadi");
            cmbMusteri.DataSource = tablo;
            cmbMusteri.DisplayMember = "musteri_goster";
            cmbMusteri.ValueMember = "musteri_id";
            cmbMusteri.SelectedIndex = -1;
        }

        private decimal SeciliUrunFiyati()
        {
            if (cmbUrun.SelectedItem is DataRowView satir)
                return Convert.ToDecimal(satir["fiyat"]);
            return 0m;
        }

        private void FiyatiGuncelle()
        {
            decimal fiyat = SeciliUrunFiyati();
            lblFiyat.Text = fiyat.ToString("N2") + " TL";
            lblToplam.Text = (fiyat * nudAdet.Value).ToString("N2") + " TL";
        }

        private void TxtBarkod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            string barkod = txtBarkod.Text.Trim();
            if (barkod == "") return;

            foreach (DataRow satir in urunTablosu.Rows)
            {
                if (satir["barkod"].ToString() == barkod)
                {
                    cmbUrun.SelectedValue = Convert.ToInt32(satir["urun_id"]);
                    txtBarkod.Clear();
                    nudAdet.Focus();
                    return;
                }
            }
            MessageBox.Show("Bu barkodla kayıtlı ürün bulunamadı.");
            txtBarkod.SelectAll();
        }

        private void Listele()
        {
            var tablo = Baglanti.Getir(
                @"SELECT s.satis_id, s.satis_tarihi, u.urun_adi,
                         CONCAT(m.adi, ' ', m.soyadi) AS musteri,
                         s.adet, s.satis_fiyati,
                         s.adet * s.satis_fiyati AS toplam,
                         s.odeme_yontemi
                  FROM satis s
                  JOIN urun u    ON s.urun_id = u.urun_id
                  JOIN musteri m ON s.musteri_id = m.musteri_id
                  ORDER BY s.satis_tarihi DESC, s.satis_id DESC
                  LIMIT 200");
            dgv.DataSource = tablo;

            dgv.Columns["satis_id"].Visible = false;
            dgv.Columns["satis_tarihi"].HeaderText = "Tarih";
            dgv.Columns["satis_tarihi"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgv.Columns["urun_adi"].HeaderText = "Ürün";
            dgv.Columns["musteri"].HeaderText = "Müşteri";
            dgv.Columns["adet"].HeaderText = "Adet";
            dgv.Columns["satis_fiyati"].HeaderText = "Birim Fiyat";
            dgv.Columns["satis_fiyati"].DefaultCellStyle.Format = "N2";
            dgv.Columns["toplam"].HeaderText = "Toplam";
            dgv.Columns["toplam"].DefaultCellStyle.Format = "N2";
            dgv.Columns["odeme_yontemi"].HeaderText = "Ödeme";
            dgv.ClearSelection();
            seciliSatisId = 0;
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (cmbUrun.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir ürün seçin.");
                return;
            }
            if (cmbMusteri.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir müşteri seçin.");
                return;
            }

            try
            {
                Baglanti.Calistir(
                    @"INSERT INTO satis (urun_id, musteri_id, satis_tarihi, satis_fiyati, adet, odeme_yontemi)
                      VALUES (@urun, @musteri, @tarih, @fiyat, @adet, @odeme)",
                    new MySqlParameter("@urun", Convert.ToInt32(cmbUrun.SelectedValue)),
                    new MySqlParameter("@musteri", Convert.ToInt32(cmbMusteri.SelectedValue)),
                    new MySqlParameter("@tarih", dtpTarih.Value),
                    new MySqlParameter("@fiyat", SeciliUrunFiyati()),
                    new MySqlParameter("@adet", (int)nudAdet.Value),
                    new MySqlParameter("@odeme", cmbOdeme.SelectedItem.ToString()));

                Listele();

                // Yeni satış için formu hazırla (müşteri ve ödeme yöntemi aynı kalır)
                cmbUrun.SelectedIndex = -1;
                nudAdet.Value = 1;
                dtpTarih.Value = DateTime.Now;
                FiyatiGuncelle();
                txtBarkod.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (seciliSatisId == 0)
            {
                MessageBox.Show("Önce listeden bir satış seçin.");
                return;
            }
            if (MessageBox.Show("Seçili satış kaydı silinsin mi?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                Baglanti.Calistir(
                    "DELETE FROM satis WHERE satis_id=@id",
                    new MySqlParameter("@id", seciliSatisId));
                Listele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}
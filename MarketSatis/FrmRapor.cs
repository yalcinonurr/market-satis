#nullable disable
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MarketSatis
{
    public partial class FrmRapor : Form
    {
        private DateTimePicker dtpBaslangic, dtpBitis;
        private ComboBox cmbRapor;
        private Button btnGetir;
        private DataGridView dgv;
        private Label lblOzet;

        public FrmRapor()
        {
            InitializeComponent();
            ArayuzuOlustur();
            RaporuGetir();
        }

        private void ArayuzuOlustur()
        {
            Text = "Raporlar";
            ClientSize = new Size(920, 600);
            StartPosition = FormStartPosition.CenterScreen;

            var lblBas = new Label { Text = "Başlangıç:", Left = 20, Top = 23, AutoSize = true };
            dtpBaslangic = new DateTimePicker
            {
                Left = 95,
                Top = 20,
                Width = 130,
                Format = DateTimePickerFormat.Short,
                Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            };

            var lblBit = new Label { Text = "Bitiş:", Left = 245, Top = 23, AutoSize = true };
            dtpBitis = new DateTimePicker
            {
                Left = 290,
                Top = 20,
                Width = 130,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            var lblRapor = new Label { Text = "Rapor:", Left = 440, Top = 23, AutoSize = true };
            cmbRapor = new ComboBox { Left = 490, Top = 20, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRapor.Items.AddRange(new object[]
            {
                "Satış detayı",
                "Günlük satış toplamı",
                "Ödeme yöntemine göre",
                "En çok satan ürünler",
                "Müşteri bazlı satışlar",
                "Kategori bazlı satışlar"
            });
            cmbRapor.SelectedIndex = 0;

            btnGetir = new Button { Text = "Raporu Getir", Left = 760, Top = 17, Width = 140, Height = 30 };

            lblOzet = new Label
            {
                Left = 20,
                Top = 60,
                Width = 880,
                Height = 30,
                Font = new Font(Font.FontFamily, 11, FontStyle.Bold),
                Text = ""
            };

            dgv = new DataGridView
            {
                Left = 20,
                Top = 100,
                Width = 880,
                Height = 480,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            btnGetir.Click += (s, e) => RaporuGetir();
            cmbRapor.SelectedIndexChanged += (s, e) => RaporuGetir();

            Controls.AddRange(new Control[]
            {
                lblBas, dtpBaslangic, lblBit, dtpBitis, lblRapor, cmbRapor,
                btnGetir, lblOzet, dgv
            });
        }

        private string RaporSorgusu()
        {
            // Hepsinde ortak filtre: tarih aralığı (@bas dahil, @bit hariç)
            string filtre = "s.satis_tarihi >= @bas AND s.satis_tarihi < @bit";

            switch (cmbRapor.SelectedIndex)
            {
                case 0: // Satış detayı
                    return $@"SELECT s.satis_tarihi AS `Tarih`, u.urun_adi AS `Ürün`,
                                     CONCAT(m.adi, ' ', m.soyadi) AS `Müşteri`,
                                     s.adet AS `Adet`, s.satis_fiyati AS `Birim Fiyat`,
                                     s.adet * s.satis_fiyati AS `Toplam`,
                                     s.odeme_yontemi AS `Ödeme`
                              FROM satis s
                              JOIN urun u    ON s.urun_id = u.urun_id
                              JOIN musteri m ON s.musteri_id = m.musteri_id
                              WHERE {filtre}
                              ORDER BY s.satis_tarihi DESC";

                case 1: // Günlük toplam
                    return $@"SELECT DATE(s.satis_tarihi) AS `Tarih`,
                                     COUNT(*) AS `Satış Sayısı`,
                                     CAST(SUM(s.adet) AS SIGNED) AS `Toplam Adet`,
                                     SUM(s.adet * s.satis_fiyati) AS `Toplam Tutar`
                              FROM satis s
                              WHERE {filtre}
                              GROUP BY DATE(s.satis_tarihi)
                              ORDER BY DATE(s.satis_tarihi) DESC";

                case 2: // Ödeme yöntemi
                    return $@"SELECT s.odeme_yontemi AS `Ödeme Yöntemi`,
                                     COUNT(*) AS `Satış Sayısı`,
                                     SUM(s.adet * s.satis_fiyati) AS `Toplam Tutar`
                              FROM satis s
                              WHERE {filtre}
                              GROUP BY s.odeme_yontemi
                              ORDER BY `Toplam Tutar` DESC";

                case 3: // En çok satan ürünler
                    return $@"SELECT u.urun_adi AS `Ürün`, u.barkod AS `Barkod`,
                                     CAST(SUM(s.adet) AS SIGNED) AS `Satılan Adet`,
                                     SUM(s.adet * s.satis_fiyati) AS `Toplam Tutar`
                              FROM satis s
                              JOIN urun u ON s.urun_id = u.urun_id
                              WHERE {filtre}
                              GROUP BY u.urun_id, u.urun_adi, u.barkod
                              ORDER BY `Satılan Adet` DESC";

                case 4: // Müşteri bazlı
                    return $@"SELECT CONCAT(m.adi, ' ', m.soyadi) AS `Müşteri`,
                                     m.telefon AS `Telefon`,
                                     COUNT(*) AS `Satış Sayısı`,
                                     SUM(s.adet * s.satis_fiyati) AS `Toplam Tutar`
                              FROM satis s
                              JOIN musteri m ON s.musteri_id = m.musteri_id
                              WHERE {filtre}
                              GROUP BY m.musteri_id, m.adi, m.soyadi, m.telefon
                              ORDER BY `Toplam Tutar` DESC";

                default: // Kategori bazlı
                    return $@"SELECT k.kategori_adi AS `Kategori`,
                                     CAST(SUM(s.adet) AS SIGNED) AS `Satılan Adet`,
                                     SUM(s.adet * s.satis_fiyati) AS `Toplam Tutar`
                              FROM satis s
                              JOIN urun u     ON s.urun_id = u.urun_id
                              JOIN kategori k ON u.kategori_id = k.kategori_id
                              WHERE {filtre}
                              GROUP BY k.kategori_id, k.kategori_adi
                              ORDER BY `Toplam Tutar` DESC";
            }
        }

        private void RaporuGetir()
        {
            if (cmbRapor == null || dgv == null) return;

            if (dtpBaslangic.Value.Date > dtpBitis.Value.Date)
            {
                MessageBox.Show("Başlangıç tarihi bitiş tarihinden büyük olamaz.");
                return;
            }

            var bas = new MySqlParameter("@bas", dtpBaslangic.Value.Date);
            var bit = new MySqlParameter("@bit", dtpBitis.Value.Date.AddDays(1));

            try
            {
                var tablo = Baglanti.Getir(RaporSorgusu(), bas, bit);
                dgv.DataSource = tablo;

                foreach (DataGridViewColumn kolon in dgv.Columns)
                {
                    if (kolon.ValueType == typeof(decimal))
                    {
                        kolon.DefaultCellStyle.Format = "N2";
                        kolon.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    else if (kolon.ValueType == typeof(DateTime))
                    {
                        kolon.DefaultCellStyle.Format =
                            cmbRapor.SelectedIndex == 0 ? "dd.MM.yyyy HH:mm" : "dd.MM.yyyy";
                    }
                }
                dgv.ClearSelection();

                OzetiGuncelle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void OzetiGuncelle()
        {
            // Özet parametreleri yeniden oluşturulmalı (bir parametre iki komutta kullanılamaz)
            var ozet = Baglanti.Getir(
                @"SELECT IFNULL(SUM(adet * satis_fiyati), 0) AS ciro,
                         IFNULL(CAST(SUM(adet) AS SIGNED), 0) AS adet,
                         COUNT(*) AS sayi
                  FROM satis
                  WHERE satis_tarihi >= @bas AND satis_tarihi < @bit",
                new MySqlParameter("@bas", dtpBaslangic.Value.Date),
                new MySqlParameter("@bit", dtpBitis.Value.Date.AddDays(1)));

            var r = ozet.Rows[0];
            lblOzet.Text =
                $"Toplam Ciro: {Convert.ToDecimal(r["ciro"]):N2} TL     " +
                $"Satılan Adet: {r["adet"]}     " +
                $"Satış Sayısı: {r["sayi"]}";
        }
    }
}
#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarketSatis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Text = "Market Satýþ Programý";
            ClientSize = new Size(320, 330);
            StartPosition = FormStartPosition.CenterScreen;

            Button MenuButonu(string metin, int ust, Func<Form> formUret)
            {
                var b = new Button { Text = metin, Left = 60, Top = ust, Width = 200, Height = 40 };
                b.Click += (s, e) => formUret().ShowDialog();
                Controls.Add(b);
                return b;
            }

            MenuButonu("Kategoriler", 30, () => new FrmKategori());
            MenuButonu("Ürünler", 85, () => new FrmUrun());
            MenuButonu("Müþteriler", 140, () => new FrmMusteri());
            MenuButonu("Satýþ", 195, () => new FrmSatis());
            MenuButonu("Raporlar", 250, () => new FrmRapor());
        }
    }
}
using MySql.Data.MySqlClient;
using System.Data;

namespace MarketSatis
{
    public static class Baglanti
    {
        // Şifreyi kendi MySQL root şifrenle değiştir
        private static string baglantiMetni =
            "Server=localhost;Port=3306;Database=market_db;Uid=root;Pwd=;CharSet=utf8mb4;";

        public static MySqlConnection BaglantiAl()
        {
            return new MySqlConnection(baglantiMetni);
        }

        // SELECT sorguları için: sonucu DataTable olarak döndürür
        public static DataTable Getir(string sql, params MySqlParameter[] parametreler)
        {
            using (var baglanti = BaglantiAl())
            using (var komut = new MySqlCommand(sql, baglanti))
            {
                komut.Parameters.AddRange(parametreler);
                var tablo = new DataTable();
                new MySqlDataAdapter(komut).Fill(tablo);
                return tablo;
            }
        }

        // INSERT, UPDATE, DELETE için: etkilenen satır sayısını döndürür
        public static int Calistir(string sql, params MySqlParameter[] parametreler)
        {
            using (var baglanti = BaglantiAl())
            using (var komut = new MySqlCommand(sql, baglanti))
            {
                komut.Parameters.AddRange(parametreler);
                baglanti.Open();
                return komut.ExecuteNonQuery();
            }
        }
    }
}
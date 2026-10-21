-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Anamakine: 127.0.0.1
-- Üretim Zamanı: 21 Eki 2026, 09:09:06
-- Sunucu sürümü: 10.4.24-MariaDB
-- PHP Sürümü: 8.1.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Veritabanı: `market_db`
--
CREATE DATABASE IF NOT EXISTS `market_db` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_turkish_ci;
USE `market_db`;

-- --------------------------------------------------------

--
-- Tablo için tablo yapısı `kategori`
--

CREATE TABLE `kategori` (
  `kategori_id` int(11) NOT NULL,
  `kategori_adi` varchar(100) COLLATE utf8mb4_turkish_ci NOT NULL,
  `kategori_kodu` varchar(20) COLLATE utf8mb4_turkish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci;

--
-- Tablo döküm verisi `kategori`
--

INSERT INTO `kategori` (`kategori_id`, `kategori_adi`, `kategori_kodu`) VALUES
(1, 'Süt Ürünleri', 'SUT'),
(2, 'İçecek', 'ICE'),
(3, 'Temizlik', 'TEM');

-- --------------------------------------------------------

--
-- Tablo için tablo yapısı `musteri`
--

CREATE TABLE `musteri` (
  `musteri_id` int(11) NOT NULL,
  `adi` varchar(50) COLLATE utf8mb4_turkish_ci NOT NULL,
  `soyadi` varchar(50) COLLATE utf8mb4_turkish_ci NOT NULL,
  `telefon` varchar(20) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `eposta` varchar(100) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `adres` varchar(255) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `semt_ilce` varchar(50) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `sehir` varchar(50) COLLATE utf8mb4_turkish_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci;

--
-- Tablo döküm verisi `musteri`
--

INSERT INTO `musteri` (`musteri_id`, `adi`, `soyadi`, `telefon`, `eposta`, `adres`, `semt_ilce`, `sehir`) VALUES
(1, 'Ayşe', 'Yılmaz', '05320000000', 'ayse@mail.com', 'Gül Sk. No:5', 'Kadıköy', 'İstanbul');

-- --------------------------------------------------------

--
-- Tablo için tablo yapısı `satis`
--

CREATE TABLE `satis` (
  `satis_id` int(11) NOT NULL,
  `urun_id` int(11) NOT NULL,
  `musteri_id` int(11) NOT NULL,
  `satis_tarihi` datetime NOT NULL DEFAULT current_timestamp(),
  `satis_fiyati` decimal(10,2) NOT NULL,
  `adet` int(11) NOT NULL DEFAULT 1,
  `odeme_yontemi` varchar(30) COLLATE utf8mb4_turkish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci;

--
-- Tablo döküm verisi `satis`
--

INSERT INTO `satis` (`satis_id`, `urun_id`, `musteri_id`, `satis_tarihi`, `satis_fiyati`, `adet`, `odeme_yontemi`) VALUES
(1, 1, 1, '2026-10-21 09:15:17', '32.50', 2, 'Nakit');

-- --------------------------------------------------------

--
-- Tablo için tablo yapısı `urun`
--

CREATE TABLE `urun` (
  `urun_id` int(11) NOT NULL,
  `urun_adi` varchar(150) COLLATE utf8mb4_turkish_ci NOT NULL,
  `barkod` varchar(50) COLLATE utf8mb4_turkish_ci NOT NULL,
  `fiyat` decimal(10,2) NOT NULL,
  `kategori_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci;

--
-- Tablo döküm verisi `urun`
--

INSERT INTO `urun` (`urun_id`, `urun_adi`, `barkod`, `fiyat`, `kategori_id`) VALUES
(1, 'Süt 1 L', '8690000000011', '32.50', 1),
(2, 'Kola 1 L', '8690000000028', '45.00', 2),
(3, 'Çamaşır Suyu', '8690000000035', '60.00', 3);

--
-- Dökümü yapılmış tablolar için indeksler
--

--
-- Tablo için indeksler `kategori`
--
ALTER TABLE `kategori`
  ADD PRIMARY KEY (`kategori_id`),
  ADD UNIQUE KEY `kategori_kodu` (`kategori_kodu`);

--
-- Tablo için indeksler `musteri`
--
ALTER TABLE `musteri`
  ADD PRIMARY KEY (`musteri_id`);

--
-- Tablo için indeksler `satis`
--
ALTER TABLE `satis`
  ADD PRIMARY KEY (`satis_id`),
  ADD KEY `urun_id` (`urun_id`),
  ADD KEY `musteri_id` (`musteri_id`);

--
-- Tablo için indeksler `urun`
--
ALTER TABLE `urun`
  ADD PRIMARY KEY (`urun_id`),
  ADD UNIQUE KEY `barkod` (`barkod`),
  ADD KEY `kategori_id` (`kategori_id`);

--
-- Dökümü yapılmış tablolar için AUTO_INCREMENT değeri
--

--
-- Tablo için AUTO_INCREMENT değeri `kategori`
--
ALTER TABLE `kategori`
  MODIFY `kategori_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Tablo için AUTO_INCREMENT değeri `musteri`
--
ALTER TABLE `musteri`
  MODIFY `musteri_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Tablo için AUTO_INCREMENT değeri `satis`
--
ALTER TABLE `satis`
  MODIFY `satis_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Tablo için AUTO_INCREMENT değeri `urun`
--
ALTER TABLE `urun`
  MODIFY `urun_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Dökümü yapılmış tablolar için kısıtlamalar
--

--
-- Tablo kısıtlamaları `satis`
--
ALTER TABLE `satis`
  ADD CONSTRAINT `satis_ibfk_1` FOREIGN KEY (`urun_id`) REFERENCES `urun` (`urun_id`),
  ADD CONSTRAINT `satis_ibfk_2` FOREIGN KEY (`musteri_id`) REFERENCES `musteri` (`musteri_id`);

--
-- Tablo kısıtlamaları `urun`
--
ALTER TABLE `urun`
  ADD CONSTRAINT `urun_ibfk_1` FOREIGN KEY (`kategori_id`) REFERENCES `kategori` (`kategori_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

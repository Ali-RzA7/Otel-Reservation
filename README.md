# Otel Rezervasyon Sistemi

Otel Rezervasyon Sistemi, otel odalarının yönetimi, rezervasyon yapılması ve müşteri işlemlerinin gerçekleştirilmesi için geliştirilmiş modern bir uygulamadır. Bu uygulama, otel sahiplerinin ve çalışanlarının odaları yönetmesini, rezervasyonları takip etmesini ve müşteri bilgilerini kaydetmesini sağlar.

## 🛠️ Teknolojiler
- **C#**: Uygulama mantığı ve kullanıcı arayüzü.
- **SQL Server**: Veritabanı yönetimi.
- **Windows Forms**: Masaüstü uygulaması için görsel arayüz.

## 🎯 Proje Özellikleri
- **Kullanıcı Girişi (Login Sayfası)**: Kullanıcılar sisteme giriş yapabilir.
- **Ana Sayfa (Main Page)**: Ana sayfa üzerinden otel odalarına göz atılabilir.
- **Oda Listesi**: Oteldeki tüm odalar görüntülenebilir.
- **Rezervasyon Sayfası**: Oda rezervasyonu yapılabilir.
- **Müşteri Eklemek**: Yeni müşteri bilgileri eklenebilir.
- **Müşteri İşlemleri**: Mevcut müşteri bilgileri güncellenebilir veya silinebilir.

## 🖼️ Ekran Görüntüleri

Aşağıda, sistemin farklı bölümlerine ait ekran görüntüleri:

## 1. **Login Sayfası**:
- **Kullanıcı**, "Giriş Yap" butonuna tıkladığında, `TextBox`'lardan alınan veriler veritabanındaki `Users` tablosu bilgileriyle eşleştirilir.
- **Doğru bilgiler** girildiğinde kullanıcı ana sayfaya yönlendirilir. 
- **Yanlış bilgiler** girilirse hata mesajı görüntülenir.
   ![Login Sayfası](https://github.com/Ali-RzA7/Otel-Reservation/blob/main/Images/login.png)

## 2. **Ana Sayfa**:
-	Giriş yapan kullanıcının yetki bilgisi (admin veya personel) veritabanından alınır ve `Label` üzerinde gösterilir.
-	Kullanıcı, ilgili butonları kullanarak yetkisi dahilinde **müşteri işlemleri** ve **oda işlemlerini** gerçekleştirebilir.

   ![Ana Sayfa](https://github.com/Ali-RzA7/Otel-Reservation/blob/main/Images/main.png)

 ## 3. **Oda Listeleme Sayfası**:

### Oda Görselleştirme

#### • Oda Durumu Gösterimi:
Odalar, `FlowLayoutPanel` içerisinde kutucuklar olarak görselleştirilmiştir. Her kutucuk şu bilgileri içerir:
- **Oda Numarası**: Kutucuk üzerinde gösterilir.
- **Durum Rengi**:
  - **Kırmızı**: Dolu odalar.
  - **Yeşil**: Boş odalar.

### Dolu Oda İşlemleri

#### • Oda Kutucuğuna Tıklama:
- **Açılan Ekran**: Dolu odalara tıklanıldığında, o odada kalan müşterileri listeleyen bir pencere açılır.
- **Bilgi Görüntüleme**: Bu pencere şunları sağlar:
  - Mevcut müşterilerin bilgilerini listeleme (Ad, Soyad, Giriş/Çıkış Tarihi vb.).
  - **Servis Ekleme**: Bu müşteri için ek hizmet (örn. oda servisi, çamaşırhane) ekleme seçeneği.

### Boş Oda İşlemleri

#### • Oda Kutucuğuna Tıklama:
- **Açılan Form**: Rezervasyon yapmak için kullanılan bir form açılır.
  
   ![Oda Listesi](https://github.com/Ali-RzA7/Otel-Reservation/blob/main/Images/rooms.png)

4. **Rezervasyon Sayfası**: Kullanıcıların oda rezervasyonu yapabildiği sayfa.  
   ![Rezervasyon Sayfası](https://github.com/Ali-RzA7/Otel-Reservation/blob/main/Images/rezervation.png)

5. **Müşteri Ekleme Sayfası**: Yeni müşteri ekleme işlemlerini gerçekleştiren sayfa.  
   ![Müşteri Ekleme Sayfası](https://github.com/Ali-RzA7/Otel-Reservation/blob/main/Images/adding.png)

6. **Müşteri İşlemleri Sayfası**: Dolu odalardaki müşteri bilgilerini gösterne sayfa.  
   ![Müşteri İşlemleri Sayfası](https://github.com/Ali-RzA7/Otel-Reservation/blob/main/Images/detail.png)

7. **Müşteri İşlemleri Sayfası**: Var olan müşteri bilgileri üzerinde işlem yapabileceğiniz sayfa. Müşterilerileri listeleme, silme, excele aktarma, excelden veri çekme gibi işlemlerin yapılmasını sağlar.  
   ![Müşteri İşlemleri Sayfası](https://github.com/Ali-RzA7/Otel-Reservation/blob/main/Images/operations.png)


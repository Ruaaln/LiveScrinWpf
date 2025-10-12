# 🖥️ Live Screen TCP

Real-time ekran paylaşımı üçün hazırlanmış **C# TCP Server & WPF Client** tətbiqi.

Server ekran görüntülərini TCP üzərindən göndərir, Client isə onları canlı olaraq göstərir.

---

## 📸 Ekran görüntüsü

![LiveScreenPreview](Image)

---

## ⚙️ Quruluş

### 🔹 Server (Console App)
- Run zamanı IP və Port soruşur.
- Əgər boş buraxılsa, avtomatik olaraq `127.0.0.1:8080` ünvanına qoşulur.
- Ekran görüntüsünü periodik olaraq TCP vasitəsilə göndərir.

### 🔹 Client (WPF App)
- `Start` düyməsi ilə qoşulma baş verir.
- `Stop` düyməsi ilə əlaqə sonlandırılır.
- Canlı ekran görüntüsü `ImageBox` içində göstərilir.
- Mesajlar ekranın yuxarısında `ShowMessage()` metodu ilə göstərilir.

---

## 🧩 Texnologiyalar
- **.NET 8.0**
- **C# (WPF + Console)**
- **System.Net.Sockets**
- **System.Drawing**
- **Async/Thread-based streaming**

---

## 🚀 İşlədilməsi

### 🖥️ Server
```bash
cd Server
dotnet run

using Kitaphanem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Kitaphanem.Controllers
{
    public class BookController : Controller
    {
        private readonly string _bookFile = "wwwroot/data/book.json";  // Kitap verilerinin kaydedileceği dosya

        // Kitapları Gösterme Sayfası (GET)
        public IActionResult Index()
        {
            var books = new List<Book>();

            // Kitaplar dosyasını okuma
            if (System.IO.File.Exists(_bookFile))
            {
                var jsonData = System.IO.File.ReadAllText(_bookFile);
                var options = new JsonSerializerOptions { AllowTrailingCommas = true };
                books = JsonSerializer.Deserialize<List<Book>>(jsonData, options) ?? new List<Book>();
            }

            return View(books); // Kitapları View'a gönder
        }

        // Kitap Ekleme Sayfası (GET)
        [HttpGet]
        public IActionResult AddBook()
        {
            return View();
        }

        // Kitap Ekleme İşlemi (POST)
        [HttpPost]
        public IActionResult AddBook(Book book)
        {
            if (ModelState.IsValid)
            {
                var books = new List<Book>();

                // Kitapları okuma
                if (System.IO.File.Exists(_bookFile))
                {
                    var jsonData = System.IO.File.ReadAllText(_bookFile);
                    var options = new JsonSerializerOptions { AllowTrailingCommas = true };
                    books = JsonSerializer.Deserialize<List<Book>>(jsonData, options) ?? new List<Book>();
                }

                // Yeni kitabın ID'sini belirleyelim
                int newId = books.Any() ? books.Max(b => b.Id) + 1 : 1;
                book.Id = newId;  // Kitabın ID'sini ekle

                // Yeni kitabı listeye ekle
                books.Add(book);

                // Güncellenmiş listeyi JSON formatında kaydet
                var updatedJson = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(_bookFile, updatedJson);

                return RedirectToAction("Index"); // Kitap eklendikten sonra kitapların listelendiği sayfaya yönlendir
            }

            return View(book); // Hata varsa aynı sayfayı yeniden göster
        }

        // Kitap Detay Sayfası (GET)
        public IActionResult Details(int id)
        {
            var books = new List<Book>();

            // Kitapları okuma
            if (System.IO.File.Exists(_bookFile))
            {
                var jsonData = System.IO.File.ReadAllText(_bookFile);
                var options = new JsonSerializerOptions { AllowTrailingCommas = true };
                books = JsonSerializer.Deserialize<List<Book>>(jsonData, options) ?? new List<Book>();
            }

            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();  // Eğer kitap bulunmazsa 404 döndür
            }

            return View(book); // Kitap detayı sayfasına yönlendir
        }

        // Kitap Güncelleme Sayfası (GET)
        public IActionResult EditBook(int id)
        {
            var books = new List<Book>();

            // Kitapları okuma
            if (System.IO.File.Exists(_bookFile))
            {
                var jsonData = System.IO.File.ReadAllText(_bookFile);
                var options = new JsonSerializerOptions { AllowTrailingCommas = true };
                books = JsonSerializer.Deserialize<List<Book>>(jsonData, options) ?? new List<Book>();
            }

            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();  // Kitap bulunamazsa 404 döndür
            }

            return View(book); // Kitap düzenleme sayfasına yönlendir
        }

        // Kitap Güncelleme İşlemi (POST)
        [HttpPost]
        public IActionResult EditBook(Book updatedBook)
        {
            if (ModelState.IsValid)
            {
                var books = new List<Book>();

                // Kitapları okuma
                if (System.IO.File.Exists(_bookFile))
                {
                    var jsonData = System.IO.File.ReadAllText(_bookFile);
                    var options = new JsonSerializerOptions { AllowTrailingCommas = true };
                    books = JsonSerializer.Deserialize<List<Book>>(jsonData, options) ?? new List<Book>();
                }

                var book = books.FirstOrDefault(b => b.Id == updatedBook.Id);
                if (book != null)
                {
                    // Kitap bilgilerini güncelle
                    book.Title = updatedBook.Title;
                    book.Author = updatedBook.Author;
                    book.Category = updatedBook.Category;
                    book.Description = updatedBook.Description;
                    book.price = updatedBook.price;
                    book.ImageUrl = updatedBook.ImageUrl;

                    // Güncellenmiş listeyi JSON formatında kaydet
                    var updatedJson = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(_bookFile, updatedJson);
                }

                return RedirectToAction("Index"); // Kitaplar listelendiği sayfaya geri dön
            }

            return View(updatedBook); // Hata varsa aynı sayfayı yeniden göster
        }

        // Kitap Silme (POST)
        [HttpPost]
        public IActionResult DeleteBook(int id)
        {
            var books = new List<Book>();

            // Kitapları okuma
            if (System.IO.File.Exists(_bookFile))
            {
                var jsonData = System.IO.File.ReadAllText(_bookFile);
                var options = new JsonSerializerOptions { AllowTrailingCommas = true };
                books = JsonSerializer.Deserialize<List<Book>>(jsonData, options) ?? new List<Book>();
            }

            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();  // Kitap bulunamazsa 404 döndür
            }

            books.Remove(book);  // Kitabı sil

            // Güncellenmiş listeyi JSON formatında kaydet
            var updatedJson = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_bookFile, updatedJson);

            return RedirectToAction("Index");  // Kitaplar listelendiği sayfaya yönlendir
        }

        // Favorilere ekleme işlemi (POST)
        [HttpPost]
        public IActionResult AddToFavorites(int id)
        {
            // Kitapları JSON dosyasından okuma
            var books = new List<Book>();
            if (System.IO.File.Exists(_bookFile))
            {
                var jsonData = System.IO.File.ReadAllText(_bookFile);
                var options = new JsonSerializerOptions { AllowTrailingCommas = true };
                books = JsonSerializer.Deserialize<List<Book>>(jsonData, options) ?? new List<Book>();
            }

            // Kitap ID'sine göre kitabı bulma
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();  // Kitap bulunamazsa 404 döndür
            }

            // Favori kitapları session'dan al
            var favoriteBooks = HttpContext.Session.GetObjectFromJson<List<Book>>("FavoriteBooks") ?? new List<Book>();

            // Kitap zaten favorilerde mi kontrol et
            if (!favoriteBooks.Any(b => b.Id == id))
            {
                favoriteBooks.Add(book);  // Favorilere ekle
                HttpContext.Session.SetObjectAsJson("FavoriteBooks", favoriteBooks);  // Güncellenmiş favorileri session'a kaydet
            }

            return RedirectToAction("Favorites");  // Favoriler sayfasına yönlendir
        }

        // Favorileri görüntüleme sayfası (GET)
        public IActionResult Favorites()
        {
            var favoriteBooks = HttpContext.Session.GetObjectFromJson<List<Book>>("FavoriteBooks") ?? new List<Book>();
            return View(favoriteBooks);  // Favori kitapları görüntüle
        }

        [HttpPost]
        public IActionResult RemoveFromFavorites(int id)
        {
            // Favori kitapları session'dan al
            var favoriteBooks = HttpContext.Session.GetObjectFromJson<List<Book>>("FavoriteBooks") ?? new List<Book>();

            // Kitap ID'sine göre kitabı bul
            var book = favoriteBooks.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                favoriteBooks.Remove(book);  // Kitap favorilerden kaldır
                HttpContext.Session.SetObjectAsJson("FavoriteBooks", favoriteBooks);  // Güncellenmiş favorileri session'a kaydet
            }

            return RedirectToAction("Favorites");  // Favoriler sayfasına yönlendir
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var books = new List<Book>();
            if (System.IO.File.Exists(_bookFile))
            {
                var jsonData = System.IO.File.ReadAllText(_bookFile);
                var options = new JsonSerializerOptions { AllowTrailingCommas = true };
                books = JsonSerializer.Deserialize<List<Book>>(jsonData, options) ?? new List<Book>();
            }

            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var cartBooks = HttpContext.Session.GetObjectFromJson<List<Book>>("Cart") ?? new List<Book>();

            if (!cartBooks.Any(b => b.Id == id))
            {
                cartBooks.Add(book);
                HttpContext.Session.SetObjectAsJson("Cart", cartBooks);
            }

            TempData["Message"] = "Kitap sepete eklendi."; // Bilgilendirme mesajı

            return RedirectToAction("Index"); // Ana sayfada kalmaya devam et
        }
        // Sepeti görüntüleme sayfası (GET)
        public IActionResult Cart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Book>>("Cart") ?? new List<Book>();
            return View(cart);  // Sepeti görüntüle
        }

    }
}

using Kitaphanem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Kitaphanem.Controllers
{
    public class BookController : Controller
    {
        private readonly string _bookFile = "wwwroot/data/books.json";

        // Kitap ekleme sayfası
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // Kitap ekleme işlemi
        [HttpPost]
        public IActionResult Add(book book)
        {
            if (ModelState.IsValid)
            {
                var books = new List<book>();

                // Eğer books.json dosyası varsa, içerisindeki verileri oku
                if (System.IO.File.Exists(_bookFile))
                {
                    var jsonData = System.IO.File.ReadAllText(_bookFile);
                    books = JsonSerializer.Deserialize<List<book>>(jsonData);
                }

                // Yeni kitap ekle
                books.Add(book);

                // Güncellenmiş veriyi books.json dosyasına yaz
                var updatedJson = JsonSerializer.Serialize(books);
                System.IO.File.WriteAllText(_bookFile, updatedJson);

                return RedirectToAction("Index", "Home");
            }

            return View(book);
        }
    }
}
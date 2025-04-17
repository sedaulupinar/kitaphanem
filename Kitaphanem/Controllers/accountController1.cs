using Kitaphanem.Models;
using Kitaphanem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IkinciElKitapSatis.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _userFile = "wwwroot/data/users.json";

        // Kayıt Olma Sayfası
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt İşlemi
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var users = new List<User>();

                if (System.IO.File.Exists(_userFile))
                {
                    var jsonData = System.IO.File.ReadAllText(_userFile);
                    users = JsonSerializer.Deserialize<List<User>>(jsonData);
                }

                users.Add(user);
                var updatedJson = JsonSerializer.Serialize(users);
                System.IO.File.WriteAllText(_userFile, updatedJson);

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Giriş Sayfası
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Giriş Yapma İşlemi
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (System.IO.File.Exists(_userFile))
            {
                var jsonData = System.IO.File.ReadAllText(_userFile);
                var users = JsonSerializer.Deserialize<List<User>>(jsonData);

                var user = users.FirstOrDefault(x => x.Email == email && x.şifre == password);

                if (user != null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "E-posta veya şifre hatalı.";
            return View();
        }
    }
}
using Kitaphanem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Linq;

namespace IkinciElKitapSatis.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _userFile = "wwwroot/data/users.json";
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        // Kayıt Olma Sayfası (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt İşlemi (POST)
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Şifre güvenlik kontrolü
                if (string.IsNullOrWhiteSpace(user.PasswordHash) ||
                    user.PasswordHash.Length < 8 ||
                    !user.PasswordHash.Any(char.IsLetter) ||
                    !user.PasswordHash.Any(char.IsDigit))
                {
                    ViewBag.Error = "Şifreniz en az 8 karakter olmalı ve hem harf hem rakam içermelidir.";
                    return View(user);
                }

                var users = new List<User>();

                if (System.IO.File.Exists(_userFile))
                {
                    var jsonData = System.IO.File.ReadAllText(_userFile);
                    users = JsonSerializer.Deserialize<List<User>>(jsonData);
                }

                if (users.Any(x => x.Email == user.Email))
                {
                    ViewBag.Error = "Bu e-posta zaten kayıtlı.";
                    return View(user);
                }

                // Güçlü şifreyi hashle
                user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

                users.Add(user);
                var updatedJson = JsonSerializer.Serialize(users);
                System.IO.File.WriteAllText(_userFile, updatedJson);

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Giriş Sayfası (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Giriş Yapma İşlemi (POST)
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (System.IO.File.Exists(_userFile))
            {
                var jsonData = System.IO.File.ReadAllText(_userFile);
                var users = JsonSerializer.Deserialize<List<User>>(jsonData);

                var user = users.FirstOrDefault(x => x.Email == email);

                if (user != null)
                {
                    var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        TempData["KullanıcıAdı"] = email;
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ViewBag.Error = "E-posta veya şifre hatalı.";
            return View();
        }

        public IActionResult Logout()
        {
            TempData.Clear();
            return RedirectToAction("Login");
        }
    }
}

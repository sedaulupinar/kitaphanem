
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 🔹 GEREKLİ EKLEME: Session için bellek tabanlı cache servisini ekle
builder.Services.AddDistributedMemoryCache();

// 🔹 ZATEN DOĞRU: Session servisini ekle
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true; // Güvenlik için sadece HTTP üzerinden erişilsin
    options.Cookie.IsEssential = true; // Bu cookie’nin zorunlu olduğunu belirt
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔹 Session'ı burada aktifleştiriyoruz
app.UseSession();

app.UseAuthorization();

// Yönlendirme için route ekleme
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "book",
    pattern: "Book/{action=Index}/{id?}",
    defaults: new { controller = "Book" });

app.Run();

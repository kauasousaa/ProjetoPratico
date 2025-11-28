using EM.Domain;
using EM.Repository;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

var connectionString = builder.Configuration.GetConnectionString("FirebirdConnection") 
    ?? throw new InvalidOperationException("Connection string 'FirebirdConnection' não encontrada.");

builder.Services.AddScoped<IRepositorioCidade<Cidade>>(sp => new RepositorioCidade(connectionString));
builder.Services.AddScoped<IRepositorioAluno<Aluno>>(sp => new RepositorioAluno(connectionString));
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

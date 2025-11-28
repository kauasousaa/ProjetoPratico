using EM.Domain;
using EM.Repository;
using QuestPDF.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

string connectionString = builder.Configuration.GetConnectionString("FirebirdConnection") 
    ?? throw new InvalidOperationException("Connection string 'FirebirdConnection' não encontrada.");

builder.Services.AddScoped<IRepositorioCidade<Cidade>>(_ => new RepositorioCidade(connectionString));
builder.Services.AddScoped<IRepositorioAluno<Aluno>>(_ => new RepositorioAluno(connectionString));
builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

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

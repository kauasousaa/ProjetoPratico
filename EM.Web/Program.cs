using EM.Domain;
using EM.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRepositorioCidade<Cidade>, RepositorioCidade>();
builder.Services.AddSingleton<IRepositorioAluno<Aluno>, RepositorioAluno>();
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
    name: "cadastroCidade",
    pattern: "AdministracaoCidade/CadastroCidade/{id?}",
    defaults: new { controller = "AdministracaoCidade", action = "CadastroCidade" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

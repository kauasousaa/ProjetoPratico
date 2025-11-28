using System.Diagnostics;
using System.IO;
using System.Linq;
using EM.Domain;
using EM.Repository;
using EM.Repository.Utilitarios;
using EM.Web.Models;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace EM.Web.Controllers;

public class HomeController : Controller
{
    private readonly IRepositorioAluno<Aluno> _repositorioAluno;

    public HomeController(IRepositorioAluno<Aluno> repositorioAluno)
    {
        _repositorioAluno = repositorioAluno;
    }

    public IActionResult Index(string? busca, string? tipoBusca)
    {
        string termo = busca?.Trim() ?? string.Empty;
        string filtro = string.IsNullOrWhiteSpace(tipoBusca) ? "matricula" : tipoBusca;
        IEnumerable<Aluno> alunos = _repositorioAluno
            .Listar()
            .FiltrarPorTermo(termo, filtro)
            .OrdenarPorMatricula();

        ViewBag.TermoBusca = termo;
        ViewBag.TipoBusca = filtro;

        return View(alunos);
    }

    public IActionResult Relatorio()
    {
        List<Aluno> alunos = _repositorioAluno.Listar().ToList();

        QuestPDF.Fluent.Document document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Relatório de alunos")
                    .FontSize(18)
                    .SemiBold()
                    .AlignCenter();

                page.Content().PaddingTop(12).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(0.8f);
                        columns.RelativeColumn(2f);
                        columns.RelativeColumn(1f);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(0.7f);
                        columns.RelativeColumn(0.9f);
                        columns.RelativeColumn(1.3f);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Matrícula").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Nome").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Sexo").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Cidade").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("UF").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Nascimento").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("CPF").SemiBold();
                    });

                    foreach (Aluno aluno in alunos)
                    {
                        Cidade cidade = aluno.Residencia ?? new Cidade();

                        table.Cell().Padding(3).Text(aluno.Matricula.ToString());
                        table.Cell().Padding(3).Text(aluno.NomeCompleto);
                        table.Cell().Padding(3).Text(aluno.Genero.ToString());
                        table.Cell().Padding(3).Text(cidade.Nome);
                        table.Cell().Padding(3).Text(cidade.Estado);
                        table.Cell().Padding(3).Text(aluno.DataNascimento.ToString("dd/MM/yyyy"));
                        table.Cell().Padding(3).Text(aluno.CPF ?? string.Empty);
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}");
            });
        });

        MemoryStream stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;
        return File(stream, "application/pdf", "alunos-relatorio.pdf");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

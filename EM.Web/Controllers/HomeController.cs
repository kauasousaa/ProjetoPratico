using System.Diagnostics;
using System.Linq;
using System.Text;
using EM.Domain;
using EM.Repository;
using EM.Repository.Utilitarios;
using EM.Web.Models;
using Microsoft.AspNetCore.Mvc;

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
        var termo = busca?.Trim() ?? string.Empty;
        var filtro = string.IsNullOrWhiteSpace(tipoBusca) ? "matricula" : tipoBusca;
        var alunos = _repositorioAluno
            .Listar()
            .FiltrarPorTermo(termo, filtro)
            .OrdenarPorMatricula();

        ViewBag.TermoBusca = termo;
        ViewBag.TipoBusca = filtro;

        return View(alunos);
    }

    public IActionResult Relatorio()
    {
        var alunos = _repositorioAluno.Listar();
        var csv = new StringBuilder();
        csv.AppendLine("Matricula,Nome,Sexo,Cidade,UF,Nascimento,CPF");

        foreach (var aluno in alunos)
        {
            var cidade = aluno.Residencia ?? new Cidade();
            csv.AppendLine(
                $"{aluno.Matricula},{EscaparCsv(aluno.NomeCompleto)},{aluno.Genero},{EscaparCsv(cidade.Nome)},{cidade.Estado},{aluno.DataNascimento:dd/MM/yyyy},{(aluno.CPF ?? string.Empty)}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", "alunos-relatorio.csv");
    }

    private static string EscaparCsv(string valor)
        => $"\"{(valor ?? string.Empty).Replace("\"", "\"\"")}\"";

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

using System;
using System.Linq;
using EM.Domain;
using EM.Domain.Enuns;
using EM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EM.Web.Controllers;

public class AdministracaoAlunoController : Controller
{
    private readonly IRepositorioAluno<Aluno> _repositorioAluno;
    private readonly IRepositorioCidade<Cidade> _repositorioCidade;

    public AdministracaoAlunoController(
        IRepositorioAluno<Aluno> repositorioAluno,
        IRepositorioCidade<Cidade> repositorioCidade)
    {
        _repositorioAluno = repositorioAluno;
        _repositorioCidade = repositorioCidade;
    }

    private IEnumerable<SelectListItem> ObterCidadesSelecionadas() =>
        _repositorioCidade
            .Listar()
            .Select(c => new SelectListItem($"{c.Nome} - {c.Estado}", c.Id.ToString()));

    public IActionResult Cadastro(int? id)
    {
        Aluno modelo;

        if (id.HasValue)
        {
            var existente = _repositorioAluno.Buscar(a => a.Id == id.Value).FirstOrDefault();
            if (existente == null)
            {
                return NotFound();
            }

            modelo = existente;
        }
        else
        {
            modelo = new Aluno
            {
                Residencia = new Cidade(),
                DataNascimento = DateTime.Now,
                Genero = SexoEnum.Masculino
            };
        }

        ViewBag.Cidades = ObterCidadesSelecionadas();
        ViewBag.IsEdicao = id.HasValue;
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastro(Aluno aluno)
    {
        var cidadeSelecionada = _repositorioCidade
            .Buscar(c => c.Id == aluno.Residencia?.Id)
            .FirstOrDefault();
        if (cidadeSelecionada == null)
        {
            ModelState.AddModelError("Residencia.Id", "Selecione uma cidade válida");
        }
        else
        {
            aluno.Residencia = new Cidade
            {
                Id = cidadeSelecionada.Id,
                Nome = cidadeSelecionada.Nome,
                Estado = cidadeSelecionada.Estado
            };
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Cidades = ObterCidadesSelecionadas();
            ViewBag.IsEdicao = aluno.Id != 0;
            return View(aluno);
        }

        if (aluno.Id == 0)
        {
            _repositorioAluno.Inserir(aluno);
        }
        else
        {
            _repositorioAluno.Atualizar(aluno);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Excluir(int id)
    {
        var aluno = _repositorioAluno.Buscar(a => a.Id == id).FirstOrDefault();
        if (aluno != null)
        {
            _repositorioAluno.Apagar(aluno);
        }

        return RedirectToAction("Index", "Home");
    }
}


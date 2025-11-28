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

    private IEnumerable<SelectListItem> ObterCidadesSelecionadas()
    {
        List<Cidade> cidades = _repositorioCidade.Listar().ToList();

        return cidades.Select(c => new SelectListItem
        {
            Text = $"{c.Nome} - {c.Estado}",
            Value = c.Id.ToString()
        });
    }

    public IActionResult Cadastro(int? id)
    {
        Aluno modelo;

        if (id.HasValue)
        {
            Aluno? existente = _repositorioAluno.Buscar(a => a.Id == id.Value).FirstOrDefault();
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
        if (aluno.Residencia == null)
        {
            aluno.Residencia = new Cidade();
        }

        ModelState.Remove("Residencia.Nome");
        ModelState.Remove("Residencia.Estado");
        ModelState.Remove("Residencia");

        int? cidadeId = null;
        bool campoCidadeFoiEnviado = false;

        if (Request.Form.ContainsKey("Residencia.Id"))
        {
            string? valorForm = Request.Form["Residencia.Id"].FirstOrDefault();
            campoCidadeFoiEnviado = true;

            if (valorForm != null)
            {
                if (valorForm == "0")
                {
                    cidadeId = 0;
                }
                else if (!string.IsNullOrWhiteSpace(valorForm) && int.TryParse(valorForm, out int parsedId))
                {
                    cidadeId = parsedId;
                }
            }
        }

        if (!campoCidadeFoiEnviado && aluno.Residencia != null)
        {
            cidadeId = aluno.Residencia.Id;
            campoCidadeFoiEnviado = true;
        }

        if (!campoCidadeFoiEnviado)
        {
            foreach (string key in Request.Form.Keys)
            {
                if (key.Contains("Residencia", StringComparison.OrdinalIgnoreCase))
                {
                    string? valor = Request.Form[key].FirstOrDefault();
                    campoCidadeFoiEnviado = true;

                    if (valor != null && int.TryParse(valor, out int parsedId))
                    {
                        cidadeId = parsedId;
                        break;
                    }
                }
            }
        }

        if (!campoCidadeFoiEnviado || cidadeId == null)
        {
            ModelState.AddModelError("Residencia.Id", "Selecione uma cidade válida");
            aluno.Residencia = new Cidade();
        }
        else
        {
            Cidade? cidadeSelecionada = _repositorioCidade.Listar().FirstOrDefault(c => c.Id == cidadeId.Value);

            if (cidadeSelecionada == null)
            {
                ModelState.AddModelError("Residencia.Id", $"Cidade com ID {cidadeId.Value} não encontrada no banco de dados. Verifique se a cidade existe.");
                aluno.Residencia = new Cidade { Id = cidadeId.Value };
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
        }

        ModelState.Remove("Residencia");

        if (string.IsNullOrWhiteSpace(aluno.CPF))
        {
            aluno.CPF = null;
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Cidades = ObterCidadesSelecionadas();
            ViewBag.IsEdicao = aluno.Id != 0;
            return View(aluno);
        }

        try
        {
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
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Erro ao salvar aluno: {ex.Message}");

            ViewBag.Cidades = ObterCidadesSelecionadas();
            ViewBag.IsEdicao = aluno.Id != 0;
            return View(aluno);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Excluir(int id)
    {
        Aluno? aluno = _repositorioAluno.Buscar(a => a.Id == id).FirstOrDefault();
        if (aluno != null)
        {
            _repositorioAluno.Apagar(aluno);
        }

        return RedirectToAction("Index", "Home");
    }
}

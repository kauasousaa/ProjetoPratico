using System.Linq;
using EM.Domain;
using EM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EM.Web.Controllers;

public class AdministracaoCidadeController : Controller
{
    private readonly IRepositorioCidade<Cidade> _repositorioCidade;

    public AdministracaoCidadeController(IRepositorioCidade<Cidade> repositorioCidade)
    {
        _repositorioCidade = repositorioCidade;
    }

    public IActionResult Index(int? id)
    {
        var cidades = _repositorioCidade.Listar();
        
        Cidade cidadeModel;
        if (id.HasValue)
        {
            cidadeModel = _repositorioCidade.Buscar(c => c.Id == id.Value).FirstOrDefault() ?? new Cidade();
            ViewBag.IsEdicao = cidadeModel.Id != 0;
        }
        else
        {
            cidadeModel = new Cidade();
            ViewBag.IsEdicao = false;
        }

        ViewBag.Cidades = cidades;
        return View(cidadeModel);
    }

    [HttpPost]
    public IActionResult Index(Cidade cidade)
    {
        cidade.Nome = (cidade.Nome ?? string.Empty).Trim();
        cidade.Estado = (cidade.Estado ?? string.Empty).ToUpperInvariant().Trim();

        var cidades = _repositorioCidade.Listar();
        ViewBag.Cidades = cidades;

        if (!ModelState.IsValid)
        {
            ViewBag.IsEdicao = cidade.Id != 0;
            return View(cidade);
        }

        if (cidade.Id != 0)
        {
            _repositorioCidade.Atualizar(cidade);
        }
        else
        {
            _repositorioCidade.Inserir(cidade);
        }

        return RedirectToAction(nameof(Index));
    }
}


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

    public IActionResult Index()
    {
        var cidades = _repositorioCidade.Listar();
        return View(cidades);
    }

    public IActionResult CadastroCidade(int? id)
    {
        if (id.HasValue)
        {
            var cidade = _repositorioCidade.Buscar(c => c.Id == id.Value).FirstOrDefault();
            if (cidade == null)
            {
                return NotFound();
            }

            ViewBag.IsEdicao = true;
            return View(cidade);
        }

        ViewBag.IsEdicao = false;
        return View(new Cidade());
    }

    [HttpPost]
    public IActionResult CadastroCidade(Cidade cidade)
    {
        cidade.Nome = (cidade.Nome ?? string.Empty).Trim();
        cidade.Estado = (cidade.Estado ?? string.Empty).ToUpperInvariant().Trim();

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


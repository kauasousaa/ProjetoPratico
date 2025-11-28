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
        var cidades = _repositorioCidade.Listar().ToList();
        System.Diagnostics.Debug.WriteLine($"Total de cidades carregadas: {cidades.Count}");
        
        foreach (var c in cidades)
        {
            System.Diagnostics.Debug.WriteLine($"  - Cidade: {c.Nome} - {c.Estado} (ID: {c.Id})");
        }
        
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
        // Garantir que Residencia está inicializada
        if (aluno.Residencia == null)
        {
            aluno.Residencia = new Cidade();
        }
        
        // Remover erros de validação da propriedade Residencia (Cidade)
        // porque estamos validando apenas o ID da cidade, não os campos da Cidade
        ModelState.Remove("Residencia.Nome");
        ModelState.Remove("Residencia.Estado");
        ModelState.Remove("Residencia");
        
        // Capturar ID da cidade do formulário - tentar múltiplas formas
        int? cidadeId = null;
        bool campoCidadeFoiEnviado = false;
        
        // 1. Tentar diretamente do Request.Form primeiro (mais confiável)
        if (Request.Form.ContainsKey("Residencia.Id"))
        {
            var valorForm = Request.Form["Residencia.Id"].FirstOrDefault();
            campoCidadeFoiEnviado = true;
            
            System.Diagnostics.Debug.WriteLine($"Valor capturado do form 'Residencia.Id': '{valorForm}' (null={valorForm == null}, empty={string.IsNullOrEmpty(valorForm)})");
            
            // Se o campo foi enviado, tentar converter (mesmo que seja "0" para Goiânia)
            if (valorForm != null)
            {
                // Aceitar "0" como valor válido (Goiânia tem ID 0)
                if (valorForm == "0")
                {
                    cidadeId = 0; // Goiânia
                    System.Diagnostics.Debug.WriteLine($"Cidade ID é 0 (Goiânia) - VALIDADO");
                }
                else if (!string.IsNullOrWhiteSpace(valorForm) && int.TryParse(valorForm, out int parsedId))
                {
                    cidadeId = parsedId;
                    System.Diagnostics.Debug.WriteLine($"Cidade ID parseado com sucesso: {cidadeId}");
                }
                else if (string.IsNullOrWhiteSpace(valorForm))
                {
                    System.Diagnostics.Debug.WriteLine("Campo Residencia.Id está vazio - nenhuma cidade selecionada");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Falha ao fazer parse do valor '{valorForm}'");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Campo Residencia.Id é null");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Campo 'Residencia.Id' NÃO encontrado no Request.Form");
        }
        
        // 2. Se não encontrou no form, tentar do model binding (pode ter vindo do Residencia.Id)
        if (!campoCidadeFoiEnviado && aluno.Residencia != null)
        {
            cidadeId = aluno.Residencia.Id;
            campoCidadeFoiEnviado = true; // Assumir que foi enviado se existe no objeto
        }
        
        // 3. Se ainda não encontrou, procurar em todas as chaves do form
        if (!campoCidadeFoiEnviado)
        {
            foreach (var key in Request.Form.Keys)
            {
                if (key.Contains("Residencia", StringComparison.OrdinalIgnoreCase))
                {
                    var valor = Request.Form[key].FirstOrDefault();
                    campoCidadeFoiEnviado = true;
                    
                    if (valor != null && int.TryParse(valor, out int parsedId))
                    {
                        cidadeId = parsedId;
                        break;
                    }
                }
            }
        }
        
        // Validar se cidade foi selecionada e atribuir ao aluno
        // cidadeId == null significa que o campo não foi preenchido ou estava vazio
        // cidadeId == 0 significa que Goiânia foi selecionada (ID válido)
        System.Diagnostics.Debug.WriteLine($"Validação final - campoCidadeFoiEnviado: {campoCidadeFoiEnviado}, cidadeId: {cidadeId}");
        
        if (!campoCidadeFoiEnviado || cidadeId == null)
        {
            System.Diagnostics.Debug.WriteLine("ERRO: Cidade não foi selecionada (campo vazio ou não enviado)");
            ModelState.AddModelError("Residencia.Id", "Selecione uma cidade válida");
            aluno.Residencia = new Cidade(); // Garantir que Residencia não seja null
        }
        else
        {
            // cidadeId tem valor (pode ser 0, como no caso de Goiânia)
            int idParaBuscar = cidadeId.Value;
            System.Diagnostics.Debug.WriteLine($"Buscando cidade com ID: {idParaBuscar} (pode ser 0 para Goiânia)");
            
            // Buscar a cidade no banco (incluindo ID 0)
            var todasCidades = _repositorioCidade.Listar().ToList();
            System.Diagnostics.Debug.WriteLine($"Total de cidades no banco: {todasCidades.Count}");
            var cidadeSelecionada = todasCidades.FirstOrDefault(c => c.Id == idParaBuscar);
            
            if (cidadeSelecionada == null)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO: Cidade com ID {idParaBuscar} não encontrada no banco");
                var idsDisponiveis = string.Join(", ", todasCidades.Select(c => c.Id));
                System.Diagnostics.Debug.WriteLine($"IDs disponíveis: {idsDisponiveis}");
                ModelState.AddModelError("Residencia.Id", $"Cidade com ID {idParaBuscar} não encontrada no banco de dados. Verifique se a cidade existe.");
                aluno.Residencia = new Cidade { Id = idParaBuscar }; // Manter o ID selecionado para exibição
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"SUCESSO: Cidade encontrada - {cidadeSelecionada.Nome} - {cidadeSelecionada.Estado} (ID: {cidadeSelecionada.Id})");
                // Atribuir a cidade completa ao aluno
                aluno.Residencia = new Cidade
                {
                    Id = cidadeSelecionada.Id,
                    Nome = cidadeSelecionada.Nome,
                    Estado = cidadeSelecionada.Estado
                };
            }
        }
        
        // Remover erros de validação da propriedade Residencia após validar manualmente
        // mas manter erros de Residencia.Id que adicionamos manualmente
        ModelState.Remove("Residencia");

        // Limpar CPF se estiver vazio
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
            System.Diagnostics.Debug.WriteLine($"ERRO no controller: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            
            ViewBag.Cidades = ObterCidadesSelecionadas();
            ViewBag.IsEdicao = aluno.Id != 0;
            return View(aluno);
        }
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


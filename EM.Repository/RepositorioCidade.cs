using EM.Domain;

namespace EM.Repository;

public class RepositorioCidade : RepositorioBase<Cidade>, IRepositorioCidade<Cidade>
{
    public RepositorioCidade()
    {
        if (!Registros.Any())
        {
            Registros.AddRange(new[]
            {
                new Cidade { Id = ObterProximoId(), Nome = "Maré Alta", Estado = "RJ" },
                new Cidade { Id = ObterProximoId(), Nome = "Vale Sol", Estado = "MG" },
                new Cidade { Id = ObterProximoId(), Nome = "Novo Horizonte", Estado = "SP" }
            });
        }
    }

    public void Inserir(Cidade entidade)
    {
        entidade.Id = ObterProximoId();
        Registros.Add(CloneCidade(entidade));
    }

    public void Atualizar(Cidade entidade)
    {
        var existente = Registros.FirstOrDefault(c => c.Id == entidade.Id);
        if (existente != null)
        {
            existente.Nome = entidade.Nome;
            existente.Estado = entidade.Estado;
        }
    }

    private static Cidade CloneCidade(Cidade origem) =>
        new()
        {
            Id = origem.Id,
            Nome = origem.Nome,
            Estado = origem.Estado
        };
}






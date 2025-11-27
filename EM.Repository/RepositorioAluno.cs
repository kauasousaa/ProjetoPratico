using EM.Domain;
using EM.Domain.Enuns;
using System.Linq;

namespace EM.Repository;

public class RepositorioAluno : RepositorioBase<Aluno>, IRepositorioAluno<Aluno>
{
    public RepositorioAluno()
    {
        if (!Registros.Any())
        {
            Registros.AddRange(new[]
            {
                new Aluno
                {
                    Id = ObterProximoId(),
                    Matricula = 5001,
                    NomeCompleto = "Marina Lessa",
                    CPF = "028.556.989-61",
                    DataNascimento = new(2005, 2, 14),
                    Genero = SexoEnum.Feminino,
                    Residencia = new Cidade { Id = 1, Nome = "Maré Alta", Estado = "RJ" }
                },
                new Aluno
                {
                    Id = ObterProximoId(),
                    Matricula = 5203,
                    NomeCompleto = "Mateus Oliveira",
                    CPF = "198.662.980-40",
                    DataNascimento = new(2003, 8, 2),
                    Genero = SexoEnum.Masculino,
                    Residencia = new Cidade { Id = 2, Nome = "Vale Sol", Estado = "MG" }
                },
                new Aluno
                {
                    Id = ObterProximoId(),
                    Matricula = 5347,
                    NomeCompleto = "Luna Prado",
                    CPF = "444.513.220-11",
                    DataNascimento = new(2004, 11, 19),
                    Genero = SexoEnum.Feminino,
                    Residencia = new Cidade { Id = 3, Nome = "Novo Horizonte", Estado = "SP" }
                }
            });
        }
    }

    public void Inserir(Aluno entidade)
    {
        entidade.Id = ObterProximoId();
        Registros.Add(CloneAluno(entidade));
    }

    public void Atualizar(Aluno entidade)
    {
        var cursor = Registros.FirstOrDefault(a => a.Id == entidade.Id);
        if (cursor == null)
        {
            return;
        }

        cursor.Matricula = entidade.Matricula;
        cursor.NomeCompleto = entidade.NomeCompleto;
        cursor.CPF = entidade.CPF;
        cursor.DataNascimento = entidade.DataNascimento;
        cursor.Genero = entidade.Genero;
        cursor.Residencia = CloneCidade(entidade.Residencia);
    }

    public void Apagar(Aluno entidade)
    {
        Registros.RemoveAll(a => a.Id == entidade.Id);
    }

    private static Aluno CloneAluno(Aluno origem) =>
        new()
        {
            Id = origem.Id,
            Matricula = origem.Matricula,
            NomeCompleto = origem.NomeCompleto,
            CPF = origem.CPF,
            DataNascimento = origem.DataNascimento,
            Genero = origem.Genero,
            Residencia = CloneCidade(origem.Residencia)
        };

    private static Cidade CloneCidade(Cidade origem) =>
        new()
        {
            Id = origem.Id,
            Nome = origem.Nome,
            Estado = origem.Estado
        };
}


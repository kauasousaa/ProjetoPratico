using System;
using System.Collections.Generic;
using System.Linq;
using EM.Domain;

namespace EM.Repository.Utilitarios;

public static class AlunoFiltroExtensoes
{
    public static IEnumerable<Aluno> FiltrarPorTermo(this IEnumerable<Aluno> alunos, string termo, string tipoBusca)
    {
        if (string.IsNullOrWhiteSpace(termo))
        {
            return alunos;
        }

        return tipoBusca switch
        {
            "nome" => alunos.Where(aluno => aluno.NomeCompleto.Contains(termo, StringComparison.OrdinalIgnoreCase)),
            _ => alunos.Where(aluno => aluno.Matricula.ToString().Contains(termo))
        };
    }

    public static IEnumerable<Aluno> OrdenarPorMatricula(this IEnumerable<Aluno> alunos) =>
        alunos.OrderBy(aluno => aluno.Matricula);
}






using EM.Domain.Interface;
using System;

namespace EM.Repository;

public interface IRepositorioAluno<T> where T : IEntidade
{
    void Inserir(T entidade);
    void Atualizar(T entidade);
    void Apagar(T entidade);
    IEnumerable<T> Listar();
    IEnumerable<T> Buscar(Func<T, bool> criterio);
}


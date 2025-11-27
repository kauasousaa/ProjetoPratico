using EM.Domain.Interface;
using System;

namespace EM.Repository;

public interface IRepositorioCidade<T> where T : IEntidade
{
    void Inserir(T entidade);
    void Atualizar(T entidade);
    IEnumerable<T> Listar();
    IEnumerable<T> Buscar(Func<T, bool> criterio);
}


using EM.Domain.Interface;

namespace EM.Repository;

public abstract class RepositorioBase<T> where T : IEntidade
{
    protected readonly List<T> Registros = new();

    public virtual IEnumerable<T> Listar() => Registros.ToList();

    public virtual IEnumerable<T> Buscar(Func<T, bool> criterio) => Registros.Where(criterio);

    protected int ObterProximoId() =>
        Registros.Any()
            ? Registros.Max(entidade => entidade.Id) + 1
            : 1;
}






using EM.Domain.Interface;
using FirebirdSql.Data.FirebirdClient;

namespace EM.Repository;

public abstract class RepositorioBase<T> where T : IEntidade
{
    protected readonly string ConnectionString;

    protected RepositorioBase(string connectionString)
    {
        ConnectionString = connectionString;
    }

    protected FbConnection CriarConexao()
    {
        return new FbConnection(ConnectionString);
    }
}






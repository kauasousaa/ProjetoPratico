using EM.Domain;
using FirebirdSql.Data.FirebirdClient;
using System;

namespace EM.Repository;

public class RepositorioCidade : RepositorioBase<Cidade>, IRepositorioCidade<Cidade>
{
    public RepositorioCidade(string connectionString) : base(connectionString)
    {
    }

    public IEnumerable<Cidade> Listar()
    {
        List<Cidade> cidades = new List<Cidade>();

        try
        {
            using FbConnection conexao = CriarConexao();
            conexao.Open();

            string sql = "SELECT CIDCODIGO, CIDNOME, CIDUF FROM TBCIDADE ORDER BY CIDCODIGO";

            using FbCommand cmd = new FbCommand(sql, conexao);
            using FbDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cidades.Add(new Cidade
                {
                    Id = reader.GetInt32(reader.GetOrdinal("CIDCODIGO")),
                    Nome = reader.IsDBNull(reader.GetOrdinal("CIDNOME")) ? "" : reader.GetString(reader.GetOrdinal("CIDNOME")),
                    Estado = reader.IsDBNull(reader.GetOrdinal("CIDUF")) ? "" : reader.GetString(reader.GetOrdinal("CIDUF"))
                });
            }
        }
        catch (Exception)
        {
        }

        return cidades;
    }

    public IEnumerable<Cidade> Buscar(Func<Cidade, bool> criterio)
    {
        return Listar().Where(criterio);
    }

    public void Inserir(Cidade entidade)
    {
        using FbConnection conexao = CriarConexao();
        conexao.Open();

        if (entidade.Id == 0)
        {
            string sqlMaxId = "SELECT COALESCE(MAX(CIDCODIGO), 0) + 1 AS PROXIMO_ID FROM TBCIDADE";
            using FbCommand cmdMaxId = new FbCommand(sqlMaxId, conexao);
            object? proximoId = cmdMaxId.ExecuteScalar();
            entidade.Id = Convert.ToInt32(proximoId);
        }

        string sql = @"INSERT INTO TBCIDADE (CIDCODIGO, CIDNOME, CIDUF)
                    VALUES (@Codigo, @Nome, @UF)";

        using FbCommand cmd = new FbCommand(sql, conexao);
        cmd.Parameters.Add("@Codigo", FbDbType.Integer).Value = entidade.Id;
        cmd.Parameters.Add("@Nome", FbDbType.VarChar, 100).Value = entidade.Nome;
        cmd.Parameters.Add("@UF", FbDbType.Char, 2).Value = entidade.Estado;

        cmd.ExecuteNonQuery();
    }

    public void Atualizar(Cidade entidade)
    {
        using FbConnection conexao = CriarConexao();
        conexao.Open();

        string sql = @"UPDATE TBCIDADE 
                    SET CIDNOME = @Nome,
                        CIDUF = @UF
                    WHERE CIDCODIGO = @Codigo";

        using FbCommand cmd = new FbCommand(sql, conexao);
        cmd.Parameters.Add("@Codigo", FbDbType.Integer).Value = entidade.Id;
        cmd.Parameters.Add("@Nome", FbDbType.VarChar, 100).Value = entidade.Nome;
        cmd.Parameters.Add("@UF", FbDbType.Char, 2).Value = entidade.Estado;

        cmd.ExecuteNonQuery();
    }
}

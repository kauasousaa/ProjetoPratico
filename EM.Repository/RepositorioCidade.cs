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
        var cidades = new List<Cidade>();

        try
        {
            using var conexao = CriarConexao();
            conexao.Open();

            var sql = "SELECT CIDCODIGO, CIDNOME, CIDUF FROM TBCIDADE ORDER BY CIDCODIGO";

            using var cmd = new FbCommand(sql, conexao);
            using var reader = cmd.ExecuteReader();

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
        using var conexao = CriarConexao();
        conexao.Open();

        if (entidade.Id == 0)
        {
            var sqlMaxId = "SELECT COALESCE(MAX(CIDCODIGO), 0) + 1 AS PROXIMO_ID FROM TBCIDADE";
            using var cmdMaxId = new FbCommand(sqlMaxId, conexao);
            var proximoId = cmdMaxId.ExecuteScalar();
            entidade.Id = Convert.ToInt32(proximoId);
        }

        var sql = @"INSERT INTO TBCIDADE (CIDCODIGO, CIDNOME, CIDUF)
                    VALUES (@Codigo, @Nome, @UF)";

        using var cmd = new FbCommand(sql, conexao);
        cmd.Parameters.Add("@Codigo", FbDbType.Integer).Value = entidade.Id;
        cmd.Parameters.Add("@Nome", FbDbType.VarChar, 100).Value = entidade.Nome;
        cmd.Parameters.Add("@UF", FbDbType.Char, 2).Value = entidade.Estado;

        cmd.ExecuteNonQuery();
    }

    public void Atualizar(Cidade entidade)
    {
        using var conexao = CriarConexao();
        conexao.Open();

        var sql = @"UPDATE TBCIDADE 
                    SET CIDNOME = @Nome,
                        CIDUF = @UF
                    WHERE CIDCODIGO = @Codigo";

        using var cmd = new FbCommand(sql, conexao);
        cmd.Parameters.Add("@Codigo", FbDbType.Integer).Value = entidade.Id;
        cmd.Parameters.Add("@Nome", FbDbType.VarChar, 100).Value = entidade.Nome;
        cmd.Parameters.Add("@UF", FbDbType.Char, 2).Value = entidade.Estado;

        cmd.ExecuteNonQuery();
    }
}

using EM.Domain;
using EM.Domain.Enuns;
using FirebirdSql.Data.FirebirdClient;
using System;

namespace EM.Repository;

public class RepositorioAluno : RepositorioBase<Aluno>, IRepositorioAluno<Aluno>
{
    public RepositorioAluno(string connectionString) : base(connectionString)
    {
    }

    public IEnumerable<Aluno> Listar()
    {
        var alunos = new List<Aluno>();

        try
        {
            using var conexao = CriarConexao();
            conexao.Open();

            var sql = @"SELECT 
                            a.ALUMATRICULA,
                            a.ALUNOME,
                            a.ALUCPF,
                            a.ALUNASCIMENTO,
                            a.ALUSEXO,
                            a.ALUCODCIDADE,
                            c.CIDNOME,
                            c.CIDUF
                        FROM TBALUNO a
                        LEFT JOIN TBCIDADE c ON a.ALUCODCIDADE = c.CIDCODIGO
                        ORDER BY a.ALUMATRICULA";

            using var cmd = new FbCommand(sql, conexao);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                alunos.Add(new Aluno
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ALUMATRICULA")),
                    Matricula = reader.GetInt32(reader.GetOrdinal("ALUMATRICULA")),
                    NomeCompleto = reader.GetString(reader.GetOrdinal("ALUNOME")),
                    CPF = reader.IsDBNull(reader.GetOrdinal("ALUCPF")) ? null : reader.GetString(reader.GetOrdinal("ALUCPF")),
                    DataNascimento = reader.GetDateTime(reader.GetOrdinal("ALUNASCIMENTO")),
                    Genero = ConverterSexo(reader.GetInt32(reader.GetOrdinal("ALUSEXO"))),
                    Residencia = new Cidade
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ALUCODCIDADE")),
                        Nome = reader.IsDBNull(reader.GetOrdinal("CIDNOME")) ? "" : reader.GetString(reader.GetOrdinal("CIDNOME")),
                        Estado = reader.IsDBNull(reader.GetOrdinal("CIDUF")) ? "" : reader.GetString(reader.GetOrdinal("CIDUF"))
                    }
                });
            }
        }
        catch (Exception ex)
        {
            return new List<Aluno>();
        }

        return alunos;
    }

    public IEnumerable<Aluno> Buscar(Func<Aluno, bool> criterio)
    {
        return Listar().Where(criterio);
    }

    public void Inserir(Aluno entidade)
    {
        try
        {
            using var conexao = CriarConexao();
            conexao.Open();

            var sql = @"INSERT INTO TBALUNO (ALUMATRICULA, ALUNOME, ALUCPF, ALUNASCIMENTO, ALUSEXO, ALUCODCIDADE)
                        VALUES (@Matricula, @Nome, @CPF, @DataNascimento, @Sexo, @CodCidade)";

            using var cmd = new FbCommand(sql, conexao);
            cmd.Parameters.Add("@Matricula", FbDbType.Integer).Value = entidade.Matricula;
            cmd.Parameters.Add("@Nome", FbDbType.VarChar, 100).Value = entidade.NomeCompleto;
            cmd.Parameters.Add("@CPF", FbDbType.VarChar, 14).Value = (object?)entidade.CPF ?? DBNull.Value;
            cmd.Parameters.Add("@DataNascimento", FbDbType.Date).Value = entidade.DataNascimento;
            cmd.Parameters.Add("@Sexo", FbDbType.Integer).Value = ConverterSexoParaInt(entidade.Genero);
            cmd.Parameters.Add("@CodCidade", FbDbType.Integer).Value = entidade.Residencia?.Id ?? 0;

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public void Atualizar(Aluno entidade)
    {
        using var conexao = CriarConexao();
        conexao.Open();

        var sql = @"UPDATE TBALUNO 
                    SET ALUMATRICULA = @Matricula,
                        ALUNOME = @Nome,
                        ALUCPF = @CPF,
                        ALUNASCIMENTO = @DataNascimento,
                        ALUSEXO = @Sexo,
                        ALUCODCIDADE = @CodCidade
                    WHERE ALUMATRICULA = @Matricula";

        using var cmd = new FbCommand(sql, conexao);
        cmd.Parameters.Add("@Matricula", FbDbType.Integer).Value = entidade.Matricula;
        cmd.Parameters.Add("@Nome", FbDbType.VarChar, 100).Value = entidade.NomeCompleto;
        cmd.Parameters.Add("@CPF", FbDbType.VarChar, 14).Value = (object?)entidade.CPF ?? DBNull.Value;
        cmd.Parameters.Add("@DataNascimento", FbDbType.Date).Value = entidade.DataNascimento;
        cmd.Parameters.Add("@Sexo", FbDbType.Integer).Value = ConverterSexoParaInt(entidade.Genero);
        cmd.Parameters.Add("@CodCidade", FbDbType.Integer).Value = entidade.Residencia.Id;

        cmd.ExecuteNonQuery();
    }

    public void Apagar(Aluno entidade)
    {
        using var conexao = CriarConexao();
        conexao.Open();

        var sql = "DELETE FROM TBALUNO WHERE ALUMATRICULA = @Matricula";

        using var cmd = new FbCommand(sql, conexao);
        cmd.Parameters.Add("@Matricula", FbDbType.Integer).Value = entidade.Matricula;

        cmd.ExecuteNonQuery();
    }

    private static SexoEnum ConverterSexo(int valor)
    {
        return valor switch
        {
            0 => SexoEnum.Masculino,
            1 => SexoEnum.Feminino,
            _ => SexoEnum.Masculino
        };
    }

    private static int ConverterSexoParaInt(SexoEnum sexo)
    {
        return sexo switch
        {
            SexoEnum.Masculino => 0,
            SexoEnum.Feminino => 1,
            _ => 0
        };
    }
}

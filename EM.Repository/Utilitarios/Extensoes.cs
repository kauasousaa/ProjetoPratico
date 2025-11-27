using EM.Domain.Enuns;

namespace EM.Repository.Utilitarios;

public static class Extensoes
{
    public static string ParaDescricao(this SexoEnum sexo) =>
        sexo switch
        {
            SexoEnum.Masculino => "Masculino",
            SexoEnum.Feminino => "Feminino",
            _ => "Outro"
        };

    public static string DataFormatada(this DateTime data) =>
        data.ToString("dd/MM/yyyy");
}






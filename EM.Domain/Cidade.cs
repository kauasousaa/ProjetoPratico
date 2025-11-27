using System.ComponentModel.DataAnnotations;
using EM.Domain.Interface;

namespace EM.Domain;

public class Cidade : IEntidade
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 60 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a UF")]
    [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "Informe uma UF válida (duas letras maiúsculas)")]
    public string Estado { get; set; } = string.Empty;
}


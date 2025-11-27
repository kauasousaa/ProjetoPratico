using System.ComponentModel.DataAnnotations;
using EM.Domain.Enuns;
using EM.Domain.Interface;

namespace EM.Domain;

public class Aluno : IEntidade
{
    public int Id { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma matrícula válida")]
    public int Matricula { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public string NomeCompleto { get; set; } = string.Empty;

    [RegularExpression(@"^$|^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido")]
    public string? CPF { get; set; }

    [Required]
    public DateTime DataNascimento { get; set; }

    [Required]
    public SexoEnum Genero { get; set; }

    [Required]
    public Cidade Residencia { get; set; } = new();
}


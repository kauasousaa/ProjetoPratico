using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EM.Domain.Validadores;

public class CpfValidoAttribute : ValidationAttribute
{
    public CpfValidoAttribute()
    {
        ErrorMessage ??= "CPF inválido";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        string? texto = value.ToString();
        if (string.IsNullOrWhiteSpace(texto))
        {
            return ValidationResult.Success;
        }

        string apenasDigitos = new string(texto.Where(char.IsDigit).ToArray());
        if (apenasDigitos.Length != 11 || DigitosRepetidos(apenasDigitos))
        {
            return new ValidationResult(ErrorMessage);
        }

        return VerificarDigitos(apenasDigitos)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage);
    }

    private static bool DigitosRepetidos(string cpf) => cpf.All(c => c == cpf[0]);

    private static bool VerificarDigitos(string cpf)
    {
        for (int posicao = 9; posicao <= 10; posicao++)
        {
            int soma = 0;
            for (int indice = 0; indice < posicao; indice++)
            {
                soma += (cpf[indice] - '0') * (posicao + 1 - indice);
            }

            int resto = soma % 11;
            int digitoEsperado = resto < 2 ? 0 : 11 - resto;
            if (digitoEsperado != cpf[posicao] - '0')
            {
                return false;
            }
        }

        return true;
    }
}




namespace ApiClinica.Validators;

public static class CpfValidator
{
    // Valida numericamente um CPF usando o algoritmo dos dígitos verificadores.
    public static bool IsValid(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;

        // Mantém apenas os dígitos (aceita CPF com ou sem máscara).
        string numeros = new string(cpf.Where(char.IsDigit).ToArray());

        if (numeros.Length != 11) return false;

        // Rejeita sequências com todos os dígitos iguais (ex.: 11111111111).
        if (numeros.Distinct().Count() == 1) return false;

        int[] digitos = numeros.Select(c => c - '0').ToArray();

        // Primeiro dígito verificador.
        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += digitos[i] * (10 - i);
        int primeiroDV = soma % 11;
        primeiroDV = primeiroDV < 2 ? 0 : 11 - primeiroDV;
        if (digitos[9] != primeiroDV) return false;

        // Segundo dígito verificador.
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += digitos[i] * (11 - i);
        int segundoDV = soma % 11;
        segundoDV = segundoDV < 2 ? 0 : 11 - segundoDV;
        if (digitos[10] != segundoDV) return false;

        return true;
    }
}

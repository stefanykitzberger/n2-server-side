namespace ApiClinica.Exceptions;

// Lançada pelos Services quando uma regra de negócio é violada (vira 400 no Controller).
public class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem) : base(mensagem)
    {
    }
}

namespace APICatalogo.Models;

public class BaseApiResponse<TResult, TError>
{
    public TResult? Result { get; set; }
    public TError? Error { get; set; }
}

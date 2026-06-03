namespace TaskFlowLite.Application.Models.Auth;

public record AuthResult(bool Succeeded, AuthResponse? Response, IReadOnlyList<string> Errors)
{
    public static AuthResult Success(AuthResponse response) => new(true, response, []);

    public static AuthResult Failure(params string[] errors) => new(false, null, errors);

    public static AuthResult Failure(IEnumerable<string> errors) => new(false, null, errors.ToArray());
}

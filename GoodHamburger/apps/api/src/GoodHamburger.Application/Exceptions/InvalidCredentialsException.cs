namespace GoodHamburger.Application.Exceptions;

/// <summary>
/// Thrown on failed login. Deliberately carries no detail about whether the
/// e-mail exists — the API answers 401 with a generic message either way.
/// </summary>
public class InvalidCredentialsException : Exception {
    public InvalidCredentialsException() : base("Invalid email or password.") { }
}

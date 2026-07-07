namespace GoodHamburger.Application.Exceptions;
public class ResourceAlreadyExists : Exception {
    public ResourceAlreadyExists(string resource, object? value)
        : base($"{resource} '{value}' already exists.") { }
}

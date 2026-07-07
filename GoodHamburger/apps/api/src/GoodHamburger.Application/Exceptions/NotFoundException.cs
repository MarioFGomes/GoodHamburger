namespace GoodHamburger.Application.Exceptions;
public class NotFoundException : Exception {
    public NotFoundException(string resource, object id)
        : base($"{resource} with id '{id}' was not found.") { }
}

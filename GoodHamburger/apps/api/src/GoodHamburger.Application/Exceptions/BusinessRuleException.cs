
namespace GoodHamburger.Application.Exceptions; 
public class BusinessRuleException : Exception {
    public BusinessRuleException(string message) : base(message) { }
}

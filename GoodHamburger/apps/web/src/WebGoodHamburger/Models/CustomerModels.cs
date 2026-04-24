using System.ComponentModel.DataAnnotations;

namespace WebGoodHamburger.Models;

public class CustomerResponse {
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class CreateCustomerRequest {
    [Required] [MaxLength(100)] public string FirstName { get; set; } = "";
    [Required] [MaxLength(100)] public string LastName { get; set; } = "";
    [MaxLength(200)] public string? Address { get; set; }
    [Required] [RegularExpression(@"^\+?[0-9]{9,15}$", ErrorMessage = "Phone must have 9-15 digits.")] public string Phone { get; set; } = "";
    [Required] [EmailAddress] public string Email { get; set; } = "";
}

public class UpdateCustomerRequest {
    [Required] [MaxLength(100)] public string FirstName { get; set; } = "";
    [Required] [MaxLength(100)] public string LastName { get; set; } = "";
    [MaxLength(200)] public string? Address { get; set; }
    [Required] [RegularExpression(@"^\+?[0-9]{9,15}$", ErrorMessage = "Phone must have 9-15 digits.")] public string Phone { get; set; } = "";
    [Required] [EmailAddress] public string Email { get; set; } = "";
}

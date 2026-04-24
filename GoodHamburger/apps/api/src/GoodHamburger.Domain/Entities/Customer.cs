using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Domain.Entities; 
public class Customer: EntityBase {
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public Customer(){}
    public Customer(string firtsname, string lastName, string email, string phone, string address) {
        FirstName = firtsname;
        LastName = lastName;
        Address = address;
        Phone = phone;
        Email = email;
    }

}


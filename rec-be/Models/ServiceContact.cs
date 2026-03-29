using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rec_be.Models
{
    public class ServiceContact
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string SecondName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string JobRole { get; set; } = "";
        public string IDCard { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public bool Avialable { get; set; }
    }
}
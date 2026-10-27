
using System;
using System.Collections.Generic;

namespace TARge25Shop.Core.Domain
{
    public class Reader
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}

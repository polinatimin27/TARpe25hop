
using System;

namespace TARge25Shop.Core.Domain
{
    public class Loan
    {
        public Guid Id { get; set; }

        public Guid BookId { get; set; }
        public Guid ReaderId { get; set; }

        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public Book Book { get; set; } = null!;
        public Reader Reader { get; set; } = null!;
    }
}

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;

namespace VickiWebApp.Models
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        { }

        public DbSet<Book> Books { get; set; }
    }
    public class Book
    {
        public int BookId { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        [DisplayName("Cover Type")]
        public string CoverType { get; set; }
        [DisplayName("Dewey Decimal Number")]
        public string DeweyDecimal { get; set; }
    }
}

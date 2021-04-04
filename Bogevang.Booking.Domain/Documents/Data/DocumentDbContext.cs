using Bogevang.Booking.Domain.Documents.Entities;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Bogevang.Booking.Domain.Documents.Data
{
  public class DocumentDbContext : DbContext
  {
    private readonly ICofoundryDbContextInitializer _cofoundryDbContextInitializer;

    
    public DocumentDbContext(ICofoundryDbContextInitializer cofoundryDbContextInitializer)
    {
      _cofoundryDbContextInitializer = cofoundryDbContextInitializer;
    }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      _cofoundryDbContextInitializer.Configure(this, optionsBuilder);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder
          .HasAppSchema()
          .ApplyConfiguration(new DocumentMap());
    }


    public DbSet<Document> Documents { get; set; }
  }
}

using Bogevang.SequenceGenerator.Domain.Entities;
using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bogevang.SequenceGenerator.Domain.Data
{
  public class SequenceDbContext : DbContext
  {
    private readonly ICofoundryDbContextInitializer _cofoundryDbContextInitializer;

    
    public SequenceDbContext(ICofoundryDbContextInitializer cofoundryDbContextInitializer)
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
          .ApplyConfiguration(new SequenceCounterMap());
    }


    public DbSet<SequenceCounter> Counters { get; set; }
  }
}

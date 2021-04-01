using Bogevang.SequenceGenerator.Domain.Entities;
using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bogevang.SequenceGenerator.Domain.Data
{
  public class SequenceCounterMap : IEntityTypeConfiguration<SequenceCounter>
  {
    public void Configure(EntityTypeBuilder<SequenceCounter> builder)
    {
      builder.ToTable("SequenceCounter", DbConstants.DefaultAppSchema);

      builder.HasKey(s => s.Name);

      // Properties

      builder.Property(s => s.Name)
          .IsRequired()
          .IsCharType(100);

      builder.Property(s => s.Counter)
          .IsRequired();
    }
  }
}

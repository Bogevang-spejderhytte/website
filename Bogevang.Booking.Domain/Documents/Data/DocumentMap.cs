using Bogevang.Booking.Domain.Documents.Entities;
using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bogevang.Booking.Domain.Documents.Data
{
  public class DocumentMap : IEntityTypeConfiguration<Document>
  {
    public void Configure(EntityTypeBuilder<Document> builder)
    {
      builder.ToTable("BookingDocument", DbConstants.DefaultAppSchema);

      builder.HasKey(s => s.Id);

      // Properties

      builder.Property(s => s.Title)
          .IsRequired()
          .IsCharType(100);

      builder.Property(s => s.MimeType)
          .IsRequired();

      builder.Property(s => s.Body)
          .IsRequired();

      builder.Property(s => s.CreatedDate)
          .IsRequired();
    }
  }
}

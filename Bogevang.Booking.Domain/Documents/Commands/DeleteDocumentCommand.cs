using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Documents.Commands
{
  public class DeleteDocumentCommand : ICommand
  {
    public int Id { get; set; }
  }
}

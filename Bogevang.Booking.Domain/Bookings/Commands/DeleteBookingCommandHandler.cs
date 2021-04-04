using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Documents.Commands;
using Bogevang.Common.Utility;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class DeleteBookingCommandHandler
    : ICommandHandler<DeleteBookingCommand>,
      IIgnorePermissionCheckHandler
  {
    private readonly ICommandExecutor CommandExecutor;
    private readonly IBookingProvider BookingProvider;


    public DeleteBookingCommandHandler(
      ICommandExecutor commandExecutor,
      IBookingProvider bookingProvider)
    {
      CommandExecutor = commandExecutor;
      BookingProvider = bookingProvider;
    }


    public async Task ExecuteAsync(DeleteBookingCommand command, IExecutionContext executionContext)
    {
      var booking = await BookingProvider.GetBookingById(command.Id);

      foreach (var document in booking.Documents)
      {
        var deleteDocumentCommand = new DeleteDocumentCommand { Id = document.DocumentId };
        await CommandExecutor.ExecuteAsync(deleteDocumentCommand);
      }

      var deleteCustomEntityCmd = new DeleteCustomEntityCommand { CustomEntityId = command.Id };
      await CommandExecutor.ExecuteAsync(deleteCustomEntityCmd);
    }
  }
}

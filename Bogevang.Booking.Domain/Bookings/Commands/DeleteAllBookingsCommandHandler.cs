using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Queries;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Linq;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class DeleteAllBookingsCommandHandler : 
    ICommandHandler<DeleteAllBookingsCommand>,
    IIgnorePermissionCheckHandler // Permission enforced in code
  {
    private readonly IBookingProvider BookingProvider;
    public ICommandExecutor CommandExecutor;
    private readonly IPermissionValidationService PermissionValidationService;


    public DeleteAllBookingsCommandHandler(
      IBookingProvider bookingProvider,
      ICommandExecutor executor,
      IPermissionValidationService permissionValidationService)
    {
      BookingProvider = bookingProvider;
      CommandExecutor = executor;
      PermissionValidationService = permissionValidationService;
    }


    public async Task ExecuteAsync(DeleteAllBookingsCommand command, IExecutionContext executionContext)
    {
      PermissionValidationService.EnforceCustomEntityPermission<CustomEntityDeletePermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);

      foreach (var booking in (await BookingProvider.FindBookingsInInterval(new SearchBookingSummariesQuery())).ToList())
      {
        var deleteCmd = new DeleteBookingCommand { Id = booking.Id };
        await CommandExecutor.ExecuteAsync(deleteCmd);
      }
    }
  }
}

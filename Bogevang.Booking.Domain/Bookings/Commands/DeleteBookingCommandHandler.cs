using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Documents.Commands;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class DeleteBookingCommandHandler :
    ICommandHandler<DeleteBookingCommand>,
    IIgnorePermissionCheckHandler // Permission enforced in code
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly ICommandExecutor CommandExecutor;
    private readonly IBookingProvider BookingProvider;
    private readonly IPermissionValidationService PermissionValidationService;


    public DeleteBookingCommandHandler(
      IAdvancedContentRepository domainRepository,
      ICommandExecutor commandExecutor,
      IBookingProvider bookingProvider,
      IPermissionValidationService permissionValidationService)
    {
      DomainRepository = domainRepository;
      CommandExecutor = commandExecutor;
      BookingProvider = bookingProvider;
      PermissionValidationService = permissionValidationService;
    }


    public async Task ExecuteAsync(DeleteBookingCommand command, IExecutionContext executionContext)
    {
      PermissionValidationService.EnforceCustomEntityPermission<CustomEntityDeletePermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);

      using (var scope = DomainRepository.Transactions().CreateScope())
      {
        var booking = await BookingProvider.GetBookingById(command.Id);

        foreach (var document in booking.Documents)
        {
          var deleteDocumentCommand = new DeleteDocumentCommand { Id = document.DocumentId };
          await CommandExecutor.ExecuteAsync(deleteDocumentCommand);
        }

        await DomainRepository.CustomEntities().DeleteAsync(command.Id);

        await scope.CompleteAsync();
      }
    }
  }
}

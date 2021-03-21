using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class BookingRequestCommandHandler
    : ICommandHandler<BookingRequestCommand>, IIgnorePermissionCheckHandler
  {
    private readonly IAdvancedContentRepository DomainRepository;


    public BookingRequestCommandHandler(
        IAdvancedContentRepository domainRepository)
    {
      DomainRepository = domainRepository;
    }


    public async Task ExecuteAsync(BookingRequestCommand command, IExecutionContext executionContext)
    {
      decimal rentalPrice = 1000;

      var booking = new BookingDataModel
      {
        ArrivalDate = command.ArrivalDate,
        DepartureDate = command.DepartureDate,
        TenantCategoryId = command.TenantCategoryId.Value,
        TenantName = command.TenantName,
        Purpose = command.Purpose,
        ContactName = command.ContactName,
        ContactPhone = command.ContactPhone,
        ContactAddress = command.ContactAddress,
        ContactCity = command.ContactCity,
        ContactEMail = command.ContactEMail,
        Comments = command.Comments,
        RentalPrice = rentalPrice,
        BookingState = BookingDataModel.BookingStateType.Requested
      };

      var addCommand = new AddCustomEntityCommand
      {
        CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
        Model = booking,
        Title = "Reservation",
        Publish = true,

      };

      await DomainRepository.CustomEntities().AddAsync(addCommand);
    }
  }
}

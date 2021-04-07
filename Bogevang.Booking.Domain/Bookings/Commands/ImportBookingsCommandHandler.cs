using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Bogevang.Booking.Domain.Bookings.CustomEntities;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;
using Bogevang.Booking.Domain.TenantCategories;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class ImportBookingsCommandHandler : 
    ICommandHandler<ImportBookingsCommand>,
    IIgnorePermissionCheckHandler // Permission enforced in code
  {
    private readonly IAdvancedContentRepository DomainRepository;
    private readonly ITenantCategoryProvider TenantCategoryProvider;
    private readonly IPermissionValidationService PermissionValidationService;


    public ImportBookingsCommandHandler(
      IAdvancedContentRepository domainRepository,
      ITenantCategoryProvider tenantCategoryProvider,
      IPermissionValidationService permissionValidationService)
    {
      DomainRepository = domainRepository;
      TenantCategoryProvider = tenantCategoryProvider;
      PermissionValidationService = permissionValidationService;
    }


    public async Task ExecuteAsync(ImportBookingsCommand command, IExecutionContext executionContext)
    {
      PermissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(BookingCustomEntityDefinition.DefinitionCode, executionContext.UserContext);

      DataSet bookings = new DataSet();
      bookings.ReadXml(command.ReadyToReadInput);

      var groupedBookings = bookings.Tables[0].Rows
        .Cast<DataRow>()
        .GroupBy(row => row["AftaleID"].ToString());

      int count = 0;
      foreach (var bookingGroup in groupedBookings)
      {
        try
        {
          int bookingNumber = Convert.ToInt32(bookingGroup.Key);

          string startStr = bookingGroup.Min(b => b["Dato"].ToString());
          DateTime arrivalDate = DateTime.SpecifyKind(DateTime.Parse(startStr), DateTimeKind.Utc);

          string endStr = bookingGroup.Max(b => b["Dato"].ToString());
          DateTime departureDate = DateTime.SpecifyKind(DateTime.Parse(endStr), DateTimeKind.Utc);

          DataRow row = bookingGroup.First();
          string status = row["Status"].ToString();
          string origin1 = row["HvorDuFra"].ToString();
          string origin2 = row["Fra"].ToString();
          string purpose = row["Formaal"].ToString();
          string contactName = row["KontaktPerson"].ToString();
          string contactEmail = row["Email"].ToString();
          string contactAddress = row["Adresse"].ToString();
          string comments = row["Bem"].ToString();
          decimal.TryParse(row["AftaltLeje"].ToString(), out decimal rentalPrice);

          int tenantCategoryId = await GetTenantCategory(origin1);

          BookingDataModel booking = new BookingDataModel
          {
            BookingNumber = bookingNumber,
            ArrivalDate = arrivalDate,
            DepartureDate = departureDate,
            OnlySelectedWeekdays = false,
            SelectedWeekdays = new List<WeekdayType>(),
            TenantCategoryId = tenantCategoryId,
            TenantName = origin2,
            Purpose = purpose,
            ContactName = contactName,
            ContactPhone = "",
            ContactAddress = contactAddress,
            ContactCity = "",
            ContactEMail = contactEmail,
            Comments = "Importeret i 2021 fra gammelt bookingsystem\n\n" + comments,
            RentalPrice = Math.Abs(rentalPrice),
            Deposit = 0,
            BookingState = BookingDataModel.BookingStateType.Closed,
            IsApproved = true,
            IsCheckedOut = true,
            WelcomeLetterIsSent = true
          };

          if (booking.ArrivalDate.Value.Year >= 2021)
          {
            if (status == "Forespørgsel")
            {
              booking.BookingState = BookingDataModel.BookingStateType.Requested;
              booking.IsApproved = false;
              booking.WelcomeLetterIsSent = false;
              booking.IsCheckedOut = false;
            }
            else if (status == "Bekræftet")
            {
              booking.BookingState = BookingDataModel.BookingStateType.Approved;
              booking.IsApproved = true;
              booking.WelcomeLetterIsSent = false;
              booking.IsCheckedOut = false;
            }
            else if (status == "Nøgle sendt")
            {
              booking.BookingState = BookingDataModel.BookingStateType.Approved;
              booking.IsApproved = true;
              booking.WelcomeLetterIsSent = true;
              booking.IsCheckedOut = false;
            }
          }

          var addCommand = new AddCustomEntityCommand
          {
            CustomEntityDefinitionCode = BookingCustomEntityDefinition.DefinitionCode,
            Model = booking,
            Title = booking.MakeTitle(),
            Publish = true
          };

          await DomainRepository.WithElevatedPermissions().CustomEntities().AddAsync(addCommand);

          if (++count >= 10000)
            break;
        }
        catch (ValidationException ex)
        {
          if (ex.ValidationResult is CompositeValidationResult vres)
            throw new Exception($"Failed to validate booking number {bookingGroup.Key}: {vres.Results?.FirstOrDefault()}.", ex);
          else
            throw new Exception($"Failed to validate booking number {bookingGroup.Key}: {ex.ValidationResult?.ErrorMessage} ({ex.ValidationResult?.MemberNames?.FirstOrDefault()}).", ex);
        }
        catch (Exception ex)
        {
          throw new Exception($"Failed to import booking number {bookingGroup.Key}.", ex);
        }
      }

      //return Task.CompletedTask;
    }


    private async Task<int> GetTenantCategory(string name)
    {
      await Task.Delay(1);

      if (name == "Spejdergruppe i Allerød")
        return 6;
      else if (name == "Spejdergruppe uden for Allerød")
        return 1100;
      else if (name == "Forening i Allerød kommune"
        || name == "Forening i Allerýmmune")
        return 7;
      else if (name == "Forældre til spejder i 1. Lillerød eller Palnatoke Gruppe"
        || name == "Forældre til spejder i 1. Lillerød eller Palnetoke Gruppe")
        return 1099;
      else if (name == "Leder i 1. Lillerød eller Palnatoke gruppe")
        return 1101;
      else if (name == "Medlemmer af Bøgevang vejforening")
        return 1102;
      else if (name == "Andre")
        return 1103;
      else
        throw new Exception($"Unknown tenant category: '{name}'.");
    }
  }
}

﻿using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Templates.Domain;
using Bogevang.Templates.Domain.CustomEntities;
using Cofoundry.Core.Mail;
using System;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings
{
  public class BookingMailService : IBookingMailService
  {
    private readonly IBookingProvider BookingProvider;
    private readonly ITemplateProvider TemplateProvider;


    public BookingMailService(
      IBookingProvider bookingProvider,
      ITemplateProvider templateProvider)
    {
      BookingProvider = bookingProvider;
      TemplateProvider = templateProvider;
    }


    public async Task<BookingMail> CreateBookingMail(int bookingId, string templateName)
    {
      var booking = await BookingProvider.GetBookingSummaryById(bookingId);
      TemplateDataModel template = await TemplateProvider.GetTemplateByName(templateName);

      booking.ArrivalDate = booking.ArrivalDate.ToLocalTime();
      booking.DepartureDate = booking.DepartureDate.ToLocalTime();
      string message = TemplateProvider.MergeText(template.Text, booking);

      return new BookingMail
      {
        Description = template.Description,
        To = new MailAddress(booking.ContactEMail, booking.ContactName),
        Subject = template.Subject,
        Message = message
      };
    }
  }
}

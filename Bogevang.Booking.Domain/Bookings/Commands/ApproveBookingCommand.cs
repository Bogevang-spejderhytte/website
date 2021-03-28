﻿using Cofoundry.Domain.CQS;

namespace Bogevang.Booking.Domain.Bookings.Commands
{
  public class ApproveBookingCommand : ICommand
  {
    public int Id { get; set; }
  }
}

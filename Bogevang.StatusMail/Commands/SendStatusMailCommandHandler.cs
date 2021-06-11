using System;
using System.Threading.Tasks;
using Cofoundry.Core.Mail;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Bogevang.StatusMail.Domain.Commands
{
  public class SendStatusMailCommandHandler :
    ICommandHandler<SendStatusMailCommand>,
    IIgnorePermissionCheckHandler
  {
    protected readonly IStatusMailProvider StatusMailProvider;

    protected readonly StatusMailSettings Settings;
    private readonly IMailDispatchService MailDispatchService;


    public SendStatusMailCommandHandler(
      IStatusMailProvider statusMailProvider,
      IMailDispatchService mailDispatchService,
      StatusMailSettings statusMailSettings)
    {
      StatusMailProvider = statusMailProvider;
      MailDispatchService = mailDispatchService;
      Settings = statusMailSettings;
    }


    public async Task ExecuteAsync(SendStatusMailCommand command, IExecutionContext executionContext)
    {
      string mailContent = await StatusMailProvider.BuildStatusMessage();

      MailAddress to = new MailAddress(Settings.MailReceiver);
      MailMessage message = new MailMessage
      {
        To = to,
        Subject = "Bøgevang statusopdatering",
        HtmlBody = mailContent
      };

      await MailDispatchService.DispatchAsync(message);
    }
  }
}

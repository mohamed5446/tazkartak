using Twilio.Rest.Api.V2010.Account;
namespace Tazkartk.SMS
{
    public interface ISMSService
    {
      Task< MessageResource> Send(string PhoneNumber, string Body);
    }
}

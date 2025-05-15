using Microsoft.Extensions.Options;
using Tazkartk.Helpers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Tazkartk.SMS
{
    public class SMSService : ISMSService
    {
        private readonly TwilioSettings _Twiliosettings;
        public SMSService(IOptions<TwilioSettings>twiliosettings)
        {
            _Twiliosettings=twiliosettings.Value;   
        }
        public async Task<MessageResource> Send(string PhoneNumber, string Body)
        {
            TwilioClient.Init(_Twiliosettings.AccountsSID, _Twiliosettings.AuthToken);
            var result=await MessageResource.CreateAsync(
                body:Body,
                from:new Twilio.Types.PhoneNumber(_Twiliosettings.TwilioPhoneNumber),
                to:PhoneNumber
                );
            return result; 
        }
    }
}

using Demo.Presentation.Settings;
using Demo.Presentation.Utitlities;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;

namespace Demo.Presentation.Helpers
{
    public class SmsService(IOptions<SmsSettings> options) : ISmsService
    {
        public MessageResource SendSms(SmsMessage smsMessage)
        {
            TwilioClient.Init(options.Value.AccountSID,options.Value.AuthToken);

            var message = MessageResource.Create(
                    body:smsMessage.Body,
                    from: new Twilio.Types.PhoneNumber(options.Value.TwilioPhoneNumber),
                    to: smsMessage.PhoneNumber
                );

            return message;
        }
    }
}

using Demo.Presentation.Utitlities;
using Twilio.Rest.Api.V2010.Account;

namespace Demo.Presentation.Helpers
{
    public interface ISmsService
    {
        MessageResource SendSms(SmsMessage smsMessage);
    }
}

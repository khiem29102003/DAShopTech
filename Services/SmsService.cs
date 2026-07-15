using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using Twilio.Clients;

public class SmsService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _twilioPhoneNumber;

    public SmsService(IConfiguration configuration)
    {
        _accountSid = configuration["Twilio:AccountSid"];
        _authToken = configuration["Twilio:AuthToken"];
        _twilioPhoneNumber = configuration["Twilio:PhoneNumber"];
    }

    public void SendSms(string toPhoneNumber, string message)
    {
        // Kiểm tra số điện thoại nhận tin nhắn (To) và số điện thoại gửi tin nhắn (From)
        if (toPhoneNumber == _twilioPhoneNumber)
        {
            throw new InvalidOperationException("The 'To' phone number cannot be the same as the 'From' phone number.");
        }

        var formattedPhoneNumber = FormatPhoneNumber(toPhoneNumber);

        var messageOptions = new CreateMessageOptions(new PhoneNumber(formattedPhoneNumber))
        {
            From = new PhoneNumber(_twilioPhoneNumber),
            Body = message
        };

        var client = new TwilioRestClient(_accountSid, _authToken);
        var msg = MessageResource.Create(messageOptions, client);
    }

    private string FormatPhoneNumber(string phoneNumber)
    {
        if (phoneNumber.StartsWith("0"))
        {
            phoneNumber = "+84" + phoneNumber.Substring(1);
        }

        return phoneNumber;
    }
}

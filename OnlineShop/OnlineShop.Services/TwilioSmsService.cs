using OnlineShop.DTO;
using OnlineShop.Services.Interfaces;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace OnlineShop.Services
{
    public class TwilioSmsService : ISmsService
    {
        const string accountSid = "AC7ee9ce3104b8f87e7caf3d1b17eb899e";
        const string authToken = "7585663101907a2ac2f550e48213d465";

        public async Task<SmsServiceResponseDTO> SendVerificationCode(string phoneNumber, string code)
        {
            TwilioClient.Init(accountSid, authToken);
            var obj = MessageResource.CreateAsync(
                body: code,
                from: new Twilio.Types.PhoneNumber("+15017122661"),
                to: new Twilio.Types.PhoneNumber($"{phoneNumber}"));

            return await Task.FromResult(new SmsServiceResponseDTO
            {
                StatusCode = 200,
                Message = "Сообщения успешно отправлено"
            });
        }
    }
}

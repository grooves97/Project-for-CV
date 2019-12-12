using BraintreeHttp;
using OnlineShop.Domain;
using OnlineShop.DTO;
using OnlineShop.Services.Interfaces;
using PayPal.Core;
using PayPal.v1.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Services
{
    public class PayPalService : IPaymentService
    {
        private const string APPROVAL_URL = "approval_url";
        public async Task<PaymentServiceResponseDTO> CreateInvoice(Domain.Order order)
        {
            var environment = new SandboxEnvironment("AVKtkv3o13BU3eqJMpxTDJepdIsPUy1wwZNItREABfTzfc5pVhunjudf5LhzctAPw_WTC6Vvgaap5HSu", "EFCsGNXXcPrxCGkyuJM5d4Ge-fKzBeBtvE6tcDk4LOAGAFghMXDIPai8hMWKah5LLmz2ZaIPzG69fyzY");
            var client = new PayPalHttpClient(environment);

            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = "10",
                            Currency = "USD"
                        }
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = "https://example.com/cancel",
                    ReturnUrl = "https://example.com/return"
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };

            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);

            try
            {
                HttpResponse response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();

                return new PaymentServiceResponseDTO
                {
                    PaymentUrl = result.Links.FirstOrDefault(link => link.Rel == APPROVAL_URL).Href
                };
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();
                return null;
            }


        }
    }
}

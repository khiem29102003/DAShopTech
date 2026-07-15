using Newtonsoft.Json;

namespace DAShopTech.Services
{
    public class PaymentService : IPaymentService
    {
        public bool CheckPaymentWithMomo(int orderId, decimal amount)
        {
            using (var httpClient = new HttpClient())
            {
                var requestUri = $"https://api.momo.vn/v1/payment/status?orderId={orderId}";
                var response = httpClient.GetStringAsync(requestUri).Result;

                // Giả sử phản hồi là JSON với thuộc tính "status"
                dynamic result = JsonConvert.DeserializeObject(response);
                return result.status == "success";
            }
        }
    }

}

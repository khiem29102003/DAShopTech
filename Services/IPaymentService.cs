namespace DAShopTech.Services
{
    public interface IPaymentService
    {
        bool CheckPaymentWithMomo(int orderId, decimal amount);
    }

}

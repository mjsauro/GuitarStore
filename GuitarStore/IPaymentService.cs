using Braintree;

namespace GuitarStore
{
    public interface IPaymentService
    {
        Customer GetCustomer(string email);

        Customer UpdateCustomer(string firstName, string lastName, string id);

        void DeleteAddress(string email, string id);

        void AddAddress(string email, string firstName, string lastName, string company, string streetAddress, string extendedAddress, string locality, string region, string postalCode, string countryName);

        string AuthorizeCard(string email, decimal total, decimal tax, string trackingNumber, string addressId, string cardholderName, string cvv, string cardNumber, string expirationMonth, string expirationYear);

        void CreatePaymentMethod(string email, string cardHolderName, string number, string cvv, string expirationMonth, string expirationYear);

        void AddCreditCard(string email, string cardHolderName, string number, string cvv, string expirationMonth, string expirationYear);

        void DeleteCreditCard(string email, string token);
    }
}
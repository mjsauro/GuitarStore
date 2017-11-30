using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Braintree;

namespace GuitarStore
{
    public class GuitarStorePaymentService : IPaymentService
    {
        protected Braintree.IBraintreeGateway gateway;

        public GuitarStorePaymentService()
        {
            string merchantId = System.Configuration.ConfigurationManager.AppSettings["Braintree.MerchantId"];
            string environment = System.Configuration.ConfigurationManager.AppSettings["Braintree.Environment"];
            string publicKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PublicKey"];
            string privateKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PrivateKey"];
            gateway = new Braintree.BraintreeGateway(environment, merchantId, publicKey, privateKey);
        }

        public Braintree.Customer GetCustomer(string email)
        {
            var customerGateway = gateway.Customer;
            Braintree.CustomerSearchRequest query = new Braintree.CustomerSearchRequest();
            query.Email.Is(email);
            var matchedCustomers = customerGateway.Search(query);
            Braintree.Customer customer = null;
            if (matchedCustomers.Ids.Count == 0)
            {
                Braintree.CustomerRequest newCustomer = new Braintree.CustomerRequest();
                newCustomer.Email = email;

                var result = customerGateway.Create(newCustomer);
                customer = result.Target;
            }
            else
            {
                customer = matchedCustomers.FirstItem;
            }
            return customer;
        }

        public void DeleteAddress(string email, string id)
        {
            Customer c = GetCustomer(email);
            gateway.Address.Delete(c.Id, id);
        }

        public void AddAddress(string email, string firstName, string lastName, string company, string streetAddress, string extendedAddress, string locality, string region, string postalCode, string countryName)
        {
            Customer c = GetCustomer(email);

            Braintree.AddressRequest newAddress = new Braintree.AddressRequest
            {
                FirstName = firstName,
                LastName = lastName,
                Company = company,
                CountryName = countryName,
                PostalCode = postalCode,
                ExtendedAddress = extendedAddress,
                Locality = locality,
                Region = region,
                StreetAddress = streetAddress
            };

            gateway.Address.Create(c.Id, newAddress);
        }

        public string AuthorizeCard(string email, decimal total, decimal tax, string trackingNumber, string addressId, string cardholderName, string cvv, string cardNumber, string expirationMonth, string expirationYear)
        {
            var customer = GetCustomer(email);
            Braintree.TransactionRequest transaction = new Braintree.TransactionRequest();
            //transaction.Amount = 1m;    //I can hard-code a dollar amount for now to test everything else
            transaction.Amount = total;
            transaction.TaxAmount = tax;
            transaction.OrderId = trackingNumber;
            transaction.CustomerId = customer.Id;
            transaction.ShippingAddressId = addressId;
            //https://developers.braintreepayments.com/reference/general/testing/ruby
            transaction.CreditCard = new Braintree.TransactionCreditCardRequest
            {
                CardholderName = cardholderName,
                CVV = cvv,
                Number = cardNumber,
                ExpirationYear = expirationYear,
                ExpirationMonth = expirationMonth
            };

            var result = gateway.Transaction.Sale(transaction);

            return result.Message;
        }

        public void CreatePaymentMethod(string email, string cardHolderName, string number, string cvv, string expirationMonth, string expirationYear)
        {
            Customer c = GetCustomer(email);

            Braintree.PaymentMethodRequest request = new Braintree.PaymentMethodRequest();
            {
                request.CardholderName = cardHolderName;
                request.Number = number;
                request.CVV = cvv;
                request.ExpirationMonth = expirationMonth;
                request.ExpirationYear = expirationYear;
            }
            gateway.PaymentMethod.Create(request);
        }

        public void AddCreditCard(string email, string cardHolderName, string number, string cvv, string expirationMonth, string expirationYear)
        {
            Customer c = GetCustomer(email);
            string customerID = c.Id;
            Braintree.CreditCardRequest newCard = new CreditCardRequest();
            {
                newCard.CustomerId = customerID;
                newCard.CardholderName = cardHolderName;
                newCard.Number = number;
                newCard.CVV = cvv;
                newCard.ExpirationMonth = expirationMonth;
                newCard.ExpirationYear = expirationYear;
            };
            CreditCard creditCard = gateway.CreditCard.Create(newCard).Target;
        }

        public Customer UpdateCustomer(string firstName, string lastName, string id)
        {
            Braintree.CustomerRequest request = new Braintree.CustomerRequest();
            request.FirstName = firstName;
            request.LastName = lastName;
            var result = gateway.Customer.Update(id, request);
            return result.Target;
        }
    }
}
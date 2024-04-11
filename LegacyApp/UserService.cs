using System;

namespace LegacyApp
{
    public class UserService
    {
        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!UserValidation.Validate(firstName,lastName, email, dateOfBirth))
            {
                return false;
            }
            var clientRepository = new ClientRepository();
            var client = clientRepository.GetById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName
            };

            switch (client.Type)
            {
                case "VeryImportantClient":
                    user.HasCreditLimit = false;
                    break;
                case "ImportantClient":
                {
                    using var userCreditService = new UserCreditService();
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    creditLimit = creditLimit * 2;
                    user.CreditLimit = creditLimit;
                    break;
                }
                default:
                {
                    user.HasCreditLimit = true;
                    using var userCreditService = new UserCreditService();
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    user.CreditLimit = creditLimit;
                    break;
                }
            }
            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                return false;
            }
            UserDataAccess.AddUser(user);
            return true;
        }
    }
}

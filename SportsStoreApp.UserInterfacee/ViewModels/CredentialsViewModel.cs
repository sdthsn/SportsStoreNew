
using SportsStoreApp.UserInterface.ViewModels.Validations;
using FluentValidation.Attributes;

namespace SportsStoreApp.UserInterface.ViewModels
{
    [Validator(typeof(CredentialsViewModelValidator))]
    public class CredentialsViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

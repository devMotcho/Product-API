namespace ProductApi.Dtos
{
    public partial class UserForRegistrationDto
    {
        public string Email {set; get;} = "";

        public string Password {set; get;} = "";

        public string PasswordConfirm {set; get;} = "";
        
        public UserForRegistrationDto()
        {
            
        }
    }
}
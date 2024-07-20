namespace ProductApi.Dtos
{
    public partial class UserForLoginDto
    {
        public string Email {set; get;} = "";

        public string Password {set; get;} = "";

        public UserForLoginDto()
        {

        }
    }
}
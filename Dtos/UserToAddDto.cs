namespace ProductApi.Models
{
    public partial class UserToAddDto
    {
        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Gender { get; set; } = "";

        public bool Active { set; get; }

        public UserToAddDto()
        {

        }
    }
}
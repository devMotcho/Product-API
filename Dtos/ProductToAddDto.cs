namespace ProductApi.Dtos
{
    public partial class ProductToAddDto
    {
        public string Name { set; get; } = "";

        public string Description { set; get; } = "";

        public decimal Price { set; get; }

        public bool Active { set; get; }
    }
}
namespace ProductApi.Models
{
    public partial class Product
    {
        public int ProductId {set; get;}

        public string Name {set; get;} = "";
        
        public string Description {set; get;} = "";
        
        public decimal Price {set; get;}
        
        public bool Active {set; get;}
    }
}
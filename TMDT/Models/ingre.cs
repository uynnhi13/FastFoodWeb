namespace TMDT.Models
{
    public class ingre
    {
        public int id { get; set; }
        public double quantity { get; set; }


        public ingre() { }
        public ingre(int id, double quantity)
        {
            this.id = id;
            this.quantity = quantity;
        }


    }
}
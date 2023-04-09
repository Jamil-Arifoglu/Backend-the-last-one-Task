namespace P230_Pronia.Entities
{
    public class PlantCategory : BaseEntity
    {

        public Plant Plant { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
    }
}

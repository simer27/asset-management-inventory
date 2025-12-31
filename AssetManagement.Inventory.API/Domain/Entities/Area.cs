namespace AssetManagement.Inventory.API.Domain.Entities
{
    public class Area
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }


        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}

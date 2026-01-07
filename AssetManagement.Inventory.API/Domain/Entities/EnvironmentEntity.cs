namespace AssetManagement.Inventory.API.Domain.Entities
{
    public class EnvironmentEntity
    {
        public Guid Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        
        public ICollection<EnvironmentImage> Imagens { get; set; } = new List<EnvironmentImage>();
    }
}

namespace AssetManagement.Inventory.API.DTOs.EnvironmentDto
{
    public class EnvironmentDetailsDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public List<string> Imagens { get; set; } = new();
    }
}

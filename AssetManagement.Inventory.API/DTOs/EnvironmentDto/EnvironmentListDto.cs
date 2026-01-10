namespace AssetManagement.Inventory.API.DTOs.EnvironmentDto
{
    public class EnvironmentListDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public List<string> Imagens { get; set; }
    }
}

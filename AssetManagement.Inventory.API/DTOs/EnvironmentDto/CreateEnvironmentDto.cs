namespace AssetManagement.Inventory.API.DTOs.EnvironmentDto
{
    public class CreateEnvironmentDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}

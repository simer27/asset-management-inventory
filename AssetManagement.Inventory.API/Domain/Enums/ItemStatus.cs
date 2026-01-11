using System.ComponentModel;

namespace AssetManagement.Inventory.API.Domain.Enums
{
    public enum ItemStatus
    {
        [Description("Ativo")]
        Ativo = 1,

        [Description("Vendido")]
        Vendido = 2,

        [Description("Emprestado")]
        Emprestado = 3,

        [Description("Em Manutenção")]
        Manutencao = 4,

        [Description("Descartado")]
        Descartado = 5
    }
}

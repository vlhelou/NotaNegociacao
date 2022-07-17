using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotaNegociacao.Dados;

internal record Operacao
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string CompraVenda { get; set; }
    public string Produto { get; set; }
    public DateTime Data { get; set; }
    public int Quantidade { get; set; }
    public decimal Preco { get; set; }
    public decimal Valor { get; set; }
    public string? DebitoCredito { get; set; }
    public decimal TaxaOperacional { get; set; }
}

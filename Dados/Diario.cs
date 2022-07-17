using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotaNegociacao.Dados
{
    internal record Diario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public DateTime DataPregao { get; set; }
        public decimal Bruto { get; set; }
        public decimal IRRF { get; set; }
        public decimal TaxaOperacional { get; set; }
        public decimal RegistroB3 { get; set; }
        public decimal TaxaB3 { get; set; }
        public decimal Impostos { get; set; }
        public decimal Liquido { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace API_INT_SOC_EXPORTA_DADOS.Models
{
    public class AsoPeriodo
    {
        [Required]
        public string? empresa { get; set; }
        [Required]
        public string? codigo { get; set; }
        [Required]
        public string? chave { get; set; }
        [Required]
        public string? tipoSaida { get; set; }
        [Required]
        public string? dataInicio { get; set; }
        [Required]
        public string? dataFim { get; set; }
        public string? tipoBusca { get; set; }
    }
}

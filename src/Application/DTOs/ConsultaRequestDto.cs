using System.ComponentModel.DataAnnotations;

namespace PousadaApi.Application.DTOs;

public class ConsultaRequestDto
{
    [Required]
    public int PousadaId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Pergunta { get; set; } = string.Empty;

    public List<ConsultaHistoricoItemDto>? Historico { get; set; }
}

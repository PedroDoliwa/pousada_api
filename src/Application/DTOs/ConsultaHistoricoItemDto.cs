using System.ComponentModel.DataAnnotations;

namespace PousadaApi.Application.DTOs;

public class ConsultaHistoricoItemDto
{
    [Required]
    [RegularExpression("^(user|assistant)$")]
    public string Role { get; set; } = "user";

    [Required]
    [StringLength(4000)]
    public string Conteudo { get; set; } = string.Empty;
}

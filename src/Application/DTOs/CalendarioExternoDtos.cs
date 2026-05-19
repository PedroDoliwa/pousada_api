namespace PousadaApi.Application.DTOs;

public class CalendarioExternoCreateDto
{
    public int QuartoId { get; set; }
    public string Canal { get; set; } = "Outro";
    public string UrlImportacao { get; set; } = string.Empty;
}

public class CalendarioExternoUpdateDto
{
    public int Id { get; set; }
    public string Canal { get; set; } = "Outro";
    public string UrlImportacao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}

public class CalendarioExternoReadDto
{
    public int Id { get; set; }
    public int QuartoId { get; set; }
    public string Canal { get; set; } = string.Empty;
    public string UrlImportacao { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime? UltimaSincronizacao { get; set; }
    public string? UltimoErro { get; set; }
}

public class CalendarioSyncResultDto
{
    public int Criados { get; set; }
    public int Atualizados { get; set; }
    public int Cancelados { get; set; }
    public int Ignorados { get; set; }
}

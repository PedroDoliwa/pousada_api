namespace PousadaApi.Infrastructure.Options;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public string From { get; set; } = "noreply@pousada.local";
    public bool EnableSsl { get; set; } = true;
}

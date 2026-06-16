namespace PousadaApi.Infrastructure.Options;

public sealed class ResendOptions
{
    public const string SectionName = "Resend";

    public string ApiKey { get; set; } = "";
    public string From { get; set; } = "noreply@pousada.local";
}

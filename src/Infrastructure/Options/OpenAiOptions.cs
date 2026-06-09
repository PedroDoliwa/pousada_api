namespace PousadaApi.Infrastructure.Options;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAI";

    public string ApiKey { get; set; } = "";

    public string Model { get; set; } = "gpt-4o-mini";

    public int MaxOutputTokens { get; set; } = 600;

    public int MaxToolRounds { get; set; } = 3;
}

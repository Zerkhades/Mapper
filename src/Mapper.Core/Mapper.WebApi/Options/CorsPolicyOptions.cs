namespace Mapper.WebApi.Options;

public class CorsPolicyOptions
{
    public const string SectionName = "Cors";

    public bool AllowAnyOriginInDevelopment { get; set; } = true;

    public string[] AllowedOrigins { get; set; } = [];
}

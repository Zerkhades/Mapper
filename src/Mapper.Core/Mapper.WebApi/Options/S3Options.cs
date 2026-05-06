namespace Mapper.WebApi.Options;

public class S3Options
{
    public const string SectionName = "S3";

    public string? ServiceUrl { get; set; }

    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; }

    public string? Bucket { get; set; }
}

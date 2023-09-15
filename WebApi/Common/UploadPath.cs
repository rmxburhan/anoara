using Org.BouncyCastle.Asn1.Ess;

namespace WebApi.Common;

public class UploadPath : IUploadPath
{
    private readonly IConfiguration configuration;

    public UploadPath(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string ClassResource()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", configuration.GetSection("UploadPath:ClassResources").Value);
    }

    public string MilestonePath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "milestone");
    }

    public string PostImage()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "post");
    }

    public string StudentPhoto()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "student");
    }

    public string TeacherPhoto()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "teacher");
    }
}
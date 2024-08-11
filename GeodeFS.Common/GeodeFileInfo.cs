using System.ComponentModel.DataAnnotations;

namespace GeodeFS.Common;

#nullable disable

public class GeodeFileInfo
{
    [MaxLength(Maximum.FileNameLength)]
    public string Name { get; set; }
    [MaxLength(Maximum.FileTypeLength)]
    public string Type { get; set; }
}
namespace GeodeFS.Common;

#nullable disable

[MessagePackObject]
public class GeodeFileInfo
{
    [Key(0), MaxLength(Maximum.FileNameLength)]
    public string Name { get; set; }
    [Key(1), MaxLength(Maximum.FileTypeLength)]
    public string Type { get; set; }
}
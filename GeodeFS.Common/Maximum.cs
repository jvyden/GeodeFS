namespace GeodeFS.Common;

public static class Maximum
{
    /// <summary>
    /// The amount of data within files a user is allowed to store, excluding metadata.
    /// </summary>
    public const int TotalStorageLimit = 32_768;

    public const int FilesPerUser = 8;
    public const int FileNameLength = 32;
    public const int FileTypeLength = 32;

    public const int DetailsPerUser = 4;
    public const int DetailKeyLength = 8;
    public const int DetailValueLength = 24;
}
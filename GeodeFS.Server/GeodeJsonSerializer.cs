using Bunkum.Listener.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GeodeFS.Server;

public class GeodeJsonSerializer : IBunkumSerializer
{
    private static readonly JsonSerializer JsonSerializer = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
    };

    /// <inherit-doc/>
    public string[] ContentTypes { get; } =
    [
        ContentType.Json
    ];
    
    /// <inherit-doc/>
    public byte[] Serialize(object data)
    {
        using MemoryStream stream = new();
        using StreamWriter sw = new(stream);
        using JsonWriter writer = new JsonTextWriter(sw);

        JsonSerializer.Serialize(writer, data);
        writer.Flush();
        return stream.ToArray();
    }
}
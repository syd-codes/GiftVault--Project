using System.Text.Json.Serialization;

public class Family
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("members")]
    public List<Member> Members { get; set; } = [];
}
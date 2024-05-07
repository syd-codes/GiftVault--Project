using System.Text.Json.Serialization;

public class Member
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("birthday")]
    public DateOnly Birthday { get; set; }

    [JsonPropertyName("giveGiftToName")]
    public string GiveToName { get; set; } = default!;

    [JsonPropertyName("giveGiftToGiftIdea")]
    public string GiveToGiftIdea { get; set; } = default!;

    [JsonPropertyName("idea")]
    public string GiftIdea { get; set; } = default!;

    [JsonPropertyName("avoid")]
    public string[] AvoidMembers { get; set; } = [];

    public string GetUniqueName(string familyName) => $"{familyName}/{Name}";
}

public class GiftIdea
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("idea")]
    public string Idea { get; set; } = default!;
}

public class AvoidMembers
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("avoid")]
    public string[] Avoid { get; set; } = default!;
}

public class GiveToName
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("giveGiftToName")]
    public string GiftReceiverName { get; set; } = default!;
}

public class GiveToGiftIdea
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("giveGiftToGiftIdea")]
    public string GiftIdeaName { get; set; } = default!;
}

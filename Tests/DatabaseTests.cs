using System.Text.Json;

namespace Tests;

public class DatabaseTests : IDisposable
{
    private readonly string _groupName = "test";

    public void Dispose()
    {
        DeleteJsonFiles();
    }

    [Fact]
    public void Directory_Exists()
    {
        // arrange
        var directory = Database.Directory;

        // act
        var exists = Directory.Exists(directory);

        // assert
        Assert.True(exists);
    }

    [Fact]
    public void ListExistingGroups_None_ReturnsCorrectList()
    {
        // arrange
        DeleteJsonFiles();

        // act
        var groups = Database.ListExistingFamilies();

        // assert
        Assert.Empty(groups);
    }

    [Fact]
    public void ListExistingGroups_One_ReturnsCorrectList()
    {
        // arrange
        DeleteJsonFiles();
        CreateJsonFile(new Family { Name = _groupName });

        // act
        var groups = Database.ListExistingFamilies();

        // assert
        Assert.Single(groups);
        Assert.Equal((_groupName, $"{_groupName}.json"), groups.First());
    }

    [Fact]
    public void Constructor_InitializesGroup_Correctly()
    {
        // arrange
        DeleteJsonFiles();

        // act
        var d = new Database(_groupName);


        // assert
        Assert.NotNull(d.Family);
    }

    [Fact]
    public void Constructor_ThrowsException_ForNullEmptyGroupName()
    {
        // arrange
        var groupName = "";

        // act
        void Act() => new Database(groupName);

        // assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Constructor_LoadsExistingGroup()
    {
        // arrange
        DeleteJsonFiles();
        var group = new Family { Name = _groupName };
        CreateJsonFile(JsonSerializer.Serialize(group));

        // act
        var d = new Database(_groupName);

        // assert
        Assert.NotNull(d.Family);
        Assert.Equal(_groupName, d.Family.Name);
    }

    [Fact]
    public void Constructor_HandlesParseErrors()
    {
        // arrange
        DeleteJsonFiles();
        var path = Path.Combine(Database.Directory, $"{_groupName}.json");
        File.WriteAllText(path, "}{");

        // act
        void Act() => new Database(_groupName);

        // assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void Exists_ReturnsTrue_WhenFileExists()
    {
        // arrange
        DeleteJsonFiles();
        CreateJsonFile("{}");

        // act
        var d = new Database(_groupName);

        // assert
        Assert.True(d.Exists);
    }

    [Fact]
    public void Exists_ReturnsFalse_WhenFileDoesNotExist()
    {
        // arrange
        DeleteJsonFiles();
        var d = new Database(_groupName);
        DeleteJsonFiles();

        // assert
        Assert.False(d.Exists);
    }

    [Fact]
    public void Path_HandlesInvalidCharacters()
    {
        // arrange
        var groupName = $"test{Path.GetInvalidFileNameChars()[0]}";

        // act
        var d = new Database(groupName);

        // assert
        var filename = Path.GetFileName(d.Path);
        Assert.Equal("test.json", filename);
    }

    [Fact]
    public void Path_ReturnsCorrectFilePath()
    {
        // arrange
        var d = new Database(_groupName);

        // act
        var path = d.Path;

        // assert
        Assert.Equal(Path.Combine(Database.Directory, $"{_groupName}.json"), path);
    }

    [Fact]
    public void Save_SerializesGroupCorrectly()
    {
        // arrange
        DeleteJsonFiles();
        var group = new Family { Name = _groupName };
        var d = new Database(_groupName);

        // act
        d.Save();

        // assert
        var path = Path.Combine(Database.Directory, $"{_groupName}.json");
        Assert.True(File.Exists(path));
        var content = File.ReadAllText(path);
        Assert.Equal(JsonSerializer.Serialize(group), content);
    }

    [Fact]
    public void Load_DeserializesGroupCorrectly()
    {
        // arrange
        DeleteJsonFiles();
        var group = new Family { Name = _groupName };
        CreateJsonFile(JsonSerializer.Serialize(group));
        var d = new Database(_groupName);

        // act
        d.Load();

        // assert
        Assert.NotNull(d.Family);
        Assert.Equal(_groupName, d.Family.Name);
    }

    [Fact]
    public void Load_HandlesDeserializationErrors()
    {
        // arrange
        DeleteJsonFiles();
        var d = new Database(_groupName);

        // act
        CreateJsonFile("}{");
        void Act() => d.Load();

        // assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void ListExistingGroups_ReturnsCorrectList()
    {
        // arrange
        DeleteJsonFiles();
        CreateJsonFile(JsonSerializer.Serialize(new Family { Name = "One" }), "One");
        CreateJsonFile(JsonSerializer.Serialize(new Family { Name = "Two" }), "Two");

        // act
        var groups = Database.ListExistingFamilies();

        // assert
        Assert.Equal(2, groups.Count());
        Assert.Equal(("One", "One.json"), groups.ToArray()[0]);
        Assert.Equal(("Two", "Two.json"), groups.ToArray()[1]);
    }

    [Fact]
    public void Delete_RemovesFile_IfExists()
    {
        // arrange
        CreateJsonFile(new Family { Name = _groupName });
        var database = new Database(_groupName);

        // act
        database.Delete();

        // assert
        Assert.False(File.Exists(database.Path));
    }

    [Fact]
    public void Delete_DoesNotThrow_WhenFileDoesNotExist()
    {
        // arrange
        var groupName = "nonexistent";
        var database = new Database(groupName);
        database.Delete();

        // act & assert
        Assert.False(File.Exists(database.Path));
    }

    [Fact]
    public void Rename_ChangesFileName_Correctly()
    {
        // arrange
        CreateJsonFile(new Family { Name = _groupName });
        var database = new Database(_groupName);
        var oldPath = database.Path;
        var newGroupName = "testRenamed";

        // act
        database.Rename(newGroupName);
        var newPath = database.Path;

        // assert
        Assert.False(File.Exists(oldPath));
        Assert.True(File.Exists(newPath));
    }

    internal void DeleteJsonFiles()
    {
        var jsonFile = Directory.GetFiles(Database.Directory, "*.json");
        foreach (var file in jsonFile)
        {
            File.Delete(file);
        }
        Assert.Empty(Directory.GetFiles(Database.Directory, "*.json"));
    }

    internal void CreateJsonFile(Family group, string? name = null)
    {
        new Database(group.Name).Save();
    }

    internal void CreateJsonFile(string content, string? name = null)
    {
        var path = Path.Combine(Database.Directory, $"{name ?? _groupName}.json");
        File.WriteAllText(path, content);
        Assert.True(File.Exists(path));
    }
}
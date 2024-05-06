using System.Reflection;
using System.Text.Json;

public class Database : IDatabase
{
    private string _familyName;

    public Database(string familyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(familyName, nameof(familyName));

        _familyName = familyName;

        if (Exists)
        {
            Load();
        }
        else
        {
            Family = new Family { Name = familyName };
            Save();
        }
    }

    public void Rename(string name)
    {
        File.Delete(Path);
        _familyName = Family!.Name = name;
        Save();
    }

    public bool Exists => File.Exists(Path);

    public string Path => NameToPath(_familyName);

    public Family? Family { get; private set; }

    public void Load()
    {
        if (!Exists)
        {
            throw new InvalidOperationException("Family does not exist.");
        }
        var json = File.ReadAllText(Path);
        if (string.IsNullOrEmpty(json))
        {
            throw new InvalidOperationException("Failed to read family.");
        }
        try
        {
            Family = JsonSerializer.Deserialize<Family>(json);
        }
        catch
        {
            throw new InvalidOperationException("Failed to open family.");
        }
    }

    public void Delete()
    {
        File.Delete(Path);
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Family);
        if (string.IsNullOrEmpty(json))
        {
            throw new InvalidOperationException("Failed to serialize family.");
        }
        WriteToFileSafely(Path, json);
    }

    public static string Directory
    {
        get
        {
            var assembly = Assembly.GetAssembly(typeof(Database)) ?? throw new InvalidOperationException("Unable to find the assembly for the Client project");
            var path = System.IO.Path.GetDirectoryName(assembly.Location) ?? throw new InvalidOperationException("Unable to find the directory for the Client project");
            path = System.IO.Path.Combine(path, "Data");
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            return System.IO.Directory.Exists(path) ? path : throw new InvalidOperationException("Unable to find the directory for the Client project");
        }
    }

    public static IEnumerable<(string Name, string FileName)> ListExistingFamilies()
    {
        var directory = new DirectoryInfo(Directory);
        var files = directory.GetFiles("*.json").Select(f => f.Name);
        foreach (var file in files)
        {
            var path = System.IO.Path.Combine(Directory, file);
            var json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                continue;
            }
            var family = JsonSerializer.Deserialize<Family>(json);
            var name = family?.Name ?? "Error";
            yield return (name, file);
        }
    }

    private static void WriteToFileSafely(string path, string content)
    {
        const int maxRetries = 3;
        const int delayOnRetry = 500;  // Delay in milliseconds
        int attempts = 0;

        while (attempts < maxRetries)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                    return;  // Exit the method upon successful write
                }
            }
            catch (IOException)
            {
                attempts++;
                Console.WriteLine($"Attempt {attempts} to write file '{path}' failed. Retrying after {delayOnRetry}ms.");
                Thread.Sleep(delayOnRetry);  // Wait a bit before retrying
            }
        }
        throw new IOException($"Failed to write to file '{path}' after {maxRetries} attempts.");
    }

    public static bool FamilyExists(string name) => File.Exists(NameToPath(name));

    public static string NameToPath(string name)
    {
        var invalidChars = System.IO.Path.GetInvalidFileNameChars();
        var filename = new string(name.Where(c => !invalidChars.Contains(c)).ToArray()).Trim();
        return System.IO.Path.Combine(Directory, $"{filename}.json");
    }
}
public interface IDatabase
{
    bool Exists { get; }
    string Path { get; }
    Family? Family { get; }
    void Load();
    void Save();
    void Delete();
    void Rename(string name);
}

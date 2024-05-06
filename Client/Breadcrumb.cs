using Spectre.Console;

public static class Breadcrumb
{
    private static readonly string _seperator = " > ";
    private static readonly string _default = "Home";

    static Breadcrumb() => Draw();

    private static string current = _default;
    public static string Current
    {
        get => current;
        set
        {
            current = value;
            Draw();
        }
    }

    public static void Clear() => Current = _default;

    public static void Back()
    {
        Console.Clear();
        var split = Current.Split(_seperator);
        if (split.Length == 1) return;
        Current = string.Join(_seperator, split[..^1]);
    }

    public static void Forward(string add)
    {
        ArgumentException.ThrowIfNullOrEmpty(add, nameof(add));
        Current = string.Join(_seperator, Current.Split(" > ").Append(add));
    }

    public static void Draw(bool clear = false)
    {
        if (clear || Current == _default) { Console.Clear(); }

        var (x, y) = RememberCursorPosition();

        Console.SetCursorPosition(0, 0);
        var rule = new Rule($"[yellow]{Current}[/]").LeftJustified();
        AnsiConsole.Write(rule);

        RestoreCursorPosition(x, y);

        static (int, int) RememberCursorPosition() 
            => Console.GetCursorPosition();

        static void RestoreCursorPosition(int x, int y) 
            => Console.SetCursorPosition(x, (y == 0) ? ++y : y);
    }
}
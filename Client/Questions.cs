using Spectre.Console;

public static class Questions
{
    public enum YesNo { Yes, No }

    public static YesNo AskYesNo(string text)
    {
        var prompt = new SelectionPrompt<YesNo>()
            .Title(text)
            .AddChoices(YesNo.Yes, YesNo.No);
        return AnsiConsole.Prompt(prompt);
    }

    public static string? AskString(string text, bool allowEmpty, Func<string?, ValidationResult>? valdate = null)
    {
        var prompt = new TextPrompt<string?>(text)
            .Validate(value => valdate?.Invoke(value) ?? ValidationResult.Success());
        if (allowEmpty) prompt.AllowEmpty();
        return AnsiConsole.Prompt(prompt);
    }

    public static int? AskInt(string text, bool allowEmpty, Func<int?, ValidationResult>? valdate = null)
    {
        var prompt = new TextPrompt<int?>(text)
            .Validate(value => valdate?.Invoke(value) ?? ValidationResult.Success());
        if (allowEmpty) prompt.AllowEmpty();
        return AnsiConsole.Prompt(prompt);
    }

    public static DateOnly? AskDateOnly(string text, bool allowEmpty, Func<DateOnly?, ValidationResult>? valdate = null)
    {
        var prompt = new TextPrompt<DateOnly?>(text)
            .Validate(value => valdate?.Invoke(value) ?? ValidationResult.Success());
        if (allowEmpty) prompt.AllowEmpty();
        return AnsiConsole.Prompt(prompt);
    }

    public static (int Index, string Text) Menu(string text, params string[] options)
    {
        var prompt = new SelectionPrompt<string>()
                .Title(text)
                .PageSize(10)
                .AddChoices(options);
        var choice = AnsiConsole.Prompt(prompt);
        var index = Array.IndexOf(options, choice);
        return (index, choice);
    }
}
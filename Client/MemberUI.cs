using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

using Spectre.Console;

public static class MemberUI
{
    public enum MenuOption { Edit, Delete, Back }

    public static void Show(Member person)
    {
        var name = new FigletText(person.Name).Color(Color.Yellow);
        AnsiConsole.Write(name);

        var table = new Table();
        table.AddColumns("", "");
        table.HideHeaders();
        table.AddRow("[blue]Birthday[/]", person.Birthday.ToString()!);
        AnsiConsole.Write(table);
    }

    public static MenuOption Menu(Member member)
    {
        ArgumentNullException.ThrowIfNull(member, nameof(member));

        var options = Enum.GetNames(typeof(MenuOption)).Select(name => Regex.Replace(name, "([a-z])([A-Z])", "$1 $2"));
        var selection = Questions.Menu(string.Empty, options.ToArray());
        return (MenuOption)selection.Index;
    }

    public static Member SelectOne(Database database)
    {
        ValidateParameters();

        var members = database.Family!.Members.Select((p, i) => $"{p.Name}").ToArray();

        if (members.Length == 1)
        {
            return database.Family.Members[0];
        }

        var selection = Questions.Menu("[blue]Choose a person to load.[/]", members);

        return database.Family.Members[selection.Index];

        void ValidateParameters()
        {
            ArgumentNullException.ThrowIfNull(database, nameof(database));
            ArgumentNullException.ThrowIfNull(database.Family, nameof(database.Family));
            ArgumentOutOfRangeException.ThrowIfLessThan(database.Family.Members.Count, 1, nameof(database.Family.Members));
        }
    }

    public static void Edit(Database database, Member member)
    {
        ArgumentNullException.ThrowIfNull(member, nameof(member));

        member.Name = Questions.AskString("What is the person's name?", true, ValidateName) ?? member.Name;

        member.Birthday = Questions.AskDateOnly("What is the person's birthday?", true, ValidateBirthday) ?? member.Birthday;

        ValidationResult ValidateName(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ValidationResult.Error("Name cannot be empty.");
            }
            else if (value != member.Name && database.Family!.Members.Any(p => p.Name == value))
            {
                return ValidationResult.Error("Name already exists.");
            }
            else
            {
                return ValidationResult.Success();
            }
        }

        static ValidationResult ValidateBirthday(DateOnly? value)
        {
            if (!value.HasValue)
            {
                return ValidationResult.Success();
            }
            else if (value > DateOnly.FromDateTime(DateTime.Now))
            {
                return ValidationResult.Error("Birthday cannot be in the future.");
            }
            else
            {
                return ValidationResult.Success();
            }
        }
    }

    public static Member Create(Database database)
    {
        return new Member
        {
            Name = Questions.AskString("What is the person's name?", false, ValidateName)!,
            Birthday = Questions.AskDateOnly("What is the person's birthday?", false, ValidateBirthday)!.Value
        };

        ValidationResult ValidateName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Error("Name cannot be empty.");
            }
            else if (database.Family!.Members.Any(p => p.Name == value))
            {
                return ValidationResult.Error("Name already exists.");
            }
            else
            {
                return ValidationResult.Success();
            }
        }

        static ValidationResult ValidateBirthday(DateOnly? value)
        {
            if (value > DateOnly.FromDateTime(DateTime.Now))
            {
                return ValidationResult.Error("Birthday cannot be in the future.");
            }
            else
            {
                return ValidationResult.Success();
            }
        }
    }

    internal static void Delete(Database? database, Member member)
    {
        ArgumentNullException.ThrowIfNull(database, nameof(database));
        ArgumentNullException.ThrowIfNull(database.Family, nameof(database.Family));
        ArgumentNullException.ThrowIfNull(member, nameof(member));

        if (Questions.AskYesNo("Are you sure you want to delete this family?") == Questions.YesNo.Yes)
        {
            database.Family.Members.Remove(member);
        }
    }
}
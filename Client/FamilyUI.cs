using System.Text.RegularExpressions;

using Spectre.Console;

static class FamilyUI
{
    public enum MenuOption
    {
        Rename,
        Delete,
        AddMember,
        OpenMember,
        Back,
    }

    public static void Show(Database? database)
    {
        ValidateParameters();

        var name = new FigletText(database!.Family!.Name).Color(Color.Yellow);
        AnsiConsole.Write(name);

        ShowMembers(database);

        void ValidateParameters()
        {
            ArgumentNullException.ThrowIfNull(database, nameof(database));
            ArgumentNullException.ThrowIfNull(database.Family, nameof(database.Family));
        }

        static void ShowMembers(Database database)
        {
            if (database!.Family!.Members.Count == 0)
            {
                AnsiConsole.MarkupLine("[blue]No members in the family.[/]");
            }
            else
            {
                var table = new Table();
                table.AddColumns("");
                table.HideHeaders();
                table.AddRow("[blue]Members[/]");
                foreach (var member in database.Family.Members)
                {
                    table.AddRow($"[yellow]{member.Name}[/]");
                }
                AnsiConsole.Write(table);
            }
        }
    }

    public static MenuOption Menu(Database database)
    {
        ValidateParameters();

        var options = Enum.GetNames(typeof(MenuOption));

        if (database.Family!.Members.Count == 0)
        {
            // if there are no members, don't show the open option
            _ = options.Except(new[] { MenuOption.OpenMember }.Select(x => x.ToString()));
        }

        var selection = Questions.Menu(string.Empty, options.Select(name => Regex.Replace(name, "([a-z])([A-Z])", "$1 $2")).ToArray());

        return Enum.Parse<MenuOption>(selection.Text.Replace(" ", string.Empty));

        void ValidateParameters()
        {
            ArgumentNullException.ThrowIfNull(database, nameof(database));
            ArgumentNullException.ThrowIfNull(database.Family, nameof(database.Family));
        }
    }

    public enum SelectOption
    {
        LoadFamily,
        CreateNewFamily,
        InsertSampleFamilies,
        ShowReport,
        AssignSecretSanta
    }


    public static (SelectOption Option, Database? FamilyDatabase) SelectOne()
    {
        string[] families = Database
            .ListExistingFamilies()
            .Select(x => x.Name)
            .ToArray();

        if (families.Length == 0)
        {
            return (SelectOption.CreateNewFamily, null);
        }

        families = families
            .Union(Enum.GetNames(typeof(SelectOption))
            .Where(x => x != SelectOption.LoadFamily.ToString())
            .Select(x => Regex.Replace(x, "([a-z])([A-Z])", "$1 $2")))
            .ToArray();

        (int Index, string Text) selection = Questions
            .Menu("[blue]Choose a family to load or an option:[/]", families);

        var clean = selection.Text.Replace(" ", string.Empty);
        if (Enum.TryParse<SelectOption>(clean, true, out var option))
        {
            return (option, null);
        }
        else
        {
            return (SelectOption.LoadFamily, new(selection.Text));
        }
    }

    public static Database Create()
    {
        var name = Questions.AskString("[blue]What is the family's name?[/]", false, ValidateName)!;

        return new(name);

        static ValidationResult ValidateName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ValidationResult.Error("Name cannot be empty.");
            }
            else if (Database.FamilyExists(name))
            {
                return ValidationResult.Error("Family already exists.");
            }
            else
            {
                return ValidationResult.Success();
            }
        }
    }

    public static void Edit(Database database)
    {
        ValidateParameters();

        Rename(database);

        static void Rename(Database database)
        {
            var name = Questions.AskString("[blue]What is the family name?[/]", true, ValidateName);

            if (name is not null)
            {
                database.Rename(name);
            }

            ValidationResult ValidateName(string? name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return ValidationResult.Success();
                }
                else if (name != database.Family!.Name && Database.FamilyExists(name))
                {
                    return ValidationResult.Error("Family already exists.");
                }
                else
                {
                    return ValidationResult.Success();
                }
            }
        }

        void ValidateParameters()
        {
            ArgumentNullException.ThrowIfNull(database, nameof(database));
            ArgumentNullException.ThrowIfNull(database.Family, nameof(database.Family));
        }
    }

    public static void Delete(Database? database)
    {
        ArgumentNullException.ThrowIfNull(database, nameof(database));
        ArgumentNullException.ThrowIfNull(database.Family, nameof(database.Family));

        if (Questions.AskYesNo("[blue]Are you sure you want to delete the family?[/]") == Questions.YesNo.Yes)
        {
            database!.Delete();
        }
    }

    public static bool Exists(Family family)
    {
        return Database.ListExistingFamilies().Any(x => x.Name == family.Name);
    }

    internal static void Show((string Name, string FileName) family)
    {
        throw new NotImplementedException();
    }
}
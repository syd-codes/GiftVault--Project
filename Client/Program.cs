using Spectre.Console;

while (true)
{
    Breadcrumb.Draw();

    var (familyAction, family) = FamilyUI.SelectOne();

    if (familyAction == FamilyUI.SelectOption.ShowReport)
    {
        ShowAllFamilyReport();
        continue;
    }
    else if (familyAction == FamilyUI.SelectOption.AssignSecretSanta)
    {
        AssignSecretSanta();
    }
    else if (familyAction == FamilyUI.SelectOption.InsertSampleFamilies)
    {
        InsertSampleFamilies(true);
        continue;
    }
    else if (familyAction == FamilyUI.SelectOption.CreateNewFamily)
    {
        family = FamilyUI.Create();
    }

    PresentFamily(family);
}

static void AssignSecretSanta()
{
    // TODO: Implement this method
    AnsiConsole.MarkupLine("[yellow]This feature is not implemented yet.[/]");
}

static void ShowAllFamilyReport()
{
    (string Name, string FileName)[] everyFamily = Database.ListExistingFamilies().ToArray();

    if (everyFamily.Length == 0)
    {
        return;
    }

    /*
     In a tree format, show EVERY family and EVERY member.
     You get to decide how it looks. Just make it nice. 
     Reference: https://spectreconsole.net/widgets/tree
    */

    AnsiConsole.MarkupLine("[yellow]Your report goes here.[/]");

    Console.WriteLine("\r\nPress any key to continue...");
    Console.Read();
}

static void InsertSampleFamilies(bool clearFirst = false)
{
    /*
     This method is used to insert sample families into the database.
     Ensure there are least three families, and at least 5 members in each.
    */

    if (clearFirst)
    {
        var families = Database.ListExistingFamilies();
        // delete all families here
    }

    var family = new Database("Smith");
    family.Family!.Members.Add(new Member { Name = "John", Birthday = new DateOnly(1980, 1, 1) });
    family.Family!.Members.Add(new Member { Name = "Jane", Birthday = new DateOnly(1982, 2, 2) });
    family.Save();

    // add more families here
}

static void PresentFamily(Database? family)
{
    Breadcrumb.Forward(family!.Family!.Name);

    while (true)
    {
        Breadcrumb.Draw(true);
        FamilyUI.Show(family);
        var familyOption = FamilyUI.Menu(family);

        if (familyOption == FamilyUI.MenuOption.Rename)
        {
            FamilyUI.Edit(family);
            family.Save();
            continue;
        }
        else if (familyOption == FamilyUI.MenuOption.OpenMember)
        {
            var member = MemberUI.SelectOne(family);
            PresentMember(family, member);
            continue;
        }
        else if (familyOption == FamilyUI.MenuOption.AddMember)
        {
            var member = MemberUI.Create(family);
            family.Family!.Members.Add(member);
            family.Save();
            continue;
        }
        else if (familyOption == FamilyUI.MenuOption.Delete)
        {
            FamilyUI.Delete(family);
        }

        Breadcrumb.Back();
        return;
    }
}

static void PresentMember(Database family, Member member)
{
    Breadcrumb.Forward(member.Name);

    while (true)
    {
        Breadcrumb.Draw(true);
        MemberUI.Show(member);

        var selection = MemberUI.Menu(member);
        if (selection == MemberUI.MenuOption.Edit)
        {
            MemberUI.Edit(family, member);
            family.Save();
            continue;
        }
        else if (selection == MemberUI.MenuOption.Delete)
        {
            MemberUI.Delete(family, member);
            family.Save();
        }

        Breadcrumb.Back();
        return;
    }
}
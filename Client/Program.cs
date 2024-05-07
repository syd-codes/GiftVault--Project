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
    var families = Database
    .ListExistingFamilies()
    .Select(x => new Database(x.Name));

    foreach (var family in families)
    {
        foreach (var member in family.Family!.Members)
        {
            if (TryFetchRandomMember(member.GetUniqueName(family.Family.Name), families, out var assignee))
            {
                member.GiveToName = assignee!.Value.Name;
                member.GiveToGiftIdea = assignee!.Value.GiftIdea;
                family.Save();

                Console.WriteLine($"{family.Family.Name}/{member.Name} is assigned to {member.GiveToName}.");
            }
        }
    }
    bool TryFetchRandomMember(string uniqueName, IEnumerable<Database> families, out (string Name, string GiftIdea)? assignee)
    {
        assignee = families
            .SelectMany(x => x.Family!.Members.Select(y=> new { UniqueName = y.GetUniqueName(x.Family.Name), Member = y }))
            .Where(x => x.UniqueName != uniqueName)
            .Where(x => string.IsNullOrEmpty(x.Member.GiveToName)||x.Member.GiveToName != "-")
            .Where(x => !x.Member.AvoidMembers.Contains(uniqueName))
            .OrderBy(x => Guid.NewGuid())
            .Select(x => (x.Member.Name, x.Member.GiftIdea))
            .FirstOrDefault();

        return assignee is not null;
    }
}

static void ShowAllFamilyReport()
{
    (string Name, string FileName)[] everyFamily = Database.ListExistingFamilies().ToArray();

    if (everyFamily.Length == 0)
    {
        return;
    }

  //Create the tree
    var root = new Tree("[yellow]Families[/]");
    

// Add some nodes
    var foo = root.AddNode("[yellow]Members[/]");
    var table = foo.AddNode(new Table()
        .RoundedBorder()
        .AddColumn("")
        .AddColumn("")
        .AddRow()
        .AddRow()
        .AddRow());

    table.AddNode("[blue][/]");
    foo.AddNode("");

    AnsiConsole.Write(root);
    /*
     In a tree format, show EVERY family and EVERY member.
     You get to decide how it looks. Just make it nice. 
     Reference: https://spectreconsole.net/widgets/tree
    */

    AnsiConsole.MarkupLine("[yellow]Your report goes here.[/]");
    //Console.WriteLine("\r\nPress any key to continue...");
    Console.Read();
}

static void InsertSampleFamilies(bool clearFirst = false)
{
    

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

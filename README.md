# GiftVault
Gift and family member storage

## This is the beginning of a C# project.
- This project manages families by names and birthdays.
- Allowing the user to add, edit, and delete members to a family or create a family.
- It will eventually have a random Secret Santa function that will allow users to know who they are buying for and some gift ideas for them. 

## Creating the GiftVault project
```
dotnet new sln -n GiftVault
dotnet new gitignore
dotnet new editorconfig

dotnet new console -n Client -o Client
dotnet sln GiftVault.sln add Client/Client.csproj
dotnet add Client/Client.csproj package Spectre.Console

dotnet new classlib -n Shared -o Shared
dotnet sln GiftVault.sln add Shared/Shared.csproj
dotnet add Client/Client.csproj reference Shared/Shared.csproj

dotnet build
```
### Step-by-step explanation of each command and what it does:

1. `dotnet new sln -n GiftVault`: This command creates a new solution file named "CIS-230-SP24.sln". A solution is a container for multiple projects in .NET.

2. `dotnet new gitignore`: This command creates a .gitignore file, which is used to specify intentionally untracked files to ignore in Git.

3. `dotnet new editorconfig`: This command creates an .editorconfig file, which helps maintain consistent coding styles across different editors and IDEs.

4. `dotnet new console -n Client`: This command creates a new console application project named "Client". A console application is a program that runs in a command-line interface and interacts with the user through text input and output.

5. `dotnet new classlib -n Shared -o Shared`: This command creates a new class library project named "Shared" in a folder named "Shared". A class library is a reusable piece of code that can be referenced and used by other projects.

6. `dotnet add Client/Client.csproj package Spectre.Console`: This command adds the Spectre.Console package as a dependency to the Client project. Spectre.Console is a library that provides enhanced console output capabilities.

7. `dotnet add Client/Client.csproj reference Shared/Shared.csproj`: This command adds a project reference from the Client project to the Shared project. This allows the Client project to use the classes and functionality defined in the Shared project.

8. `dotnet sln GiftVault.sln add Client/Client.csproj`: This command adds the Client project to the solution file.

9. `dotnet sln GiftVault.sln add Shared/Shared.csproj`: This command adds the Shared project to the solution file.

10. `dotnet build`: This command builds the solution, compiling the source code files in each project and generating executable or library files as output. It checks for errors in the code and reports them if any are found.




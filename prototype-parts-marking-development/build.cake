using System.Linq;
using System.Threading;

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// VARIABLES
///////////////////////////////////////////////////////////////////////////////
var productVersion = string.Empty;

///////////////////////////////////////////////////////////////////////////////
// HELPERS
///////////////////////////////////////////////////////////////////////////////

private string GetCommitHash()
{
    var settings = new ProcessSettings
    {
        WorkingDirectory = "./",
        RedirectStandardOutput = true,
        Arguments = new ProcessArgumentBuilder()
            .Append("git")
            .Append("log")
            .Append("--pretty=format:'%h'")
            .Append("-n 1")
    };

    using(var process = StartAndReturnProcess("powershell", settings))
    {
        process.WaitForExit();
        if (process.GetExitCode() != 0)
        {
            throw new Exception();
        }

        var output = process.GetStandardOutput().ToList();
        if (output.Count != 1)
        {
            throw new Exception();
        }

        return output[0];
    }
}

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean-Environment")
    .Does(() =>
{
    Information("Cleaning bin directories.");
    CleanDirectories(GetDirectories($"./**/bin/{configuration}"));

    Information("Cleaning obj directories.");
    CleanDirectories(GetDirectories($"./**/obj/{configuration}"));

    Information("Cleaning NPM build directory.");
    CleanDirectory("./src/client-app/build");

    Information("Cleaning artifacts directory.");
    CleanDirectory("./artifacts");
});

Task("Prepare-Environment")
    .Does(() => 
{
    EnsureDirectoryExists("./artifacts");
    EnsureDirectoryExists("./artifacts/client-temp");
    EnsureDirectoryExists("./artifacts/api-temp");

    productVersion = $"1.0.0+{GetCommitHash()}";
    EnsureDirectoryExists($"./artifacts/PPMT-{productVersion}");
    EnsureDirectoryExists($"./artifacts/PPMT-{productVersion}/wwwroot");
});

Task("Build-Client")
    .Does(() =>
{
    var settings = new ProcessSettings
        {
            WorkingDirectory = "./src/client-app/",
            Arguments = new ProcessArgumentBuilder()
                .Append("npm")
                .Append("run")
                .Append("build"),
         };

    if (StartProcess("powershell", settings) != 0)
    {
        Error("Failed to build client app.");
        throw new InvalidOperationException();
    }

    CopyFiles("./src/client-app/build/**/*.*", "./artifacts/client-temp", preserveFolderStructure: true);
});

Task("Build-API")
    .Does(() =>
{
    var settings = new DotNetCorePublishSettings
    {
        Configuration = "Release",
        Runtime = "win-x64",
        PublishSingleFile = true,
        PublishReadyToRun = true,
        SelfContained = true,
        ArgumentCustomization = args => args.Append($"-p:Version={productVersion}"),
    };

    DotNetCorePublish("./src/WebApi", settings);

    CopyFiles("./src/WebApi/bin/Release/net6.0/win-x64/publish/*.*", "artifacts/api-temp");
});

Task("Assemble")
    .Does(() => 
{
    CopyFile("./LICENCE.md", $"./artifacts/PPMT-{productVersion}/LICENCE.md");
    CopyFiles("./artifacts/api-temp/*.*", $"./artifacts/PPMT-{productVersion}", preserveFolderStructure: true);
    CopyFiles("./artifacts/client-temp/**/*.*", $"./artifacts/PPMT-{productVersion}/wwwroot", preserveFolderStructure: true);
    DeleteDirectory("./artifacts/api-temp", new DeleteDirectorySettings { Recursive = true });
    DeleteDirectory("./artifacts/client-temp", new DeleteDirectorySettings { Recursive = true });

    // wait for directory operations to finish...
    Thread.Sleep(1000);
});

Task("Create-Delivery")
    .Does(() =>
{    
    Zip("./artifacts", "./artifacts/" + File($"PPMT.zip"));
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean-Environment")
    .IsDependentOn("Prepare-Environment")    
    .IsDependentOn("Build-Client")
    .IsDependentOn("Build-API")
    .IsDependentOn("Assemble")
    .IsDependentOn("Create-Delivery");

RunTarget(target);
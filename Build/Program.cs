﻿using Cake.Common;
using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Common.Tools.DotNet.Test;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Push;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public static class Settings
{
    public static string ModuleName = "Xemo";
    public static string Version = "0.1.0";
    public static string Configuration = "Release";
    public static string NugetReleaseToken = string.Empty;
    public static string NugetSource = "https://api.nuget.org/v3/index.json";

    public static FilePath SolutionPath = new($"../{ModuleName}.sln");
    public static DirectoryPath ModulePath = new("../src");
    public static DirectoryPath TestModulePath = new("../tests");
    public static DirectoryPath ArtifactPath = new("../artifacts");
}

public class BuildContext : FrostingContext
{
    public BuildContext(ICakeContext context) : base(context)
    {
        Delay = context.Arguments.HasArgument("delay");
    }

    public bool Delay { get; set; }
}

[TaskName("Version")]
public sealed class VersionTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.AppVeyor().Environment.Repository.Tag.IsTag)
        {
            Settings.Version = context.AppVeyor().Environment.Repository.Tag.Name;
            context.Log.Information($"Version is tagged, adopting '{Settings.Version}'");
        }
    }
}

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectories(Settings.ArtifactPath.FullPath);
        foreach (var module in context.GetSubDirectories(Settings.ModulePath))
            context.CleanDirectories(
                new List<string>
                {
                    $"{module}/bin",
                    $"{module}/obj"
                }
            );
    }
}

[TaskName("Restore")]
public sealed class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.NuGetRestore(Settings.SolutionPath);
    }
}

[TaskName("Build")]
[IsDependentOn(typeof(VersionTask))]
[IsDependentOn(typeof(CleanTask))]
[IsDependentOn(typeof(RestoreTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings =
            new DotNetBuildSettings
            {
                Configuration = Settings.Configuration,
                NoRestore = true,
                MSBuildSettings = new DotNetMSBuildSettings().SetVersionPrefix(Settings.Version)
            };

        foreach (var module in
                 context.GetSubDirectories(Settings.ModulePath)
                )
        {
            var name = module.GetDirectoryName();
            context.Log.Information($"Building {name}");
            context.DotNetBuild(
                module.FullPath,
                settings
            );
        }
    }
}

[TaskName("Test")]
[IsDependentOn(typeof(BuildTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings =
            new DotNetTestSettings
            {
                Configuration = Settings.Configuration,
                NoRestore = true
            };

        foreach (var test in context.GetSubDirectories(Settings.TestModulePath))
        {
            context.Log.Information($"Running tests of {test.GetDirectoryName()}");
            context.DotNetTest(
                test.FullPath,
                settings
            );
        }
    }
}

[TaskName("Nuget Build")]
[IsDependentOn(typeof(TestTask))]
public sealed class NugetBuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Log.Information($"Building NuGet Package for Version {Settings.Version}");

        var settings = new DotNetPackSettings
        {
            Configuration = Settings.Configuration,
            OutputDirectory = Settings.ArtifactPath,
            NoRestore = false,
            IncludeSymbols = true
        };
        settings.ArgumentCustomization =
            args => args.Append("--include-symbols").Append("-p:SymbolPackageFormat=snupkg");
        settings.MSBuildSettings =
            new DotNetMSBuildSettings()
                .SetVersionPrefix(Settings.Version);

        foreach (var module in context.GetSubDirectories(Settings.ModulePath))
        {
            var name = module.GetDirectoryName();
            Console.WriteLine($"Packing nuget {module}");
            context.DotNetPack(
                module.ToString(),
                settings
            );
        }
    }
}

[TaskName("Nuget Credentials")]
[IsDependentOn(typeof(NugetBuildTask))]
public sealed class NugetCredentialsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.AppVeyor().IsRunningOnAppVeyor)
        {
            Settings.NugetReleaseToken = context.EnvironmentVariable("NUGET_TOKEN");
            if (string.IsNullOrEmpty(Settings.NugetReleaseToken))
                throw new Exception("Environment variable 'NUGET_TOKEN' is not set");
        }
    }
}

[TaskName("Nuget Release")]
[IsDependentOn(typeof(NugetCredentialsTask))]
public sealed class NugetReleaseTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.AppVeyor().IsRunningOnAppVeyor
            &&
            context.AppVeyor().Environment.Repository.Tag.IsTag
           )
        {
            var nugets = context.GetFiles($"{Settings.ArtifactPath}/*.nupkg");
            foreach (var package in nugets)
                context.NuGetPush(
                    package,
                    new NuGetPushSettings
                    {
                        Source = Settings.NugetSource,
                        ApiKey = Settings.NugetReleaseToken
                    }
                );
        }
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(TestTask))]
[IsDependentOn(typeof(NugetBuildTask))]
[IsDependentOn(typeof(NugetCredentialsTask))]
[IsDependentOn(typeof(NugetReleaseTask))]
public sealed class DefaultTask : FrostingTask
{
}
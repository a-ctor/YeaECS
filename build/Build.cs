using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

  [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
  readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

  [Solution]
  readonly Solution Solution;

  [GitVersion]
  readonly GitVersion GitVersion;

  public AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

  Target Clean => _ => _
    .Before(Restore)
    .Executes(() =>
    {
      ArtifactsDirectory.CreateOrCleanDirectory();
    });

  Target Restore => _ => _
    .Executes(() =>
    {
      DotNetRestore(s => s
        .SetProjectFile(Solution));
    });

  Target Compile => _ => _
    .DependsOn(Restore)
    .Executes(() =>
    {
      DotNetBuild(s => s
        .SetProjectFile(Solution)
        .SetConfiguration(Configuration)
        .SetAssemblyVersion(GitVersion.AssemblySemVer)
        .SetFileVersion(GitVersion.AssemblySemFileVer)
        .SetInformationalVersion(GitVersion.InformationalVersion)
        .SetContinuousIntegrationBuild(IsServerBuild)
        .EnableNoRestore());
    });

  Target Test => _ => _
    .DependsOn(Compile)
    .Executes(() =>
    {
      DotNetTest(s => s
        .SetProjectFile(Solution)
        .SetConfiguration(Configuration)
        .EnableNoRestore()
        .EnableNoBuild());
    });

  Target Pack => _ => _
    .DependsOn(Test)
    .Executes(() =>
    {
      DotNetPack(s => s
        .SetProject(Solution)
        .SetConfiguration(Configuration)
        .SetVersion(GitVersion.NuGetVersionV2)
        .EnableNoBuild()
        .EnableNoRestore()
        .SetOutputDirectory(ArtifactsDirectory)
        .SetContinuousIntegrationBuild(IsServerBuild));
    });

  Target FullBuild => _ => _
    .DependsOn(Clean)
    .DependsOn(Compile)
    .DependsOn(Test)
    .DependsOn(Pack);
}

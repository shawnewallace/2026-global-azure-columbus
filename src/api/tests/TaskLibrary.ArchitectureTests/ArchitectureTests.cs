using System.Reflection;
using NetArchTest.Rules;
using TaskLibrary.Application;
using TaskLibrary.Domain;
using TaskLibrary.Infrastructure;

namespace TaskLibrary.ArchitectureTests;

public sealed class ArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(DomainReference).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ApplicationReference).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(InfrastructureReference).Assembly;
    private static readonly Assembly ApiAssembly = typeof(Program).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_Application()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot().HaveDependencyOn("TaskLibrary.Application")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain should not depend on Application");
    }

    [Fact]
    public void Domain_ShouldNotDependOn_Infrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot().HaveDependencyOn("TaskLibrary.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain should not depend on Infrastructure");
    }

    [Fact]
    public void Domain_ShouldNotDependOn_Api()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot().HaveDependencyOn("TaskLibrary.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain should not depend on Api");
    }

    [Fact]
    public void Application_ShouldNotDependOn_Infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot().HaveDependencyOn("TaskLibrary.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, "Application should not depend on Infrastructure");
    }

    [Fact]
    public void Application_ShouldNotDependOn_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot().HaveDependencyOn("TaskLibrary.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, "Application should not depend on Api");
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot().HaveDependencyOn("TaskLibrary.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, "Infrastructure should not depend on Api");
    }

    [Fact]
    public void Domain_ShouldNotReference_EntityFrameworkCore()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot().HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain should not reference EntityFrameworkCore");
    }

    [Fact]
    public void DomainClasses_ShouldBeSealed()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That().AreClasses()
            .And().AreNotAbstract()
            .Should().BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful, "All concrete domain classes should be sealed");
    }
}

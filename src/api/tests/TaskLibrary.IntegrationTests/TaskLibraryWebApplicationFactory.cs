using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TaskLibrary.IntegrationTests;

public sealed class TaskLibraryWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Provide a fallback connection string so Program.cs doesn't throw in CI
        // environments where POSTGRES_CONNECTION_STRING is not set.
        builder.UseSetting("ConnectionStrings:DefaultConnection", "Host=localhost;Database=test;Username=test;Password=test");
    }
}

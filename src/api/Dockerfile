FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy central build/package props first for layer caching
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["TaskLibrary.slnx", "."]

# Copy project files
COPY ["src/TaskLibrary.Api/TaskLibrary.Api.csproj", "src/TaskLibrary.Api/"]
COPY ["src/TaskLibrary.Application/TaskLibrary.Application.csproj", "src/TaskLibrary.Application/"]
COPY ["src/TaskLibrary.Domain/TaskLibrary.Domain.csproj", "src/TaskLibrary.Domain/"]
COPY ["src/TaskLibrary.Infrastructure/TaskLibrary.Infrastructure.csproj", "src/TaskLibrary.Infrastructure/"]

RUN dotnet restore "src/TaskLibrary.Api/TaskLibrary.Api.csproj"

COPY . .
RUN dotnet publish "src/TaskLibrary.Api/TaskLibrary.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Run as non-root
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "TaskLibrary.Api.dll"]

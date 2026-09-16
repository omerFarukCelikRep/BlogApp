FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /src
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BlogApp.Api/BlogApp.Api.csproj", "src/BlogApp.Api/"]
COPY ["src/BlogApp.Application/BlogApp.Application.csproj", "src/BlogApp.Application/"]
COPY ["src/BlogApp.Core/BlogApp.Core.csproj", "src/BlogApp.Core/"]
COPY ["src/BlogApp.Core.Caching/BlogApp.Core.Caching.csproj", "src/BlogApp.Core.Caching/"]
COPY ["src/BlogApp.Core.EFCore/BlogApp.Core.EFCore.csproj", "src/BlogApp.Core.EFCore/"]
COPY ["src/BlogApp.Core.Email/BlogApp.Core.Email.csproj", "src/BlogApp.Core.Email/"]
COPY ["src/BlogApp.Core.Logging/BlogApp.Core.Logging.csproj", "src/BlogApp.Core.Logging/"]
COPY ["src/BlogApp.Core.Mediator/BlogApp.Core.Mediator.csproj", "src/BlogApp.Core.Mediator/"]
COPY ["src/BlogApp.Core.Search/BlogApp.Core.Search.csproj", "src/BlogApp.Core.Search/"]
COPY ["src/BlogApp.Core.Security/BlogApp.Core.Security.csproj", "src/BlogApp.Core.Security/"]
COPY ["src/BlogApp.Core.Sms/BlogApp.Core.Sms.csproj", "src/BlogApp.Core.Sms/"]
COPY ["src/BlogApp.Core.Telemetry/BlogApp.Core.Telemetry.csproj", "src/BlogApp.Core.Telemetry/"]
COPY ["src/BlogApp.Core.Validations/BlogApp.Core.Validations.csproj", "src/BlogApp.Core.Validations/"]
COPY ["src/BlogApp.Domain/BlogApp.Domain.csproj", "src/BlogApp.Domain/"] 
COPY ["src/BlogApp.Infrastructure/BlogApp.Infrastructure.csproj", "src/BlogApp.Infrastructure/"]
COPY ["src/BlogApp.Infrastructure.Caching/BlogApp.Infrastructure.Caching.csproj", "src/BlogApp.Infrastructure.Caching/"]
COPY ["src/BlogApp.Infrastructure.EFCore/BlogApp.Infrastructure.EFCore.csproj", "src/BlogApp.Infrastructure.EFCore/"]
COPY ["src/BlogApp.Infrastructure.Email/BlogApp.Infrastructure.Email.csproj", "src/BlogApp.Infrastructure.Email/"]
COPY ["src/BlogApp.Infrastructure.Search/BlogApp.Infrastructure.Search.csproj", "src/BlogApp.Infrastructure.Search/"]
COPY ["src/BlogApp.Infrastructure.Security/BlogApp.Infrastructure.Security.csproj", "src/BlogApp.Infrastructure.Security/"]
COPY ["src/BlogApp.Infrastructure.Sms/BlogApp.Infrastructure.Sms.csproj", "src/BlogApp.Infrastructure.Sms/"]
COPY Directory.Packages.props .
RUN dotnet restore "src/BlogApp.Api/BlogApp.Api.csproj"
COPY . .
WORKDIR "/src/src/BlogApp.Api"
RUN dotnet build "./BlogApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlogApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /src
COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
    
ENTRYPOINT ["dotnet", "BlogApp.Api.dll"]

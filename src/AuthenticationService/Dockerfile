#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["IdentityServer4Demo/IdentityServer4Demo.csproj", "IdentityServer4Demo/"]
RUN dotnet restore "IdentityServer4Demo/IdentityServer4Demo.csproj"
COPY . .
WORKDIR "/src/IdentityServer4Demo"
RUN dotnet build "IdentityServer4Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityServer4Demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer4Demo.dll"]
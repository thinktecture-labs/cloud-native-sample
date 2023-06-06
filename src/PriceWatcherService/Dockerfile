FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
RUN adduser -u 1000 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./PriceWatcher.csproj", "./PriceWatcher.csproj"]
RUN dotnet restore "./PriceWatcher.csproj"
COPY . .
RUN dotnet publish "./PriceWatcher.csproj" \
    -c Release \
    --no-restore \
    -o ../app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "PriceWatcher.dll"]

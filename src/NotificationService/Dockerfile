ARG mode=debug

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

FROM base AS base-release
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM base AS base-debug
WORKDIR /remote_debugger

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
            unzip curl \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL https://aka.ms/getvsdbgsh --output install_vs_dbg.sh

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app /remote_debugger
USER appuser
RUN chmod +x install_vs_dbg.sh && ./install_vs_dbg.sh -v latest -l /remote_debugger

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NotificationService.csproj", "./"]
RUN dotnet restore "NotificationService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "NotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base-${mode} AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]

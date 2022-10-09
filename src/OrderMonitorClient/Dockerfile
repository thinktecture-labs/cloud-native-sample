
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 5005

ENV ASPNETCORE_URLS=http://+:5005

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["OrderMonitorClient.csproj", "./"]
RUN dotnet restore "OrderMonitorClient.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "OrderMonitorClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderMonitorClient.csproj" -c Release -o /app/publish /p:UseAppHost=false
ARG TARGET=k8s
RUN rm /app/publish/wwwroot/appsettings.json && \
    rm /app/publish/wwwroot/appsettings.json.*

RUN mv /app/publish/wwwroot/appsettings.$TARGET.json /app/publish/wwwroot/appsettings.json && \
    rm /app/publish/wwwroot/appsettings.$TARGET.json.*

FROM nginxinc/nginx-unprivileged:alpine
EXPOSE 8080
WORKDIR /usr/share/nginx/html
COPY --chown=nginx:nginx --from=publish /app/publish/wwwroot .

COPY nginx.conf /etc/nginx/conf.d/default.conf

# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/AggregatorNotifaction/AggregatorNotifaction.csproj", "src/AggregatorNotifaction/"]
COPY . .
WORKDIR "/src/src/AggregatorNotifaction"
RUN dotnet build "AggregatorNotifaction.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AggregatorNotifaction.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Dapr CLI and runtime
RUN apt-get update && \
    apt-get install -y wget ca-certificates && \
    wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | /bin/bash && \
    dapr init --slim

ENTRYPOINT ["dotnet", "AggregatorNotifaction.dll"]
#"dapr", "run", "--app-id", "aggregatornotification", "--app-port", "8080", "--dapr-http-port", "3502", "--resources-path", "/components", "--", 
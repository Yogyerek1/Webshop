FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["NexusCore.API.csproj", "./"]
RUN dotnet restore "NexusCore.API.csproj"

COPY . .
RUN dotnet publish "NexusCore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "NexusCore.API.dll"]
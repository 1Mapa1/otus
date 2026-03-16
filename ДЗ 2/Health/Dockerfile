FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Health/Health.csproj", "Health/"]
RUN dotnet restore "Health/Health.csproj"
COPY . .
WORKDIR /src/Health
RUN dotnet publish "Health.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8000
EXPOSE 8000

ENTRYPOINT ["dotnet", "Health.dll"]
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ExitCafe.slnx ./
COPY src/ExitCafe.Domain/*.csproj src/ExitCafe.Domain/
COPY src/ExitCafe.Application/*.csproj src/ExitCafe.Application/
COPY src/ExitCafe.Infrastructure/*.csproj src/ExitCafe.Infrastructure/
COPY src/ExitCafe.WebApi/*.csproj src/ExitCafe.WebApi/
RUN dotnet restore src/ExitCafe.WebApi/ExitCafe.WebApi.csproj

COPY src/ src/
RUN dotnet publish src/ExitCafe.WebApi/ExitCafe.WebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ExitCafe.WebApi.dll"]

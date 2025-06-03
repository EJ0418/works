FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY works/*.csproj ./works/
RUN dotnet restore "works/works.csproj"
COPY . .
RUN dotnet publish "works/works.csproj" -c Release -o /app/publish

FROM build as publish
RUN dotnet publish "works/works.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "works.dll"]
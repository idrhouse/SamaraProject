# Usar una imagen base de .NET
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Usar la imagen del SDK de .NET para la construcción del proyecto
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SamaraProject1.csproj", "./"]
RUN dotnet restore "SamaraProject1.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "SamaraProject1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SamaraProject1.csproj" -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SamaraProject1.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Iot.Class.Api/Iot.Class.Api.csproj", "Iot.Class.Api/"]
COPY ["Iot.Class.Infrastructure/Iot.Class.Infrastructure.csproj","Iot.Class.Infrastructure/"]
COPY ["Iot.Class.Data/Iot.Class.Data.csproj","Iot.Class.Data/"]
COPY ["Iot.Class.Domain/Iot.Class.Domain.csproj","Iot.Class.Domain/"]
RUN dotnet restore "Iot.Class.Api.csproj" --disable-parallel
COPY . .
WORKDIR "/src/Iot.Class.Api"
RUN dotnet build "Iot.Class.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Iot.Class.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Iot.Class.Api.dll"]

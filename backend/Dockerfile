# --- Estágio de build ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY EstacionamentoDIO.Domain/EstacionamentoDIO.Domain.csproj EstacionamentoDIO.Domain/
COPY EstacionamentoDIO.Application/EstacionamentoDIO.Application.csproj EstacionamentoDIO.Application/
COPY EstacionamentoDIO.Infrastructure/EstacionamentoDIO.Infrastructure.csproj EstacionamentoDIO.Infrastructure/
COPY EstacionamentoDIO.Api/EstacionamentoDIO.Api.csproj EstacionamentoDIO.Api/
RUN dotnet restore EstacionamentoDIO.Api/EstacionamentoDIO.Api.csproj

COPY EstacionamentoDIO.Domain/ EstacionamentoDIO.Domain/
COPY EstacionamentoDIO.Application/ EstacionamentoDIO.Application/
COPY EstacionamentoDIO.Infrastructure/ EstacionamentoDIO.Infrastructure/
COPY EstacionamentoDIO.Api/ EstacionamentoDIO.Api/

RUN dotnet publish EstacionamentoDIO.Api/EstacionamentoDIO.Api.csproj -c Release -o /app/publish --no-restore

# --- Estágio final ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN mkdir -p /app/data
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__Default="Data Source=/app/data/estacionamento.db"
EXPOSE 8080

ENTRYPOINT ["dotnet", "EstacionamentoDIO.Api.dll"]

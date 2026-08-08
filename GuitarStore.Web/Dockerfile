# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY GuitarStore.Web.csproj ./
RUN dotnet restore GuitarStore.Web.csproj

COPY . ./
RUN dotnet publish GuitarStore.Web.csproj -c Release -o /app/publish --no-restore

# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# App Runner routes to 8080 by default.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Don't run as root.
RUN useradd -m -u 1001 appuser
USER appuser

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "GuitarStore.Web.dll"]

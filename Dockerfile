FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /UrlShortenerApp

COPY . ./
RUN dotnet restore UrlShortener.WebApi/UrlShortener.WebApi.csproj
RUN dotnet publish UrlShortener.WebApi/UrlShortener.WebApi.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /UrlShortenerApp
COPY --from=build /out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "UrlShortener.WebApi.dll"]
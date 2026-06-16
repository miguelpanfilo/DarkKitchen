FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "DarkKitchenWebApi/DarkKitchenWebApi.csproj" -c Release -o /app/publish
RUN dotnet publish "Importers/CsvProductImporter/CsvProductImporter.csproj" -c Release -o /importers/csv
RUN dotnet publish "Importers/JsonProductImporter/JsonProductImporter.csproj" -c Release -o /importers/json
RUN dotnet publish "Importers/XmlProductImporter/XmlProductImporter.csproj" -c Release -o /importers/xml

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /importers/csv/CsvProductImporter.dll /plugins/
COPY --from=build /importers/csv/CsvHelper.dll /plugins/
COPY --from=build /importers/json/JsonProductImporter.dll /plugins/
COPY --from=build /importers/xml/XmlProductImporter.dll /plugins/
ENTRYPOINT ["dotnet", "DarkKitchenWebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/DocEditor.API/DocEditor.API.csproj backend/DocEditor.API/
RUN dotnet restore backend/DocEditor.API/DocEditor.API.csproj

COPY backend/DocEditor.API/ backend/DocEditor.API/
RUN dotnet publish backend/DocEditor.API/DocEditor.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT

EXPOSE 8080

ENTRYPOINT ["dotnet", "DocEditor.API.dll"]
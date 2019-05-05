FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY VirusScanner.MVC/VirusScanner.MVC.csproj VirusScanner.MVC/
RUN dotnet restore VirusScanner.MVC/VirusScanner.MVC.csproj
COPY . .
WORKDIR /src/VirusScanner.MVC
RUN dotnet build VirusScanner.MVC.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish VirusScanner.MVC.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "VirusScanner.MVC.dll"]

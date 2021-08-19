#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY OrderHistoryTextSearchService/*.csproj ./OrderHistoryTextSearchService/
COPY ServiceCommon/*.csproj ./ServiceCommon/
COPY OrderHistoryTextSearchService/NuGet.Config ./OrderHistoryTextSearchService/
COPY OrderHistoryService/NuGet.Config ./ServiceCommon/

RUN dotnet restore ./OrderHistoryTextSearchService/OrderHistoryTextSearchService.csproj

COPY OrderHistoryTextSearchService/. ./OrderHistoryTextSearchService/
COPY ServiceCommon/. ./ServiceCommon/

WORKDIR /src/OrderHistoryTextSearchService
RUN dotnet build "OrderHistoryTextSearchService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderHistoryTextSearchService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderHistoryTextSearchService.dll"]
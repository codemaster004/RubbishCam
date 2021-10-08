#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["HackathonE1.Api/HackathonE1.Api.csproj", "HackathonE1.Api/"]
COPY ["HackathonE1.Domain/HackathonE1.Domain.csproj", "HackathonE1.Domain/"]
RUN dotnet restore "HackathonE1.Api/HackathonE1.Api.csproj"
COPY . .
WORKDIR "/src/HackathonE1.Api"
RUN dotnet build "HackathonE1.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HackathonE1.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HackathonE1.Api.dll"]
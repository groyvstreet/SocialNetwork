#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT Development

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/services/IdentityService/IdentityService.PL/IdentityService.PL.csproj", "src/services/IdentityService/IdentityService.PL/"]
COPY ["src/services/IdentityService/IdentityService.DAL/IdentityService.DAL.csproj", "src/services/IdentityService/IdentityService.DAL/"]
RUN dotnet restore "src/services/IdentityService/IdentityService.PL/IdentityService.PL.csproj"
COPY . .
WORKDIR "/src/src/services/IdentityService/IdentityService.PL"
RUN dotnet build "IdentityService.PL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityService.PL.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.PL.dll"]
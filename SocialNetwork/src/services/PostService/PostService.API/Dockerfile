#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT Development

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/services/PostService/PostService.API/PostService.API.csproj", "src/services/PostService/PostService.API/"]
COPY ["src/services/PostService/PostService.Infrastructure/PostService.Infrastructure.csproj", "src/services/PostService/PostService.Infrastructure/"]
COPY ["src/services/PostService/PostService.Application/PostService.Application.csproj", "src/services/PostService/PostService.Application/"]
COPY ["src/services/PostService/PostService.Domain/PostService.Domain.csproj", "src/services/PostService/PostService.Domain/"]
RUN dotnet restore "src/services/PostService/PostService.API/PostService.API.csproj"
COPY . .
WORKDIR "/src/src/services/PostService/PostService.API"
RUN dotnet build "PostService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostService.API.dll"]
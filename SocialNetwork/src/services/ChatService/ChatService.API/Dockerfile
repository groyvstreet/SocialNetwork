#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT Development

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/services/ChatService/ChatService.API/ChatService.API.csproj", "src/services/ChatService/ChatService.API/"]
COPY ["src/services/ChatService/ChatService.Application/ChatService.Application.csproj", "src/services/ChatService/ChatService.Application/"]
COPY ["src/services/ChatService/ChatService.Domain/ChatService.Domain.csproj", "src/services/ChatService/ChatService.Domain/"]
COPY ["src/services/ChatService/ChatService.Infrastructure/ChatService.Infrastructure.csproj", "src/services/ChatService/ChatService.Infrastructure/"]
RUN dotnet restore "src/services/ChatService/ChatService.API/ChatService.API.csproj"
COPY . .
WORKDIR "/src/src/services/ChatService/ChatService.API"
RUN dotnet build "ChatService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatService.API.dll"]
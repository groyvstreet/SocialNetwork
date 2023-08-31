#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/services/PostService/PostService.Grpc/PostService.Grpc.csproj", "src/services/PostService/PostService.Grpc/"]
COPY ["src/services/PostService/PostService.Application/PostService.Application.csproj", "src/services/PostService/PostService.Application/"]
COPY ["src/services/PostService/PostService.Domain/PostService.Domain.csproj", "src/services/PostService/PostService.Domain/"]
COPY ["src/services/PostService/PostService.Infrastructure/PostService.Infrastructure.csproj", "src/services/PostService/PostService.Infrastructure/"]
RUN dotnet restore "src/services/PostService/PostService.Grpc/PostService.Grpc.csproj"
COPY . .
WORKDIR "/src/src/services/PostService/PostService.Grpc"
RUN dotnet build "PostService.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostService.Grpc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostService.Grpc.dll"]
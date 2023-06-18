#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MISA.PROCESS.API/MISA.PROCESS.API.csproj", "MISA.PROCESS.API/"]
COPY ["MISA.PROCESS.BLL/MISA.PROCESS.BLL.csproj", "MISA.PROCESS.BLL/"]
COPY ["MISA.PROCESS.CORE/MISA.PROCESS.COMMON.csproj", "MISA.PROCESS.CORE/"]
COPY ["MISA.PROCESS.INFRASTRUCTURE/MISA.PROCESS.DAL.csproj", "MISA.PROCESS.INFRASTRUCTURE/"]
RUN dotnet restore "MISA.PROCESS.API/MISA.PROCESS.API.csproj"
COPY . .
WORKDIR "/src/MISA.PROCESS.API"
RUN dotnet build "MISA.PROCESS.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MISA.PROCESS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MISA.PROCESS.API.dll"]
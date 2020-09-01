#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base

ADD ./nginx-shared/cert.crt /usr/local/share/ca-certificates/service2.crt
RUN chmod 644 /usr/local/share/ca-certificates/service2.crt && update-ca-certificates

WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["service2/service2.csproj", "service2/"]
RUN dotnet restore "service2/service2.csproj"
COPY . .
WORKDIR "/src/service2"
RUN dotnet build "service2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "service2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "service2.dll"]
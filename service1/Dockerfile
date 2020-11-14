# According to https://hub.docker.com/_/microsoft-dotnet-aspnet the aspnet:3.1 dockerfile is a debian image, this is important!
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
# Copy certificates into the correct location for Debian 
ADD ./certificates/ca/certs/ca.crt /usr/local/share/ca-certificates/ca.crt
ADD ./certificates/ca/intermediate/certs/intermediate.crt /usr/local/share/ca-certificates/intermediate.crt
# Set owner of the file to have read and write access, and other users on the system to have read access
RUN chmod 644 /usr/local/share/ca-certificates/ca.crt \
    && chmod 644 /usr/local/share/ca-certificates/intermediate.crt \
    # Update the Debain certificate store to trust these certs
    && update-ca-certificates

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS restore
WORKDIR src
# Copy our service project
COPY service1/*.csproj service1/
# Copy over any dependencies of the service project
COPY shared/*.csproj shared/
RUN dotnet restore "service1/service1.csproj"

FROM restore AS build
COPY . .
RUN dotnet build --no-restore "service1/service1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore "service1/service1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "service1.dll"]
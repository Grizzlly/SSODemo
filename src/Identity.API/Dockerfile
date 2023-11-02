#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
COPY . .
WORKDIR /src/Identity.API
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
#WORKDIR /src/Identity.API
##COPY ["Identity.API/Identity.API.csproj", "Identity.API/"]
##RUN dotnet restore "Identity.API/Identity.API.csproj"
##COPY . .
#RUN dotnet build "Identity.API.csproj" -c Release -o /app/build
#
FROM base AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Identity.API.dll"]
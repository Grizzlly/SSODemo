FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . .
WORKDIR /src/Identity.BlazorClient
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN apt-get update
RUN apt-get install -y python3
RUN dotnet workload install wasm-tools
RUN dotnet publish -c Release -o /app/output --self-contained --runtime browser-wasm

FROM nginx:alpine AS final
WORKDIR /var/www/identityclient
COPY --from=publish /app/output/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
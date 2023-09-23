FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ShippingCalculator.WebApi/ShippingCalculator.WebApi.csproj", "ShippingCalculator.WebApi/"]
RUN dotnet restore "ShippingCalculator.WebApi/ShippingCalculator.WebApi.csproj"
COPY . .
WORKDIR "/src/ShippingCalculator.WebApi"
RUN dotnet build "ShippingCalculator.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShippingCalculator.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShippingCalculator.WebApi.dll"]

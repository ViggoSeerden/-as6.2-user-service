FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UserService/UserService.csproj", "UserService/"]
COPY ["UserServiceBusiness/UserServiceBusiness.csproj", "UserServiceBusiness/"]
COPY ["UserServiceDAL/UserServiceDAL.csproj", "UserServiceDAL/"]
RUN dotnet restore "UserService/UserService.csproj"
COPY . .
WORKDIR "/src/UserService"
RUN dotnet build "UserService.csproj" -c Development -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.csproj" -c Development -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.dll"]

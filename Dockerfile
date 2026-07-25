FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY BibliotecaApp.API/BibliotecaApp.API.csproj ./BibliotecaApp.API/
COPY BibliotecaApp.Application/BibliotecaApp.Application.csproj ./BibliotecaApp.Application/
COPY BibliotecaApp.Domain/BibliotecaApp.Domain.csproj ./BibliotecaApp.Domain/
COPY BibliotecaApp.Infrastructure/BibliotecaApp.Infrastructure.csproj ./BibliotecaApp.Infrastructure/

RUN dotnet restore BibliotecaApp.API/BibliotecaApp.API.csproj

COPY . .

RUN dotnet publish BibliotecaApp.API/BibliotecaApp.API.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
CMD ["dotnet", "BibliotecaApp.API.dll"]
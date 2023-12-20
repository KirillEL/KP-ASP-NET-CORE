FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

COPY *.sln .
COPY dotnet-kp.csproj ./dotnet-kp/
WORKDIR /source/dotnet-kp
RUN dotnet restore

COPY ./ ./
COPY /Pages /app/Pages
WORKDIR /source/dotnet-kp
RUN dotnet add package Microsoft.EntityFrameworkCore.Analyzers --version 8.0.0
RUN dotnet add package Microsoft.EntityFrameworkCore.Design --version 7.0.11

ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet tool install --global dotnet-ef --version 7.0.11
RUN dotnet build


RUN dotnet publish -c release -o /app /source/dotnet-kp/dotnet-kp.csproj

# FROM mcr.microsoft.com/dotnet/aspnet:7.0
# WORKDIR /app

# COPY --from=build /app ./

EXPOSE 2222

CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:2222"]



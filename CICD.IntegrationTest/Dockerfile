FROM mcr.microsoft.com/dotnet/core/sdk:2.2
WORKDIR /integration

COPY CICD.IntegrationTest.csproj .
RUN dotnet restore 
RUN dotnet build

COPY . . 
ENTRYPOINT [ "dotnet", "test" ]

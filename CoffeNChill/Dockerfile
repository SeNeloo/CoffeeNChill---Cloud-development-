FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["CoffeNChill.csproj", "."]
RUN dotnet restore "CoffeNChill.csproj"

COPY . .

RUN dotnet publish "CoffeNChill.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore


FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0

WORKDIR /home/site/wwwroot

COPY --from=build /app/publish .

ENV AzureWebJobsStorage="DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFeqC4j2pZ6u8f2t8J3s9n8p6q5r4s3t2u1v0w;BlobEndpoint=http://coffeenchill-azurite:10000/devstoreaccount1;QueueEndpoint=http://coffeenchill-azurite:10001/devstoreaccount1;TableEndpoint=http://coffeenchill-azurite:10002/devstoreaccount1" \
    AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

EXPOSE 80
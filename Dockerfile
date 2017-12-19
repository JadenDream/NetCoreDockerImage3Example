FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY myApp/*.csproj ./myApp/
COPY myLibrary/*.csproj ./myLibrary/
COPY Librarys/*.dll ./Librarys/

WORKDIR /app/myApp
RUN dotnet restore

# copy everything else and build
WORKDIR /app

COPY myApp/ ./myApp/
COPY myLibrary/ ./myLibrary/

WORKDIR /app/myApp
RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/dotnet:runtime
WORKDIR /app/myApp
COPY --from=build-env /app/myApp/out ./
ENTRYPOINT ["dotnet", "myApp.dll"]

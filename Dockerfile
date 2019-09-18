FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build-env
COPY . ./
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/core/runtime:2.1
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "Beehive.dll"]
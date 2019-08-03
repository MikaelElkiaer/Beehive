FROM microsoft/dotnet:sdk AS build-env
COPY . ./
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/core/runtime:2.2.4
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "Beehive.dll"]
FROM microsoft/dotnet:2.2-aspnetcore-runtime

WORKDIR /app
ADD ingress-app/build .
ENTRYPOINT ["dotnet", "telemetry-ingress.dll"]
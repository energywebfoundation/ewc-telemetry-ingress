FROM microsoft/dotnet:2.2-runtime

WORKDIR /app
ADD build .
ENTRYPOINT ["dotnet", "telemetry-ingress.dll"]
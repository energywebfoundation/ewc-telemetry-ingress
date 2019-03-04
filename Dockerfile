FROM microsoft/dotnet:2.2-aspnetcore-sdk

WORKDIR /app
ADD ingress-app/build .
RUN apt update && apt install procps unzip -y
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /publish/vsdbg;
ENTRYPOINT ["dotnet", "telemetry-ingress.dll"]
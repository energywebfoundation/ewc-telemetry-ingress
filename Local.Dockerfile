FROM microsoft/dotnet:2.2-sdk AS builder

RUN mkdir -p /src
WORKDIR /src
ADD . .
RUN dotnet restore
RUN dotnet build
RUN dotnet test --no-build -v=normal tests --logger "trx;LogFileName=TestResults.trx" /p:CollectCoverage=true /p:Exclude="[xunit.*]*"
RUN dotnet publish -c Debug -o build

FROM microsoft/dotnet:2.2-aspnetcore-runtime

WORKDIR /app
COPY --from=builder /src/ingress-app/build .
RUN apt update && apt install procps unzip -y
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /publish/vsdbg;
ENTRYPOINT ["dotnet", "telemetry-ingress.dll"]
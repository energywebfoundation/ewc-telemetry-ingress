# Telemetry Ingress - Codename Apple Jack

Receives telemetry from telemetry-signer and big sister of Apple Bloom

## Key management

* Add key `dotnet run --keycmd add --validator <validatoraddress> --publickey <base64key>`
* Remove key `dotnet run --keycmd add --validator <validatoraddress>`

## Build

- Have dotnet SDK installed
- `dotnet build`

## Tests

- Have the build pass
- switch into the tests directory `cd tests`
- run the tests `dotnet test`

Remark: Some tests require a running influxdb at `http://influxdb:8086` with creds `root/root` and the db `telemetry` to be present.

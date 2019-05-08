# Telemetry Ingress - Codename Apple Jack

Receives telemetry from telemetry-signer and big sister of Apple Bloom

## Key management

These commands are used to register/deregister nodes/signers from the ingress. Only signers that are registered are able to send telemetry - others will be rejected. 

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

## Local docker build

To mimic the GitLab CI and to build a docker image locally run 

```
bash build-and-test-local.sh
```

Needs `docker` to be installed and accessible by the current user.

**CAUTION: Make sure you don't have a docker container with the name `ìnfluxdb` running**

## Configuration

Ingress can be configured using environment variables:

- `TELEMETRY_INTERNAL_DIR` - directory to hold certain data (registered nodes etc.)
- `TELEMETRY_KEYPASS` - Password to unlock the certificate
- `TELEMETRY_INFLUXDB_USER` - username to use when connecting to InfluxDB
- `TELEMETRY_INFLUXDB_USER_PASSWORD` - password to use when connecting to InfluxDB
- `TELEMETRY_INFLUXDB_DB` - database name to use when connecting to InfluxDB
- `TELEMETRY_INFLUXDB_HOST` - InfluxDB host to connect to

## Certificate

The SSL Certificate is only used for transport layer encryption and to authenticate the ingress against the signer. The signers will verify the cert fingerprint against the presented cert.

The certifictate is expected to be located at `$TELEMETRY_INTERNAL_DIR/telemetry-ingress.pfx`


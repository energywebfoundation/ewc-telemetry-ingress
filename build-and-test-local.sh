#!/bin/bash

echo "Create temporary build network..."
docker network create ingress_build

echo "Starting InfluxDB..."
docker run --network ingress_build --name influxdb -e INFLUXDB_DB=telemetry -d influxdb

sleep 2
echo "Wait for influx to come up..."
sleep 30

docker build -t ingress-local --network ingress_build -f Local.Dockerfile .

echo "Cleanup..."
docker stop influxdb
docker rm influxdb
docker network rm ingress_build

echo " ==> Image build as: ingress-local"
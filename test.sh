#!/usr/bin/env bash

set -e

KEEP_RUNNING=
USE_EXISTING_CONTAINERS=

while [ ! -z "$*" ] ; do
  case $1 in
    "--keep-running" )
      KEEP_RUNNING=yes
      ;;
    "--use-existing-containers" )
      USE_EXISTING_CONTAINERS=yes
      ;;
    "--help" )
      echo ./test.sh --keep-running --use-existing-containers
      exit 0
      ;;
    *)
      echo ./test.sh --keep-running --use-existing-containers
      exit 0
      ;;
  esac
  shift
done

if dotnet nuget list source | grep -q 'https://nuget.pkg.github.com/eventuate-tram/index.json'; then
  echo "Package already exists"
else
  echo "Add package"
  ## dotnet nuget add source https://nuget.pkg.github.com/eventuate-tram/index.json
fi

docker-compose build

if [ -z "$USE_EXISTING_CONTAINERS" ] ; then
  docker-compose down
fi

docker-compose up -d db

docker-compose run --rm wait-for-db

if [ -z "$USE_EXISTING_CONTAINERS" ] ; then
 docker-compose run --rm dbsetup
fi

docker-compose up -d zookeeper
docker-compose up -d kafka
docker-compose up -d cdc-service

# Wait for docker containers to start up
./wait-for-services.sh

#Run Customer-Service Tests
dotnet build CustomerService.UnitTests/CustomerService.UnitTests.csproj -c release
docker-compose run --rm customer-service-unittests

#Run Order-Service Tests
dotnet build OrderService.UnitTests/OrderService.UnitTests.csproj -c release
docker-compose run --rm order-service-unittests

docker-compose down

docker-compose up -d db

docker-compose run --rm wait-for-db

if [ -z "$USE_EXISTING_CONTAINERS" ] ; then
 docker-compose run --rm dbsetup
fi

docker-compose up -d zookeeper
docker-compose up -d kafka
docker-compose up -d cdc-service
docker-compose up -d mongo
docker-compose up -d elasticsearch

# Wait for docker containers to start up
./wait-for-services.sh

# Run Service
docker-compose up -d customer-service
docker-compose up -d order-service
docker-compose up -d order-history-service
docker-compose up -d order-history-text-search-service

#Run Tests
dotnet build EndToEndTests/EndToEndTests.csproj
dotnet test EndToEndTests/EndToEndTests.csproj
# Tear down test environment

if [ -z "$KEEP_RUNNING" ] ; then
  docker-compose down -v --remove-orphans
fi

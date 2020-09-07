#!/bin/bash

#build the Tram test database

echo -n waiting...

until /opt/mssql-tools/bin/sqlcmd -S $TRAM_DB_SERVER -U sa -P $TRAM_SA_PASSWORD -b -Q "SELECT 1" ; do
  echo -n .
  sleep 1
done

echo succeeded

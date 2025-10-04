#!/bin/bash
set -e

echo ">> Creating mongo-init.js"
envsubst < /docker-entrypoint-initdb.d/mongo-init.js.template > /docker-entrypoint-initdb.d/mongo-init.js

echo ">> Executing mongo-init.js"
/usr/bin/mongosh --username "$MONGO_INITDB_ROOT_USERNAME" --password "$MONGO_INITDB_ROOT_PASSWORD" --authenticationDatabase admin /docker-entrypoint-initdb.d/mongo-init.js

echo ">> Deleting mongo-init.js"
rm -f /docker-entrypoint-initdb.d/mongo-init.js
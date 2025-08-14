#!/bin/bash
set -e

# Modifica pg_hba.conf per permettere connessioni senza password da localhost
echo "host all all 0.0.0.0/0 trust" >> /var/lib/postgresql/data/pg_hba.conf

# Ricarica la configurazione di PostgreSQL
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    SELECT pg_reload_conf();
EOSQL
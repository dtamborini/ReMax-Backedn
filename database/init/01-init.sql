-- Script di inizializzazione database Remax Management
-- Questo script viene eseguito automaticamente quando il container PostgreSQL si avvia per la prima volta

-- Tutti i microservizi condividono lo stesso schema public
-- Grant permissions per l'utente remax_user
GRANT ALL PRIVILEGES ON DATABASE remax_management TO remax_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO remax_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO remax_user;
GRANT USAGE ON SCHEMA public TO remax_user;

-- Creazione di indici e funzioni comuni se necessario
-- (Verrà espanso quando avremo le entità)
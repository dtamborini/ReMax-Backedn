-- Script esteso semplificato per popolare un tenant con dati di test e relazioni
-- Esegui questo script specificando lo schema del tenant: SET search_path TO 'tenant_nomeschema';

-- ============================================================================
-- CLEAN UP (opzionale - per test)
-- ============================================================================
-- TRUNCATE TABLE "ResidentPremises" CASCADE;
-- TRUNCATE TABLE "SupplierBuildings" CASCADE;
-- TRUNCATE TABLE "PremisesBuildings" CASCADE;
-- TRUNCATE TABLE "Residents" CASCADE;
-- TRUNCATE TABLE "Suppliers" CASCADE;
-- TRUNCATE TABLE "Buildings" CASCADE;

-- ============================================================================
-- BUILDINGS (BuildingService)
-- ============================================================================

INSERT INTO "Buildings" (
    "Id", "Name", "Address", "Phone", "Pec", "Email", "FiscalCode", "VatCode",
    "CreatedAt", "CreatedBy"
) VALUES 
(
    '11111111-1111-1111-1111-111111111111',
    'Condominio Bella Vista Extended',
    'Via Roma 123, 20121 Milano MI',
    '+39 02 1234567',
    'bellavista.ext@pec.it',
    'admin@bellavista-ext.it',
    'BLLEXT85C15F205X',
    '11111111111',
    NOW(),
    'system'
),
(
    '22222222-2222-2222-2222-222222222222',
    'Residence Le Torri Extended',
    'Corso Venezia 45, 20121 Milano MI',
    '+39 02 2345678',
    'letorri.ext@pec.it',
    'info@letorri-ext.it',
    'LTREXT90H25F205Y',
    '22222222222',
    NOW(),
    'system'
),
(
    '33333333-3333-3333-3333-333333333333',
    'Palazzo Reale Extended',
    'Piazza del Duomo 1, 20122 Milano MI',
    '+39 02 3456789',
    'palazzo.ext@pec.it',
    'gestione@palazzo-ext.it',
    'PLZEXT95L30F205Z',
    '33333333333',
    NOW(),
    'system'
);

-- ============================================================================
-- SUPPLIERS (SupplierService) 
-- ============================================================================

INSERT INTO "Suppliers" (
    "Id", "CompanyName", "Name", "Surname", "Address", "Description", "Email", 
    "Phone", "VatCode", "AtecoCode", "State", "CreatedAt", "CreatedBy"
) VALUES 
(
    '44444444-4444-4444-4444-444444444444',
    'Edilservice Milano Extended SRL',
    'Marco',
    'Rossi',
    'Via Garibaldi 12, 20121 Milano MI',
    'Servizi di manutenzione e riparazione impianti condominiali',
    'info@edilservice-ext.it',
    '+39 02 4567890',
    '44444444444',
    '43.22.01',
    0, -- Approved
    NOW(),
    'system'
),
(
    '55555555-5555-5555-5555-555555555555',
    'Idraulica Lombarda Extended SNC',
    'Giuseppe',
    'Bianchi',
    'Via Torino 67, 20123 Milano MI',
    'Specialisti in impianti idraulici e sanitari',
    'contatti@idraulica-ext.it',
    '+39 02 5678901',
    '55555555555',
    '43.22.02',
    0, -- Approved
    NOW(),
    'system'
),
(
    '66666666-6666-6666-6666-666666666666',
    'Elettrica Nord Extended SRL',
    'Luigi',
    'Verdi',
    'Viale Monza 189, 20125 Milano MI',
    'Installazione e manutenzione impianti elettrici',
    'info@elettrica-ext.it',
    '+39 02 6789012',
    '66666666666',
    '43.21.01',
    2, -- PendingApproval
    NOW(),
    'system'
);

-- ============================================================================
-- SUPPLIER-BUILDING RELATIONS (SupplierService)
-- ============================================================================

INSERT INTO "SupplierBuildings" (
    "Id", "BuildingId", "SupplierId", "Favorite", "CreatedAt", "CreatedBy"
) VALUES
-- Edilservice Extended lavora per tutti e 3 i building (è il favorito del primo)
(
    '77777777-7777-7777-7777-777777777771',
    '11111111-1111-1111-1111-111111111111', -- Bella Vista
    '44444444-4444-4444-4444-444444444444', -- Edilservice
    true, -- Favorite
    NOW(),
    'system'
),
(
    '77777777-7777-7777-7777-777777777772',
    '22222222-2222-2222-2222-222222222222', -- Le Torri
    '44444444-4444-4444-4444-444444444444', -- Edilservice
    false,
    NOW(),
    'system'
),
(
    '77777777-7777-7777-7777-777777777773',
    '33333333-3333-3333-3333-333333333333', -- Palazzo Reale
    '44444444-4444-4444-4444-444444444444', -- Edilservice
    false,
    NOW(),
    'system'
),
-- Idraulica Extended lavora per 2 building
(
    '88888888-8888-8888-8888-888888888881',
    '11111111-1111-1111-1111-111111111111', -- Bella Vista
    '55555555-5555-5555-5555-555555555555', -- Idraulica
    false,
    NOW(),
    'system'
),
(
    '88888888-8888-8888-8888-888888888882',
    '22222222-2222-2222-2222-222222222222', -- Le Torri
    '55555555-5555-5555-5555-555555555555', -- Idraulica
    true, -- Favorite per questo building
    NOW(),
    'system'
),
-- Elettrica Extended è in attesa ma già assegnata a un building
(
    '99999999-9999-9999-9999-999999999991',
    '33333333-3333-3333-3333-333333333333', -- Palazzo Reale
    '66666666-6666-6666-6666-666666666666', -- Elettrica
    true, -- Favorite (anche se in pending)
    NOW(),
    'system'
);

-- ============================================================================
-- PREMISES BUILDINGS (BuildingService)
-- ============================================================================

INSERT INTO "PremisesBuildings" (
    "Id", "BuildingId", "FatherId", "Type", "Note", "TypeTags", "CreatedAt", "CreatedBy"
) VALUES
-- Building 1: Condominio Bella Vista - 3 appartamenti
(
    'aaaa1111-1111-1111-1111-111111111111',
    '11111111-1111-1111-1111-111111111111',
    NULL, -- Root level
    2, -- PropertyUnit
    'Appartamento piano terra - 80mq',
    '[2]'::jsonb, -- PropertyUnit
    NOW(),
    'system'
),
(
    'aaaa1111-1111-1111-1111-111111111112',
    '11111111-1111-1111-1111-111111111111',
    NULL,
    2, -- PropertyUnit
    'Appartamento primo piano - 85mq',
    '[2]'::jsonb,
    NOW(),
    'system'
),
(
    'aaaa1111-1111-1111-1111-111111111113',
    '11111111-1111-1111-1111-111111111111',
    NULL,
    2, -- PropertyUnit
    'Attico secondo piano - 120mq',
    '[2]'::jsonb,
    NOW(),
    'system'
),
-- Building 2: Residence Le Torri - 3 appartamenti
(
    'bbbb2222-2222-2222-2222-222222222221',
    '22222222-2222-2222-2222-222222222222',
    NULL,
    2, -- PropertyUnit
    'Bilocale Torre A - 55mq',
    '[2]'::jsonb,
    NOW(),
    'system'
),
(
    'bbbb2222-2222-2222-2222-222222222222',
    '22222222-2222-2222-2222-222222222222',
    NULL,
    2, -- PropertyUnit
    'Trilocale Torre B - 75mq',
    '[2]'::jsonb,
    NOW(),
    'system'
),
(
    'bbbb2222-2222-2222-2222-222222222223',
    '22222222-2222-2222-2222-222222222222',
    NULL,
    2, -- PropertyUnit
    'Quadrilocale Torre B - 95mq',
    '[2]'::jsonb,
    NOW(),
    'system'
),
-- Building 3: Palazzo Reale - 3 appartamenti di lusso
(
    'cccc3333-3333-3333-3333-333333333331',
    '33333333-3333-3333-3333-333333333333',
    NULL,
    2, -- PropertyUnit
    'Appartamento di rappresentanza - 150mq',
    '[2]'::jsonb,
    NOW(),
    'system'
),
(
    'cccc3333-3333-3333-3333-333333333332',
    '33333333-3333-3333-3333-333333333333',
    NULL,
    2, -- PropertyUnit
    'Suite presidenziale - 200mq',
    '[2]'::jsonb,
    NOW(),
    'system'
),
(
    'cccc3333-3333-3333-3333-333333333333',
    '33333333-3333-3333-3333-333333333333',
    NULL,
    2, -- PropertyUnit
    'Penthouse con terrazzo - 250mq',
    '[2]'::jsonb,
    NOW(),
    'system'
);

-- ============================================================================
-- RESIDENTS (UsersService)
-- ============================================================================

INSERT INTO "Residents" (
    "Id", "Name", "Surname", "Phone", "Email", "DelegateId", "DelegatorId",
    "CreatedAt", "CreatedBy"
) VALUES 
(
    'dddd4444-4444-4444-4444-444444444441',
    'Mario',
    'Condomini Extended',
    '+39 333 1234567',
    'mario.extended@email.it',
    NULL,
    NULL,
    NOW(),
    'system'
),
(
    'dddd4444-4444-4444-4444-444444444442',
    'Anna',
    'Palazzi Extended',
    '+39 334 2345678',
    'anna.extended@email.it',
    NULL,
    NULL,
    NOW(),
    'system'
),
(
    'dddd4444-4444-4444-4444-444444444443',
    'Franco',
    'Abitanti Extended',
    '+39 335 3456789',
    'franco.extended@email.it',
    NULL,
    NULL,
    NOW(),
    'system'
);

-- ============================================================================
-- RESIDENT-PREMISES RELATIONS (UsersService)
-- ============================================================================

INSERT INTO "ResidentPremises" (
    "Id", "ResidentId", "BuildingId", "PremisesBuildingId", "Percentage", 
    "CreatedAt", "CreatedBy"
) VALUES
-- Mario Extended: proprietario al 100% di un appartamento nel Condominio Bella Vista
(
    'eeee5555-5555-5555-5555-555555555551',
    'dddd4444-4444-4444-4444-444444444441', -- Mario
    '11111111-1111-1111-1111-111111111111', -- Bella Vista
    'aaaa1111-1111-1111-1111-111111111111', -- Appartamento piano terra
    100.0, -- 100% ownership
    NOW(),
    'system'
),
-- Anna Extended: proprietaria al 100% di un appartamento nel Residence Le Torri
(
    'eeee5555-5555-5555-5555-555555555552',
    'dddd4444-4444-4444-4444-444444444442', -- Anna
    '22222222-2222-2222-2222-222222222222', -- Le Torri
    'bbbb2222-2222-2222-2222-222222222221', -- Bilocale Torre A
    100.0,
    NOW(),
    'system'
),
-- Franco Extended: proprietario al 80% del Penthouse nel Palazzo Reale
(
    'eeee5555-5555-5555-5555-555555555553',
    'dddd4444-4444-4444-4444-444444444443', -- Franco
    '33333333-3333-3333-3333-333333333333', -- Palazzo Reale
    'cccc3333-3333-3333-3333-333333333333', -- Penthouse
    80.0, -- 80% ownership
    NOW(),
    'system'
),
-- Mario Extended: comproprietario al 50% dell'attico nel Bella Vista (condiviso)
(
    'eeee5555-5555-5555-5555-555555555554',
    'dddd4444-4444-4444-4444-444444444441', -- Mario
    '11111111-1111-1111-1111-111111111111', -- Bella Vista
    'aaaa1111-1111-1111-1111-111111111113', -- Attico
    50.0, -- 50% ownership
    NOW(),
    'system'
),
-- Anna Extended: comproprietaria al 50% dell'attico nel Bella Vista (condiviso con Mario)
(
    'eeee5555-5555-5555-5555-555555555555',
    'dddd4444-4444-4444-4444-444444444442', -- Anna
    '11111111-1111-1111-1111-111111111111', -- Bella Vista
    'aaaa1111-1111-1111-1111-111111111113', -- Attico
    50.0, -- 50% ownership  
    NOW(),
    'system'
);

-- ============================================================================
-- MESSAGGIO DI RIEPILOGO
-- ============================================================================

SELECT '=== SCRIPT ESTESO COMPLETATO CON SUCCESSO! ===' as message;
SELECT '' as spacing;
SELECT 'DATI INSERITI:' as summary;
SELECT '- 3 Buildings (Extended versions)' as buildings;
SELECT '- 3 Suppliers (2 approvati, 1 in attesa di approvazione)' as suppliers;
SELECT '- 6 Relazioni Supplier-Building' as supplier_relations;
SELECT '- 9 Premises (3 per ogni building)' as premises;
SELECT '- 3 Residents (Extended versions)' as residents;
SELECT '- 5 Relazioni Resident-Premises (inclusa proprietà condivisa)' as resident_relations;
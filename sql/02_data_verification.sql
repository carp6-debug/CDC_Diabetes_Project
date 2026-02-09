/* FILE: 02_data_verification.sql
   DESCRIPTION: Data Quality (DQ) Audit & Relational Integrity Verification.
   Execute this after running 'health_diabetes_ingestion.py'.
*/

-- 1. THE "GOLDEN COUNT" (Relational Integrity Check)
-- Success Criteria: Must return exactly 253,680 records.
SELECT COUNT(*) as total_integrated_records
FROM healthcare_analytics.fact_diabetes_survey f
JOIN healthcare_analytics.dim_demographics d ON f.demographic_id = d.demographic_id
JOIN healthcare_analytics.dim_lifestyle l ON f.lifestyle_id = l.lifestyle_id;

-- 2. ORPHANED RECORD CHECK
-- Ensures referential integrity; should return 0 results.
SELECT 'Orphaned Demographics' as issue, COUNT(*) 
FROM healthcare_analytics.fact_diabetes_survey f
LEFT JOIN healthcare_analytics.dim_demographics d ON f.demographic_id = d.demographic_id
WHERE d.demographic_id IS NULL
UNION ALL
SELECT 'Orphaned Lifestyle' as issue, COUNT(*) 
FROM healthcare_analytics.fact_diabetes_survey f
LEFT JOIN healthcare_analytics.dim_lifestyle l ON f.lifestyle_id = l.lifestyle_id
WHERE l.lifestyle_id IS NULL;

-- 3. SCHEMA CONSTRAINT AUDIT
-- Proves PK/FK enforcement for documentation/ERD verification.
SELECT 
    conname AS constraint_name, 
    relname AS table_name, 
    pg_get_constraintdef(c.oid) AS definition
FROM pg_constraint c
JOIN pg_class r ON c.conrelid = r.oid
WHERE r.relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'healthcare_analytics');
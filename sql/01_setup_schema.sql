/* PROJECT: CDC Healthcare Analytics Portal
   FILE: 01_setup_schema.sql
   
   DATA PROVENANCE:
   - Primary Source: CDC Behavioral Risk Factor Surveillance System (BRFSS)
   - Refined Dataset: 'Diabetes Health Indicators' (Cleaned by Alex Teboul)
   - Scale: 253,680 records across 21 indicators.
   
   DESCRIPTION:
   Establishes the Star Schema normalization. Converts flat-file indicators 
   into a Relational Model (Fact/Dimensions) to optimize query performance 
   for .NET 9 Service layers and Python Analytics.
*/

-- 1. ENVIRONMENT INITIALIZATION
DROP SCHEMA IF EXISTS healthcare_analytics CASCADE;
CREATE SCHEMA healthcare_analytics;
SET search_path TO healthcare_analytics, public;

-- 2. DIMENSION: Demographics
-- Handles socio-economic and age-based stratification
CREATE TABLE healthcare_analytics.dim_demographics (
    demographic_id SERIAL PRIMARY KEY,
    age_group SMALLINT,       -- 13-level scale (CDC standardized)
    sex SMALLINT,             -- 0: Female, 1: Male
    education_level SMALLINT, -- 1-6 scale
    income_level SMALLINT     -- 1-8 scale
);

-- 3. DIMENSION: Lifestyle Factors
-- Captures behavioral indicators mapped from the CDC survey
CREATE TABLE healthcare_analytics.dim_lifestyle (
    lifestyle_id SERIAL PRIMARY KEY,
    smoker BOOLEAN,
    hvy_alcohol_consump BOOLEAN,
    phys_activity BOOLEAN,
    fruits BOOLEAN,
    veggies BOOLEAN,
    any_healthcare BOOLEAN,
    no_docbc_cost BOOLEAN
);

-- 4. FACT TABLE: Health Metrics & Target
-- The central hub connecting clinical metrics to standardized dimensions
CREATE TABLE healthcare_analytics.fact_diabetes_survey (
    survey_id SERIAL PRIMARY KEY,
    diabetes_status INT,       -- 0: No, 1: Pre, 2: Yes
    high_bp BOOLEAN,
    high_chol BOOLEAN,
    bmi NUMERIC(4,1),
    gen_hlth INT,              -- 1-5 scale (Excellent to Poor)
    ment_hlth INT,             -- 0-30 days
    phys_hlth INT,             -- 0-30 days
    -- Foreign Key Relationships
    demographic_id INT REFERENCES healthcare_analytics.dim_demographics(demographic_id),
    lifestyle_id INT REFERENCES healthcare_analytics.dim_lifestyle(lifestyle_id)
);

-- 5. PERFORMANCE OPTIMIZATION
-- Indexes on Foreign Keys to ensure O(log n) join performance for large-scale analysis
CREATE INDEX idx_fact_demographic ON healthcare_analytics.fact_diabetes_survey(demographic_id);
CREATE INDEX idx_fact_lifestyle ON healthcare_analytics.fact_diabetes_survey(lifestyle_id);
/* FILE: 03_analytics_utilities.sql
   DESCRIPTION: Supporting queries for .NET Dashboard and Python Visualizations.
*/

-- 1. .NET RISK CALCULATOR VALIDATION
-- Direct verification of the GenHlth/Diabetes intersection used in the UI.
SELECT 
    gen_hlth, 
    diabetes_status, 
    COUNT(*) as record_count,
    ROUND(AVG(bmi), 2) as avg_bmi
FROM healthcare_analytics.fact_diabetes_survey
GROUP BY gen_hlth, diabetes_status
ORDER BY gen_hlth, diabetes_status;

-- 2. LIFESTYLE VS DIABETES (For Python Plot Verification)
SELECT 
    l.phys_activity, 
    f.diabetes_status, 
    COUNT(*) 
FROM healthcare_analytics.fact_diabetes_survey f
JOIN healthcare_analytics.dim_lifestyle l ON f.lifestyle_id = l.lifestyle_id
GROUP BY 1, 2;

-- 3. EMERGENCY TRUNCATE (Utility)
-- Use this if a data refresh is needed without destroying the schema structure.
-- TRUNCATE TABLE 
--    healthcare_analytics.fact_diabetes_survey, 
--    healthcare_analytics.dim_demographics, 
--    healthcare_analytics.dim_lifestyle 
-- RESTART IDENTITY CASCADE;
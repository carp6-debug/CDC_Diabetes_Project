"""
Project: CDC Healthcare Analytics
Module: ingestion.py
Description: Processes raw CDC BRFSS CSV data into a normalized Star Schema.
             Handles type casting for PostgreSQL BOOLEANs and maintains
             referential integrity across Dimensions and Fact tables.

             This script targets the PostgreSQL Star Schema created by 
             01_setup_schema.sql and is designed to be run after the 
             schema is set up and before any analytics are performed. 

             Author: Portfolio Project AI Advisor

"""

import pandas as pd
import logging
from sqlalchemy import create_engine, text
from sqlalchemy.exc import SQLAlchemyError

# Database Connection Configuration
# Replace 'your_password' with your actual PostgreSQL password
DB_URL = "postgresql://postgres:YOUR_DATABASE_PASSWORD@localhost:5432/cdc_diabetes_project"

# Logging configuration for progress tracking and debugging
logging.basicConfig(
    level=logging.INFO, 
    format='%(asctime)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

def get_engine():
    """Creates a SQLAlchemy engine."""
    return create_engine(DB_URL)

def ingest_data(file_path):
    """
    Main ingestion pipeline: 
    1. Reads CSV 
    2. Cleans/Casts data types
    3. Populates Dimensions (Commit 1)
    4. Populates Fact table (Commit 2)
    """
    try:
        engine = get_engine()
        
        # Load CSV - low_memory=False prevents DtypeWarnings on large datasets
        df = pd.read_csv(file_path)
        row_count = len(df)
        logger.info(f"Successfully read {row_count} rows from CSV.")

        # --- STEP 1: PREPARE DIMENSION DATA ---
        # Map CSV columns to our SQL schema names
        demo_df = df[['Age', 'Sex', 'Education', 'Income']].copy()
        demo_df.columns = ['age_group', 'sex', 'education_level', 'income_level']
        
        # Prepare Lifestyle data with explicit Boolean casting for PostgreSQL compatibility
        lifestyle_cols = ['Smoker', 'HvyAlcoholConsump', 'PhysActivity', 'Fruits', 'Veggies', 'AnyHealthcare', 'NoDocbcCost']
        life_df = df[lifestyle_cols].copy()

        for col in life_df.columns:
            life_df[col] = life_df[col].astype(bool)

        # Match the 7 input columns with 7 output names
        life_df.columns = [
            'smoker', 
            'hvy_alcohol_consump', 
            'phys_activity', 
            'fruits', 
            'veggies', 
            'any_healthcare', 
            'no_docbc_cost'
        ]
        # --- STEP 2: PREPARE FACT DATA ---
        # Mapping remaining metrics and creating surrogate keys to match Dimensions
        fact_df = pd.DataFrame({
            'diabetes_status': df['Diabetes_binary'].astype(int),
            'high_bp': df['HighBP'].astype(bool),
            'high_chol': df['HighChol'].astype(bool),
            'bmi': df['BMI'],
            'gen_hlth': df['GenHlth'].astype(int),
            'ment_hlth': df['MentHlth'].astype(int),
            'phys_hlth': df['PhysHlth'].astype(int),
            # Creating manual IDs (1 to N) to link to our freshly truncated tables
            'demographic_id': range(1, row_count + 1),
            'lifestyle_id': range(1, row_count + 1)
        })

        # --- STEP 3: DATABASE EXECUTION ---
        # We use two separate 'with' blocks to ensure Dimensions are committed 
        # BEFORE the Fact table is inserted. This prevents ForeignKeyViolations.
        
        # Commit Dimensions
        with engine.begin() as conn:
            logger.info("Truncating tables and loading Dimensions...")
            conn.execute(text("TRUNCATE TABLE healthcare_analytics.fact_diabetes_survey, healthcare_analytics.dim_demographics, healthcare_analytics.dim_lifestyle RESTART IDENTITY CASCADE;"))
            
            demo_df.to_sql('dim_demographics', conn, schema='healthcare_analytics', if_exists='append', index=False, chunksize=10000)
            life_df.to_sql('dim_lifestyle', conn, schema='healthcare_analytics', if_exists='append', index=False, chunksize=10000)
            logger.info("Dimensions successfully committed.")

        # Commit Fact Table
        with engine.begin() as conn:
            logger.info("Loading Fact table...")
            fact_df.to_sql('fact_diabetes_survey', conn, schema='healthcare_analytics', if_exists='append', index=False, chunksize=10000)
            logger.info("Fact table successfully committed.")

        # FINAL SANITY CHECK
        verify_load(engine)

    except FileNotFoundError:
        logger.error(f"File not found: {file_path}. Check your path syntax (use r'' for Windows).")
    except SQLAlchemyError as e:
        logger.error(f"Database error during ingestion: {e}")
    except Exception as e:
        logger.error(f"Unexpected error: {e}")

def verify_load(engine):
    """Query the database to confirm row counts match the source file."""
    with engine.connect() as conn:
        for table in ['dim_demographics', 'dim_lifestyle', 'fact_diabetes_survey']:
            count = conn.execute(text(f"SELECT COUNT(*) FROM healthcare_analytics.{table}")).scalar()
            logger.info(f"Verification: {table} has {count} rows.")

if __name__ == "__main__":
    # Use the Raw string prefix 'r' to handle Windows backslashes correctly
    target_file = r'D:\OneDrive\dev\projects\CDC_Diabetes_Project\data\diabetes_binary_health_indicators_BRFSS2015.csv'
    ingest_data(target_file)
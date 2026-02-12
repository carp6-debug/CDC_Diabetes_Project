namespace CDC_Diabetes.Data.Services;

using CDC_Diabetes.Models;

/// <summary>
/// MODULE: HealthcareDataEngine (Service Layer)
/// ROLE: Programmer Analyst / Data Engineer
/// 
/// INTENT:
/// This class serves as the core logic engine for the CDC Healthcare Analytics Portal.
/// It acts as the "Tier 2" broker between the PostgreSQL Data Warehouse (Tier 3) 
/// and the Blazor Presentation Layer (Tier 1).
///
/// DESIGN PATTERNS:
/// - Dependency Injection: Decouples database context from UI logic.
/// - Repository Pattern: Provides a structured interface for querying Fact/Dimension tables.
/// - Data Normalization: Translates raw relational data into clinical risk models.
///
/// RESPONSIBILITIES:
/// 1. Execute high-performance LINQ queries against the 253k record Star Schema.
/// 2. Calculate patient-specific risk percentiles based on clinical indicators.
/// 3. Abstract complex SQL joins into simplified DTOs for UI rendering.
/// </summary>
public class HealthRiskService : IHealthRiskService
{
    public int CalculateRiskScore(FactDiabetesSurvey survey, DimLifestyle lifestyle)
    {
        int score = 0;

        // 1. BMI Factor (Heavily weighted)
        if (survey.Bmi >= 30) score += 30; // Obese
        else if (survey.Bmi >= 25) score += 15; // Overweight

        // 2. Clinical Factors
        if (survey.HighBp) score += 25;
        if (survey.HighChol) score += 15;

        // 3. Lifestyle Factors
        if (lifestyle.Smoker) score += 10;
        if (!lifestyle.PhysActivity) score += 15;
        if (lifestyle.HvyApples) score -= 5; // Protective factor example

        // Clamp the score between 0 and 100
        return Math.Clamp(score, 0, 100);
    }

    public string GetRiskLevel(int score)
    {
        return score switch
        {
            >= 70 => "High Risk",
            >= 40 => "Moderate Risk",
            _ => "Low Risk"
        };
    }
}
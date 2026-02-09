namespace CDC_Diabetes.Data.Services;

using CDC_Diabetes.Models;

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
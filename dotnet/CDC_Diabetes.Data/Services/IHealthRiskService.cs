namespace CDC_Diabetes.Data.Services;

using CDC_Diabetes.Models;

public interface IHealthRiskService
{
    // Calculates a score from 0-100 based on CDC survey correlations
    int CalculateRiskScore(FactDiabetesSurvey survey, DimLifestyle lifestyle);
    
    // Returns a string description of the risk level (Low, Moderate, High)
    string GetRiskLevel(int score);
}
namespace CDC_Diabetes.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("fact_diabetes_survey", Schema = "healthcare_analytics")]
public class FactDiabetesSurvey
{
    [Key]
    [Column("survey_id")]
    public int SurveyId { get; set; }

    [Column("diabetes_status")]
    public int DiabetesStatus { get; set; }

    [Column("high_bp")]
    public bool HighBp { get; set; }

    [Column("high_chol")]
    public bool HighChol { get; set; }

    [Column("bmi")]
    public decimal Bmi { get; set; }

    [Column("gen_hlth")]
    public int GenHlth { get; set; }

    [Column("ment_hlth")]
    public int MentHlth { get; set; }

    [Column("phys_hlth")]
    public int PhysHlth { get; set; }

    [Column("demographic_id")]
    public int DemographicId { get; set; }

    [Column("lifestyle_id")]
    public int LifestyleId { get; set; }

    // Navigation Properties for EF Core Joins
    [ForeignKey("DemographicId")]
    public virtual DimDemographics Demographics { get; set; } = null!;

    [ForeignKey("LifestyleId")]
    public virtual DimLifestyle Lifestyle { get; set; } = null!;
}
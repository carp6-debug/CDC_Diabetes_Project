namespace CDC_Diabetes.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("dim_demographics", Schema = "healthcare_analytics")]
public class DimDemographics
{
    [Key]
    [Column("demographic_id")]
    public int DemographicId { get; set; }

    [Column("age_group")]
    public short AgeGroup { get; set; }

    [Column("sex")]
    public short Sex { get; set; }

    [Column("education_level")]
    public short EducationLevel { get; set; }

    [Column("income_level")]
    public short IncomeLevel { get; set; }
}
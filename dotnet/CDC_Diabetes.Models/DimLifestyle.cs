namespace CDC_Diabetes.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("dim_lifestyle", Schema = "healthcare_analytics")]
public class DimLifestyle
{
    [Key]
    [Column("lifestyle_id")]
    public int LifestyleId { get; set; }

    [Column("smoker")]
    public bool Smoker { get; set; }

    [Column("hvy_alcohol_consump")]
    public bool HvyAlcoholConsump { get; set; }

    [Column("phys_activity")]
    public bool PhysActivity { get; set; }

    [Column("hvy_apples")]
    public bool HvyApples { get; set; }

    [Column("fruits")]
    public bool Fruits { get; set; }

    [Column("veggies")]
    public bool Veggies { get; set; }

    [Column("any_healthcare")]
    public bool AnyHealthcare { get; set; }

    [Column("no_docbc_cost")]
    public bool NoDocbcCost { get; set; }
}
using System.ComponentModel.DataAnnotations;

public enum AutomathType
{
    [Display(Name = "Детерминированный конечный автомат")]
    DKA = 0,

    [Display(Name = "Недетерминированный конечный автомат")]
    NKA = 1,

    [Display(Name = "Недетерминированный конечный автомат с ε-переходами")]
    NKAe = 2
}

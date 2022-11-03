namespace Cards.Web.Models;

public class Player
{
    public string Name { get; set; } = string.Empty;
    public int Estimate { get; set; }
    public bool EstimateGiven { get; set; }
    public Guid Id { get; set; }
}
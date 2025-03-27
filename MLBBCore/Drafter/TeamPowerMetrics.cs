namespace Evoloor.MLBBCore.Drafter;

public abstract class TeamPowerMetrics : PowerMetrics, IComparable<TeamPowerMetrics>
{
    public int CompareTo(TeamPowerMetrics? other)
    {
        throw new NotImplementedException();
    }
    public int PhisDefLikeliness { get; init; }
    public int MagicDefLikeliness { get; init; }
    public int HeroesDominateLate { get; init; }
    public int TotalDamageAmount { get; init; }
    public int AssasinDamageAmount { get; init; }
    public int AOECCAmount { get; init; }
    public bool HasTraveller { get; init; }
    public int SquishiesAmount { get; init; }
    public int TanksAmount { get; init; }
    public int InitiatorsAmount { get; init; }
}

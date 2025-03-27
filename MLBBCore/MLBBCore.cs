using Evoloor.MOBACore;
namespace Evoloor.MLBBCore;
public abstract class Hero(string name) : IStaticData
{
    public string Name { get; init; } = name;
    protected string? _fileName;
    public string? FileName { protected get => _fileName; set { if (value is null) return; FileRelativePath = Path.Combine(@"\heroheadsnamed\", value); _fileName = value; } }
    public string? FileRelativePath { get; protected set; }
}
public class HeroSimpleGraded(string name, HeroAttributes attributes) : Hero(name)
{
    public HeroAttributes Attributes { get; init; } = attributes;
}
public class HeroBuildlyGraded(
    string name,
    Dictionary<Build, HeroAttributes> heroGrades
    )
    : Hero(name)
{
    public Dictionary<Build, HeroAttributes> HeroGrades { get; } = heroGrades;
    public HeroSimpleGraded? GetBuildGrades(Build build) => HeroGrades.ContainsKey(build) ? new(Name, HeroGrades[build]) : null;
}
public class PowerMetrics
{
    public static readonly int METRICS_CEILING = 3;
    public static readonly int METRICS_STEP = 1;
    public int? TurretDamage { get; set; }
    public int? LaneClear { get; set; }
    public int? CCAmount { get; set; }
    public int? ReachingFarEvaluation { get; set; }
    public int? UniqueAbilityPoints { get; set; }
    public int? MetaPoints { get; set; }
    public static int MetricsAmount { get; } = 6;
}
public class HeroAttributes : PowerMetrics
{
    public static new int MetricsAmount { get; } = 6 + PowerMetrics.MetricsAmount;
    public CCType? CCType { get; set; }
    public FightRange? FightRange { get; set; }
    public GameStage? DominatesStage { get; set; }
    public DamageAmount? DamageAmount { get; set; }
    public DamageTarget? DamageTarget { get; set; }
    public DamageBlockable? DamageBlockable { get; set; }
    public HpAmount? HpAmount { get; set; }
    public MoveSpeed? MoveSpeed { get; set; }
    public IEnumerable<HeroTrait> Traits { get; set; } = new HashSet<HeroTrait>();
    //public int CounterPointsAgainst(HeroAttributes enemy)
    //    => HeroInteractions.GetConterPoints(this, enemy)
    //        + HeroInteractions.CountInteractionsBasedOnTraits(this, enemy, CountersRules)
    //        + HeroInteractions.CountInteractionsBasedOnTraits(enemy, this, CounteredByRules);
    public int SynergyPointsWith(HeroAttributes teammate)
        => HeroInteractions.GetSynergyPoints(this, teammate);
    public int AntiSynergyPointsWith(HeroAttributes teammate)
        => HeroInteractions.GetAntiSynergyPoints(this, teammate);
    public static event Action<HeroTrait, string>? OnTraitSuggested;
}
public enum GameStage
{
    Early,
    Equal,
    Late
}
public enum CCType
{
    AOE,
    Instant,
    None
}
public enum DamageBlockable
{
    Physical,
    Magical,
    None
}
public enum DamageAmount
{
    Low_10 = 0,
    Normal_11_17 = 1,
    High_18_24 = 2,
    Carry_25_40 = 3
}
public enum DamageTarget
{
    AOE,
    Flex,
    Solo
}
public enum Build
{
    Durubility,
    Damage
}
public enum HpAmount
{
    Tanky,
    Squishy,
    Balanced
}
public enum MoveSpeed
{
    Fast,
    Immobile
}
public enum FightRange
{
    Hugging,
    Close,
    Distaned
}

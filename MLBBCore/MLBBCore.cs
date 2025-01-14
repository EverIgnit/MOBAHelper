using Evoloor.MOBACore;
namespace Evoloor.MLBBCore;

public abstract class Hero(string name, HeroTrait[] traits) : IStaticData
{
    public string Name { get; init; } = name;
    public HeroTrait[] Traits { get; init; } = traits;
}
public class HeroStableGraded : Hero
{
    public HeroStableGraded(string name, HeroTrait[] traits/*TODO: , HeroGrades grades*/)
        : base(name, traits)
    {
        Grades?.CheckForTraitSuggestions(traits);
    }
    public HeroGrades Grades { get; init; }
}
public class HeroDymanicGraded : Hero
{
    public HeroDymanicGraded(
        string name,
        HeroTrait[] traits,
        Dictionary<Build, HeroGrades> heroGrades
        ) : base(name, traits)
    {
        HeroGrades = heroGrades;
        foreach (var grades in heroGrades.Values)
            grades?.CheckForTraitSuggestions(traits);
    }
    public Dictionary<Build, HeroGrades> HeroGrades { get; }
}
public class HeroGrades
{
    protected HeroGrades() { }
    public int PhisDamage { get; init; }
    public int MagicDamage { get; init; }
    public int PureDamage { get; init; }
    public int TurretDamage { get; init; }
    public int LaneClear { get; init; }
    public int CC { get; init; }
    public int Early { get; init; }
    public int Late { get; init; }
    public int ReinforcementRoam { get; init; }
    public int Backline { get; init; }
    public int Durability { get; init; }
    public int UniqueAbility { get; init; }
    public HeroGrades
    (
        int phisDamage,
        int magicDamage,
        int pureDamage,
        int turretDamage,
        int laneClear,
        int CC,
        int early,
        int late,
        int reinforcementRoam,
        int backline,
        int durability,
        int uniqueAbility)
    {
        PhisDamage = phisDamage;
        MagicDamage = magicDamage;
        PureDamage = pureDamage;
        TurretDamage = turretDamage;
        LaneClear = laneClear;
        this.CC = CC;
        Early = early;
        Late = late;
        ReinforcementRoam = reinforcementRoam;
        Backline = backline;
        Durability = durability;
        UniqueAbility = uniqueAbility;
    }
    public int TotalDamage => PhisDamage + MagicDamage + PureDamage;
    public static event Action<HeroTrait, string>? OnTraitSuggested;
    public void CheckForTraitSuggestions(HeroTrait[] traits)
    {
        if (PureDamage > 0 && !traits.Contains(HeroTrait.TankShredder))
            OnTraitSuggested?.Invoke(HeroTrait.TankShredder, "Есть чистый урон");
        if (TotalDamage >= 3 &&
            !(traits.Contains(HeroTrait.AOEBurst) || traits.Contains(HeroTrait.SoloDmg)))
            OnTraitSuggested?.Invoke(HeroTrait.AOEBurst, "Урона много, но не указано, какого");
        if (Durability >= 3 &&
            !(traits.Contains(HeroTrait.HighDurability) || traits.Contains(HeroTrait.TeamHeal)))
            OnTraitSuggested?.Invoke(HeroTrait.HighDurability, "Живучести много, но не указано, какой");

    }
}
class HeroExperimental
{
    public int Damage { get; set; }
    public DamageType DamageType { get; set; }
}
enum DamageType
{
    AOE,
    DPS,
    Solo
}
public enum Build
{
    Durubility,
    Damage
}
public enum HeroTrait
{
    Mobility,
    //Traveller,
    //MostlyEarly,
    //MostlyLate,
    //LaneClear,
    GatherCC,
    SpreadCC,
    AOEBurst,
    LowBaseDurability,
    HighDurability,
    TeamHeal,
    TeamSpeed,
    LowMobility,
    InstantCC,
    LowDmg,
    SoloDmg,
    TankShredder
}
public static class HeroesComps
{
    public static (HeroTrait trait, HeroTrait teammateTrait)[] Antinergies =
    [
        (HeroTrait.AOEBurst, HeroTrait.SpreadCC),
        (HeroTrait.LowDmg, HeroTrait.TeamHeal),
    ];
    public static (HeroTrait trait, HeroTrait teammateTrait)[] Synergies =
    [
        (HeroTrait.AOEBurst, HeroTrait.GatherCC),
        (HeroTrait.TeamHeal, HeroTrait.LowBaseDurability),
        (HeroTrait.LowMobility, HeroTrait.TeamSpeed),
        (HeroTrait.GatherCC, HeroTrait.GatherCC),
        (HeroTrait.InstantCC, HeroTrait.GatherCC),
    ];
    public static (HeroTrait strongTrait, HeroTrait weakEnemyTrait)[] Domination =
    [
        (HeroTrait.Mobility, HeroTrait.GatherCC),
        (HeroTrait.Mobility, HeroTrait.AOEBurst),
        (HeroTrait.InstantCC, HeroTrait.LowBaseDurability),
        (HeroTrait.AOEBurst, HeroTrait.LowBaseDurability),
        (HeroTrait.TeamSpeed, HeroTrait.LowMobility),
        (HeroTrait.TeamHeal, HeroTrait.AOEBurst),
        (HeroTrait.SoloDmg, HeroTrait.LowBaseDurability),
        (HeroTrait.HighDurability, HeroTrait.SoloDmg),
        (HeroTrait.AOEBurst, HeroTrait.LowMobility),
        (HeroTrait.GatherCC, HeroTrait.LowMobility),
        (HeroTrait.TankShredder, HeroTrait.HighDurability),
    ];
}

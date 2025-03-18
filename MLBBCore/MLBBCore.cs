using Evoloor.MOBACore;
namespace Evoloor.MLBBCore;

public abstract class Hero(string name, HeroTrait[] traits) : IStaticData
{
    public string Name { get; init; } = name;
}
public class HeroStableGraded : Hero
{
    public HeroStableGraded(string name, HeroTrait[] traits/*TODO: , HeroGrades grades*/)
        : base(name, traits)
    {
        Grades?.CheckForTraitSuggestions(traits);
    }
    public HeroAttributes Grades { get; init; }
}
public class HeroDymanicGraded : Hero
{
    public HeroDymanicGraded(
        string name,
        HeroTrait[] traits,
        Dictionary<Build, HeroAttributes> heroGrades
        ) : base(name, traits)
    {
        HeroGrades = heroGrades;
        foreach (var grades in heroGrades.Values)
            grades?.CheckForTraitSuggestions(traits);
    }
    public Dictionary<Build, HeroAttributes> HeroGrades { get; }
}
public class PowerMetrics
{
    public int TurretDamage { get; init; }
    public int LaneClear { get; init; }
    public int CCAmount { get; init; }
    public int ReachingFarEvaluation { get; init; }
    public int UniqueAbilityPoints { get; init; }
    public int MetaPoints { get; init; }
}
public class TeamPowerMetrics : PowerMetrics
{
    public TeamPowerMetrics(HeroAttributes[] teammates)
    {
        foreach (var teammate in teammates)
        {
            if (teammate.DamageBlockable == DamageBlockable.Physical)
                PhisDefLikeliness += (int)teammate.DamageAmount;
            else if (teammate.DamageBlockable == DamageBlockable.Magical)
                MagicDefLikeliness += (int)teammate.DamageAmount;
        }
    }
    public int PhisDefLikeliness { get; init; }
    public int MagicDefLikeliness { get; init; }
}

public class HeroAttributes : PowerMetrics
{
    public GameStage DominatesStage { get; init; }
    public DamageAmount DamageAmount { get; init; }
    public DamageTarget DamageTarget { get; init; }
    public DamageBlockable DamageBlockable { get; init; }
    public CCType CCType { get; init; }
    public HashSet<HeroTrait> Traits { get; init; }
    protected int CountInteractionsBasedOnTraits(HeroAttributes other,
        Func<HeroAttributes, HeroAttributes, bool>[] nonTraitInteractionsRules,
        Func<HeroTrait, HeroAttributes, bool>[]? traitInteractionsRules = null,
        (HeroTrait currentTrait, HeroTrait otherTrait)[]? traitToTraitInteractionsRules = null)
    {
        var res = nonTraitInteractionsRules.Count(rule => rule(this, other));
        if (traitInteractionsRules?.Length > 0)
            res += Traits.Sum(trait => traitInteractionsRules.Count(rule => rule(trait, other)));
        if (traitToTraitInteractionsRules?.Length > 0)
            res += traitToTraitInteractionsRules.Count(rule => Traits.Contains(rule.currentTrait) && other.Traits.Contains(rule.otherTrait));
        return res;
    }
    public int CounterPointsAgainst(HeroAttributes enemy)
        => CountInteractionsBasedOnTraits(enemy, nonTraitDominationRules, traitDominationRules, TraitToTraitDominationRules);
    public int SynergyPointsWith(HeroAttributes teammate)
        => CountInteractionsBasedOnTraits(teammate, nonTraitSynergyRules, traitSynergyRules, TraitToTraitSynergyRules);
    public int AntiSynergyPointsWith(HeroAttributes teammate)
        => CountInteractionsBasedOnTraits(teammate, nonTraitAntiSynergyRules, traitAntiSynergyRules, TraitToTraitAntinergyRules);
    public static readonly Func<HeroAttributes, HeroAttributes, bool>[] nonTraitDominationRules =
    [
    ];
    public static readonly Func<HeroTrait, HeroAttributes, bool>[] traitDominationRules =
    [
        (HeroTrait strong, HeroAttributes enemy) => strong is HeroTrait.Dashes or HeroTrait.Fast && enemy.CCType == CCType.OAEDodgable,
    ];
    public static readonly Func<HeroAttributes, HeroAttributes, bool>[] nonTraitSynergyRules =
    [
    ];
    public static readonly Func<HeroTrait, HeroAttributes, bool>[] traitSynergyRules =
    [
    ];
    public static readonly Func<HeroAttributes, HeroAttributes, bool>[] nonTraitAntiSynergyRules =
    [
    ];
    public static readonly Func<HeroTrait, HeroAttributes, bool>[] traitAntiSynergyRules =
    [
    ];

    public static readonly (HeroTrait trait, HeroTrait teammateTrait)[] TraitToTraitAntinergyRules =
    [
        /*(HeroTrait.AOEBurst, HeroTrait.SpreadCC),*/
        (HeroTrait.LowDmg, HeroTrait.TeamDurability),
    ];
    public static readonly (HeroTrait trait, HeroTrait teammateTrait)[] TraitToTraitSynergyRules =
    [
        /*(HeroTrait.AOEBurst, HeroTrait.GatherCC),
        (HeroTrait.GatherCC, HeroTrait.GatherCC),
        (HeroTrait.InstantCC, HeroTrait.GatherCC),*/
        (HeroTrait.TeamDurability, HeroTrait.Oneshotable),
        (HeroTrait.LowMobility, HeroTrait.TeamSpeed),
    ];
    public static readonly (HeroTrait strongTrait, HeroTrait weakEnemyTrait)[] TraitToTraitDominationRules =
    [
        /*(HeroTrait.Mobility, HeroTrait.GatherCC),
        (HeroTrait.Mobility, HeroTrait.AOEBurst),
        (HeroTrait.TeamDurability, HeroTrait.AOEBurst),
        (HeroTrait.AOEBurst, HeroTrait.Oneshotable),
        (HeroTrait.AOEBurst, HeroTrait.LowMobility),
        (HeroTrait.GatherCC, HeroTrait.LowMobility),*/

        (HeroTrait.InstantCC, HeroTrait.Oneshotable),
        (HeroTrait.TeamSpeed, HeroTrait.LowMobility),
        (HeroTrait.SoloDmg, HeroTrait.Oneshotable),
        (HeroTrait.Tank, HeroTrait.SoloDmg),
        (HeroTrait.TankShredder, HeroTrait.Tank),
    ];
    public static event Action<HeroTrait, string>? OnTraitSuggested;
    public void CheckForTraitSuggestions(HeroTrait[] traits)
    {
        /*if (Durability >= 3 &&
            !(traits.Contains(HeroTrait.HighDurability) || traits.Contains(HeroTrait.TeamHeal)))
            OnTraitSuggested?.Invoke(HeroTrait.HighDurability, "Живучести много, но не указано, какой");*/
    }
}
public enum GameStage
{
    Early,
    Equal,
    Late
}
public enum CCType
{
    OAEDodgable,
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
public enum HeroTrait
{
    Dashes,
    Fast,
    Traveller,
    Oneshotable,
    Focuser,
    Tank,
    TeamDurability,
    TeamSpeed,
    LowMobility,
    InstantCC,
    LowDmg,
    SoloDmg,
    TankShredder,
    AntiMage,
}

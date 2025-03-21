using Evoloor.MOBACore;
namespace Evoloor.MLBBCore;

public abstract class Hero(string name) : IStaticData
{
    public string Name { get; init; } = name;
}
public class HeroStableGraded(string name, HeroAttributes attributes) : Hero(name)
{
    public HeroAttributes Attributes { get; init; } = attributes;
}
public class HeroDymanicGraded(
    string name,
    Dictionary<Build, HeroAttributes> heroGrades
    )
    : Hero(name)
{
    public Dictionary<Build, HeroAttributes> HeroGrades { get; } = heroGrades;
}
public class PowerMetrics
{
    public int? TurretDamage { get; set; }
    public int? LaneClear { get; set; }
    public int? CCAmount { get; set; }
    public int? ReachingFarEvaluation { get; set; }
    public int? UniqueAbilityPoints { get; set; }
    public int? MetaPoints { get; set; }
    public static int PropsAmount { get; } = 6;
}

public class HeroAttributes : PowerMetrics
{
    public static new int PropsAmount { get; } = 7 + PowerMetrics.PropsAmount;
    public GameStage? DominatesStage { get; set; }
    public DamageAmount? DamageAmount { get; set; }
    public DamageTarget? DamageTarget { get; set; }
    public DamageBlockable? DamageBlockable { get; set; }
    public CCType? CCType { get; set; }
    public IEnumerable<HeroTrait> Traits { get; set; } = new HashSet<HeroTrait>();
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
        (HeroTrait strong, HeroAttributes enemy) => strong is HeroTrait.Dashes or HeroTrait.Fast && enemy.CCType == MLBBCore.CCType.OAEDodgable,
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

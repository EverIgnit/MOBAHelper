using System;

namespace Evoloor.MLBBCore;
public class HeroInteractions
{
    protected static readonly (HeroSimpleGraded, Func<HashSet<HeroSimpleGraded>,bool>)[] CounteredByTeamRules = [

    ];
    protected static readonly (HeroSimpleGraded, Func<HeroSimpleGraded, bool>)[] HeroesUniqueCountersByTrait = [
        (Heroes.Hanzo,(enemy)=>enemy.Attributes.Traits.Contains(HeroTrait.Traveller) && enemy.Attributes.Traits.Contains(HeroTrait.Focuser))
    ];
    protected static readonly (HeroSimpleGraded, HeroTrait)[] HeroesUniqueCounters = [
    ];
    protected static readonly Func<HeroSimpleGraded, HeroSimpleGraded, bool>[] HeroesUniqueSynegries = [

    ];
    protected static readonly Func<HeroSimpleGraded, HeroSimpleGraded, bool>[] HeroesUniqueAntisynergies = [

    ];
    protected static readonly Func<HeroAttributes, HeroAttributes, bool>[] NonTraitCounterRules =
    [
    ];
    protected static readonly Func<HeroTrait, HeroAttributes, bool>[] TraitCounterRules =
    [
    ];
    protected static readonly Func<HeroAttributes, HeroAttributes, bool>[] NonTraitSynergyRules =
    [
    ];
    protected static readonly Func<HeroTrait, HeroAttributes, bool>[] TraitSynergyRules =
    [
    ];
    protected static readonly Func<HeroAttributes, HeroAttributes, bool>[] NonTraitAntiSynergyRules =
    [
    ];
    protected static readonly Func<HeroTrait, HeroAttributes, bool>[] TraitAntiSynergyRules =
    [
    ];
    protected static readonly (HeroTrait trait, HeroTrait teammateTrait)[] TraitToTraitAntiSinergyRules =
    [
        /*(HeroTrait.LowDmg, HeroTrait.Healer),*/
    ];
    protected static readonly (HeroTrait trait, HeroTrait teammateTrait)[] TraitToTraitSynergyRules =
    [
        //(HeroTrait.Healer, HeroTrait.Oneshotable),
        //(HeroTrait.LowMobility, HeroTrait.TeamSpeed),
    ];
    protected static readonly (HeroTrait strongTrait, HeroTrait weakEnemyTrait)[] TraitToTraitCounterRules =
    [
        /*(HeroTrait.InstantCC, HeroTrait.Oneshotable),
        (HeroTrait.TeamSpeed, HeroTrait.LowMobility),
        (HeroTrait.SoloDmg, HeroTrait.Oneshotable),
        (HeroTrait.Tank, HeroTrait.SoloDmg),
        (HeroTrait.TankShredder, HeroTrait.Tank),*/
    ];
    public static int GetConterPoints(HeroAttributes current, HeroAttributes other)
        => CountInteractionsBasedOnTraits(
            current,
            other,
            NonTraitCounterRules,
            TraitCounterRules,
            TraitToTraitCounterRules);
    public static int GetSynergyPoints(HeroAttributes current, HeroAttributes other)
        => CountInteractionsBasedOnTraits(
            current,
            other,
            NonTraitSynergyRules,
            TraitSynergyRules,
            TraitToTraitSynergyRules);
    public static int GetAntiSynergyPoints(HeroAttributes current, HeroAttributes other)
        => CountInteractionsBasedOnTraits(
            current,
            other,
            NonTraitAntiSynergyRules,
            TraitAntiSynergyRules,
            TraitToTraitAntiSinergyRules);
    public static int CountInteractionsBasedOnTraits(HeroAttributes current, HeroAttributes other,
        IEnumerable<Func<HeroAttributes, HeroAttributes, bool>> nonTraitInteractionsRules,
        IEnumerable<Func<HeroTrait, HeroAttributes, bool>>? traitInteractionsRules = null,
        IEnumerable<(HeroTrait currentTrait, HeroTrait otherTrait)>? traitToTraitInteractionsRules = null)
    {
        var res = nonTraitInteractionsRules.Count(rule => rule(current, other));
        if (traitInteractionsRules?.Count() > 0)
            res += current.Traits.Sum(trait => traitInteractionsRules.Count(rule => rule(trait, other)));
        if (traitToTraitInteractionsRules?.Count() > 0)
            res += traitToTraitInteractionsRules.Count(rule => current.Traits.Contains(rule.currentTrait) && other.Traits.Contains(rule.otherTrait));
        return res;
    }
}
class TeamToTeamComparer
{
    public HashSet<HeroSimpleGraded> Team1 { get; set; }
    public HashSet<HeroSimpleGraded> Team2 { get; set; }

}
class HeroToTeamComparer
{

}
class HeroToHeroComparer
{

}
class HeroToTeammateComparer
{

}

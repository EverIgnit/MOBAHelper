using Evoloor.MOBACore;
namespace Evoloor.MLBBCore.Drafter;
public class Player
{
    public Player((Hero hero, Lane lane)[] heroPool, string anyName, bool preferRealName)
    {
        HeroPool = heroPool;
        PreferRealName = preferRealName;
        if (PreferRealName)
            Name = anyName;
        else
            UserName = anyName;
    }
    public string? UserName { get; init; }
    public string? Name { get; init; }
    protected bool PreferRealName { get; }
    public string DisplayName => (PreferRealName ? Name : UserName)
        ?? throw new Exception("Как это нет имени");
    public (Hero, Lane)[] HeroPool { get; }
}
public abstract class DraftedPlayer(Lane lane, Player player, HeroSimpleGraded hero)
{
    public Lane Lane { get; protected set; } = lane;
    public Player Player { get; init; } = player;
    public HeroSimpleGraded Hero { get; init; } = hero;
}
public class DraftedExpLaner(Player player, HeroSimpleGraded hero) : DraftedPlayer(Lane.Exp, player, hero) { }
public class DraftedJungler(Player player, HeroSimpleGraded hero) : DraftedPlayer(Lane.Exp, player, hero) { }
public class DraftedMidLaner(Player player, HeroSimpleGraded hero) : DraftedPlayer(Lane.Exp, player, hero) { }
public class DraftedRoamer(Player player, HeroSimpleGraded hero) : DraftedPlayer(Lane.Exp, player, hero) { }
public class DraftedGoldLaner(Player player, HeroSimpleGraded hero) : DraftedPlayer(Lane.Exp, player, hero) { }
public class TeamComp
{
    public TeamComp(
        DraftedExpLaner exper,
        DraftedJungler jungler,
        DraftedMidLaner mider,
        DraftedRoamer roamer,
        DraftedGoldLaner golder)
    {
        Exper = exper;
        Jungler = jungler;
        Mider = mider;
        Roamer = roamer;
        Golder = golder;
        DraftedPlayers = [Exper, Jungler, Mider, Roamer, Golder];
    }
    public DraftedExpLaner Exper { get; init; }
    public DraftedJungler Jungler { get; init; }
    public DraftedMidLaner Mider { get; init; }
    public DraftedRoamer Roamer { get; init; }
    public DraftedGoldLaner Golder { get; init; }
    public HashSet<DraftedPlayer> DraftedPlayers { get; }
    public HashSet<HeroSimpleGraded> MetricsPerPick => [.. DraftedPlayers.Select(dp => dp.Hero)];
}
public class DraftedTeamPowerMetrics : TeamPowerMetrics
{
    public HashSet<HeroSimpleGraded> MetricsPerPick { get; init; }
    public TeamTier TeamTier { get; init; }
    public int TeamScore { get; set; }
    public int ProtVariety { get; set; }
    public DraftedTeamPowerMetrics(HashSet<HeroSimpleGraded> metricsPerPick)
    {
        MetricsPerPick = metricsPerPick;
        foreach (var metrics in MetricsPerPick.Select(m => m.Attributes))
        {
            TurretDamage += metrics.TurretDamage ?? 0;
            LaneClear += metrics.LaneClear ?? 0;
            CCAmount += metrics.CCAmount ?? 0;
            ReachingFarEvaluation += metrics.ReachingFarEvaluation ?? 0;
            UniqueAbilityPoints += metrics.UniqueAbilityPoints ?? 0;
            MetaPoints += metrics.MetaPoints ?? 0;
            TotalDamageAmount += (int?)metrics.DamageAmount ?? 0;
            if (metrics.DamageBlockable == DamageBlockable.Physical)
                PhisDefLikeliness += (int?)metrics.DamageAmount ?? 0;
            else if (metrics.DamageBlockable == DamageBlockable.Magical)
                MagicDefLikeliness += (int?)metrics.DamageAmount ?? 0;
            if (metrics.DamageTarget == DamageTarget.Solo)
                AssasinDamageAmount += (int?)metrics.DamageAmount ?? 0;
            if (metrics.HpAmount == HpAmount.Squishy)
                SquishiesAmount++;
            else if(metrics.HpAmount == HpAmount.Tanky)
                TanksAmount++;
            if (metrics.CCType == CCType.AOE)
                AOECCAmount += metrics.CCAmount ?? 0;
            if (metrics.Traits.Contains(HeroTrait.Traveller))
                HasTraveller = true;
            if (metrics.Traits.Contains(HeroTrait.Initiator))
                InitiatorsAmount++;
        }
        HeroesDominateLate = MetricsPerPick.Count(m => m.Attributes.DominatesStage == GameStage.Late);
        const double PROTTYPE_PREFERABILY_THRESHHOLD = 1.5;
        const int MIN_VARIETY_DMG = 3,
            TEAM_SIZE = 5;
        ProtVariety = PhisDefLikeliness >= MIN_VARIETY_DMG? 1 : 0
            + MagicDefLikeliness >= MIN_VARIETY_DMG? 1 : 0
            + PhisDefLikeliness / MagicDefLikeliness < PROTTYPE_PREFERABILY_THRESHHOLD && MagicDefLikeliness / PhisDefLikeliness < PROTTYPE_PREFERABILY_THRESHHOLD ? 1 : 0;

        TeamScore = TurretDamage ?? 0 +
            LaneClear ?? 0 +
            CCAmount ?? 0 +
            ReachingFarEvaluation ?? 0 +
            UniqueAbilityPoints ?? 0 +
            MetaPoints ?? 0 +
            TotalDamageAmount +
        //TODO: if noone builds protection, do not consider:
            ProtVariety +
            (HeroesDominateLate > 2 ? METRICS_CEILING : HeroesDominateLate) +
            (InitiatorsAmount == 0 ? 0 : 1) +
            (LaneClear == 0?0:1)+
            (int)Math.Ceiling(TanksAmount/(METRICS_CEILING/ (double)TEAM_SIZE));
    }
}
public static class TeamTiers
{
    public static TeamPowerMetrics EvaluateTeam(TeamPowerMetrics metricsPerPick)
        => AllTiers.First(t => t.CompareTo(metricsPerPick) >= 0);
    public static Dictionary<TeamTier, TeamPowerMetrics> TiersMetrics
        => throw new NotImplementedException();
    public static HashSet<TeamPowerMetrics> AllTiers { get; } = [.. TiersMetrics.OrderByDescending(t => (int)t.Key).Select(tierMetrics => tierMetrics.Value)];
}
public enum TeamTier
{
    Imba = 10,
    Strong = 8,
    Empty = 0
}

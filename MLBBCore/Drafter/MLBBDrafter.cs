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
public abstract class DraftedPlayer(Lane lane, Player player, Hero hero)
{
    public Lane Lane { get; protected set; } = lane;
    public Player Player { get; init; } = player;
    public Hero Hero { get; init; } = hero;
}
public class DraftedExpLaner(Player player, Hero hero) : DraftedPlayer(Lane.Exp, player, hero) { }
public class DraftedJungler(Player player, Hero hero) : DraftedPlayer(Lane.Exp, player, hero) { }
public class DraftedMidLaner(Player player, Hero hero) : DraftedPlayer(Lane.Exp, player, hero) { }
public class DraftedRoamer(Player player, Hero hero) : DraftedPlayer(Lane.Exp, player, hero) { }
public class DraftedGoldLaner(Player player, Hero hero) : DraftedPlayer(Lane.Exp, player, hero) { }
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
    public DraftedPlayer[] DraftedPlayers { get; }
}
public class TeamGrades : HeroAttributes
{
    public TeamGrades(TeamComp team)
    {
        foreach (var dp in team.DraftedPlayers)
            foreach (var grade in dp.Hero switch
            {
                HeroStableGraded sg => [sg.Grades],
                HeroDymanicGraded dyn => dyn.HeroGrades.Values.AsEnumerable(),
                _ => throw new MOBAException("У героя нет стабильного или гибкого билда")
            })
            {
                PhisDamage += grade.PhisDamage;
                MagicDamage += grade.MagicDamage;
                PureDamage += grade.PureDamage;
                TurretDamage += grade.TurretDamage;
                LaneClear += grade.LaneClear;
                CCAmount += grade.CCAmount;
                Early += grade.Early;
                Late += grade.Late;
                ReinforcementRoam += grade.ReinforcementRoam;
                ReachingFarEvaluation += grade.ReachingFarEvaluation;
                Durability += grade.Durability;
                UniqueAbility += grade.UniqueAbility;
            }
    }
    public TeamTier Tier { get; }
}
public static class TeamTiers
{
    // public static TeamTier Imba { get; } = new();
}
public class TeamTier : HeroAttributes
{
    protected TeamTier() : base(){ }
}
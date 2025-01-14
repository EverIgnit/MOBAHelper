namespace Evoloor.MOBACore;
public class MOBAException(string message) : Exception(message) { }
public enum Lane { Exp, Jungle, Mid, Gold, Support }
public interface IAccount : IEquatable<IAccount>
{
    string Name { get; }
    bool IEquatable<IAccount>.Equals(IAccount? other) => other?.Name == Name;
}
public interface IStaticData
{
    string Name { get; }
}
public interface IMatchPlayer<TAccount, TChampion, TItem>
    where TAccount : IAccount
    where TChampion : IStaticData
    where TItem : IStaticData
{
    TAccount Account { get; }
    TChampion Champion { get; }
    TItem[] Items { get; }
}
public interface IMatch<TMatchPlayer, TAccount, TChampion, TItem>
    where TMatchPlayer : IMatchPlayer<TAccount, TChampion, TItem>
    where TChampion : IStaticData
    where TAccount : IAccount
    where TItem : IStaticData
{
    TMatchPlayer[] WinningTeam { get; }
    TMatchPlayer[][] Teams { get; }
    TMatchPlayer GetPlayer(TAccount account) => AllPlayers.First(p => p.Account.Equals(account));
    bool DidPlayerWin(IAccount account) => GetPlayerTeamComp(account).Equals(WinningTeam);
    protected TMatchPlayer[] GetPlayerTeamComp(IAccount account)
        => Teams.First(t => t.Any(matchPlayer => matchPlayer.Account.Equals(account)));
    protected IEnumerable<TMatchPlayer> AllPlayers => Teams.SelectMany(team => team);
    TMatchPlayer[] GetTeammates(IAccount account) => GetPlayerTeamComp(account)
        .Where(matchPlayer => !matchPlayer.Account.Equals(account))
        .ToArray();
    TMatchPlayer[] GetAllOpponents(IAccount account)
        => AllPlayers.Except(GetPlayerTeamComp(account)).ToArray();
}
public interface I2TeamMatch<TMatchPlayer, TAccount, TChampion, TItem>
    : IMatch<TMatchPlayer, TAccount, TChampion, TItem>
    where TMatchPlayer : IMatchPlayer<TAccount, TChampion, TItem>
    where TChampion : IStaticData
    where TAccount : IAccount
    where TItem : IStaticData
{
    TMatchPlayer[] Team1 { get; }
    TMatchPlayer[] Team2 { get; }
    TMatchPlayer[][] IMatch<TMatchPlayer, TAccount, TChampion, TItem>.Teams => [Team1, Team2];
}
public interface IStaticData<TChampion, TItem>
    where TChampion : IStaticData
    where TItem : IStaticData
{
    TChampion[] Champions { get; }
    TItem[] Items { get; }
}
public class MatchPerspective<TMatch, TMatchPlayer, TAccount, TChampion, TItem>
    (TAccount account, TMatch match)
    where TMatch : IMatch<TMatchPlayer, TAccount, TChampion, TItem>
    where TMatchPlayer : IMatchPlayer<TAccount, TChampion, TItem>
    where TChampion : IStaticData
    where TAccount : IAccount
    where TItem : IStaticData
{
    public bool IsVictory { get; } = match.DidPlayerWin(account);
    public TMatchPlayer Player { get; } = match.GetPlayer(account);
    public TMatchPlayer[] Teammates { get; } = match.GetTeammates(account);
    public TMatchPlayer[] Opponents { get; } = match.GetAllOpponents(account);
}
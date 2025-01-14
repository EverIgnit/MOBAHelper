namespace Evoloor.MOBACore.Stats;
public interface IWinRate
{
    int TotalGames { get; }
    int WonGames { get; }
    int LostGames => TotalGames - WonGames;
    double WRValue => Math.Round((double)WonGames / TotalGames, 2);
    string AsStr => $"{WRValue,3}% ({WonGames,3}W:{TotalGames,3}G)";
}
public class MatchesAnalizer<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem>
    (IEnumerable<TMatchPerspective> matches)
    : IWinRate
    where TMatchPerspective : MatchPerspective<TMatch, TMatchPlayer, TAccount, TChampion, TItem>
    where TMatch : I2TeamMatch<TMatchPlayer, TAccount, TChampion, TItem>
    where TMatchPlayer : IMatchPlayer<TAccount, TChampion, TItem>
    where TChampion : IStaticData
    where TAccount : IAccount
    where TItem : IStaticData
{
    public string? DisplayName { get; set; }
    public int TotalGames => Matches.Count();
    public int WonGames => Matches.Where(m => m.IsVictory).Count();
    public IEnumerable<TMatchPerspective> Matches { get; } = matches;
    protected IEnumerable<MatchesAnalizer<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem>>
        PartitionBase<TKey>(IEnumerable<IGrouping<TKey, TMatchPerspective>> groups, Converter<TKey, string>? keyStrConverter)
        => groups.Select(group => new MatchesAnalizer<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem>(group)
        { DisplayName = keyStrConverter is null ? group.Key?.ToString() : keyStrConverter(group.Key) });
    public IEnumerable<MatchesAnalizer<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem>>
        PartitionMatchHistory<TKey>(Func<TMatchPerspective, TKey> differentiator, Converter<TKey, string>? keyStrConverter = null)
        => PartitionBase(Matches.GroupBy(differentiator), keyStrConverter);
    public IEnumerable<MatchesAnalizer<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem>>
        PartitionMatchHistory<TKey>(Func<TMatchPerspective, TKey[]> differentiator, Converter<TKey, string>? keyStrConverter = null)
        => PartitionBase(Matches.SelectMany(match => differentiator(match), (match, differentiatedItem) => new { Key = differentiatedItem, Match = match })
            .GroupBy(pair => pair.Key, pair => pair.Match), keyStrConverter);
}
public static class ConsoleDisplayer
{
    static string GetDisplayStr<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem>
        (MatchesAnalizer<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem> matchHistory)
        where TMatchPerspective : MatchPerspective<TMatch, TMatchPlayer, TAccount, TChampion, TItem>
        where TMatch : I2TeamMatch<TMatchPlayer, TAccount, TChampion, TItem>
        where TMatchPlayer : IMatchPlayer<TAccount, TChampion, TItem>
        where TChampion : IStaticData
        where TAccount : IAccount
        where TItem : IStaticData
        => $"{matchHistory.DisplayName,16} {(matchHistory as IWinRate).AsStr}";
    static string GetDisplayStr<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem>
        (IEnumerable<MatchesAnalizer<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem>> matchHistories)
        where TMatchPerspective : MatchPerspective<TMatch, TMatchPlayer, TAccount, TChampion, TItem>
        where TMatch : I2TeamMatch<TMatchPlayer, TAccount, TChampion, TItem>
        where TMatchPlayer : IMatchPlayer<TAccount, TChampion, TItem>
        where TChampion : IStaticData
        where TAccount : IAccount
        where TItem : IStaticData
        => string.Join("\n\t", matchHistories.Select(GetDisplayStr));
    public static void SingleStat<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem>
        (MatchesAnalizer<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem> matchHistory)
        where TMatchPerspective : MatchPerspective<TMatch, TMatchPlayer, TAccount, TChampion, TItem>
        where TMatch : I2TeamMatch<TMatchPlayer, TAccount, TChampion, TItem>
        where TMatchPlayer : IMatchPlayer<TAccount, TChampion, TItem>
        where TChampion : IStaticData
        where TAccount : IAccount
        where TItem : IStaticData
        => Console.WriteLine(GetDisplayStr(matchHistory));
    public static void WithInnerStats<TKey, TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem>
        (MatchesAnalizer<TMatchPerspective, TMatch, TMatchPlayer, TAccount, TChampion, TItem> matchHistory,
            Func<TMatchPerspective, TKey> differentiator,
            Converter<TKey, string>? keyStrConverter = null)
        where TMatchPerspective : MatchPerspective<TMatch, TMatchPlayer, TAccount, TChampion, TItem>
        where TMatch : I2TeamMatch<TMatchPlayer, TAccount, TChampion, TItem>
        where TMatchPlayer : IMatchPlayer<TAccount, TChampion, TItem>
        where TChampion : IStaticData
        where TAccount : IAccount
        where TItem : IStaticData
    {
        SingleStat(matchHistory);
        Console.WriteLine(GetDisplayStr(matchHistory.PartitionMatchHistory(differentiator, keyStrConverter)));
    }
}
public interface ISavedPlayerMatchesReader<TMatch, TMatchPlayer, TAccount, TChampion, TItem>
    where TMatch : IMatch<TMatchPlayer, TAccount, TChampion, TItem>
    where TMatchPlayer : IMatchPlayer<TAccount, TChampion, TItem>
    where TChampion : IStaticData
    where TAccount : IAccount
    where TItem : IStaticData
{
    IEnumerable<MatchPerspective<TMatch, TMatchPlayer, TAccount, TChampion, TItem>> Matches { get; }
}

using Evoloor.MOBACore.Stats;
namespace Evoloor.LOLCore.Stats;
public record PlayedStats(int TotalGames, int WonGames) : IWinRate;
public class LOLMatchesAnalizer(IEnumerable<MatchPerspective> matches)
    : MatchesAnalizer<MatchPerspective, Match, MatchPlayer, Account, Champion, Item>(matches);

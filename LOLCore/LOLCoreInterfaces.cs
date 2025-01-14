using Evoloor.MOBACore;
namespace Evoloor.LOLCore;
public record Account(string RiotId, string Name) : IAccount;
public record Champion(string Name) : IStaticData;
public record Item(string Name) : IStaticData;
public record Rune(string Name, bool IsKeyStone) : IStaticData;
public record RuneShard(string Name) : IStaticData;
public record StatItem(string Name, bool IsLegendary) : Item(Name);
public record Boots(string Name) : Item(Name);
public record BootsS25(string Name, int Tier) : Boots(Name);
public record MatchPlayer(Account Account, Champion Champion, Item[] Items) // TODO: runes, shards
    : IMatchPlayer<Account, Champion, Item>;
public record SRMatchPlayer(Account Account, Champion Champion, Item[] Items, Lane Lane)
    : MatchPlayer(Account, Champion, Items);
public record Match(MatchPlayer[] Team1, MatchPlayer[] Team2)
    : I2TeamMatch<MatchPlayer, Account, Champion, Item>
{
    public MatchPlayer[] WinningTeam => throw new NotImplementedException(); // TODO: implement
}
public record SRMatch(SRMatchPlayer[] Team1, SRMatchPlayer[] Team2)
    : I2TeamMatch<SRMatchPlayer, Account, Champion, Item>
{
    public SRMatchPlayer[] WinningTeam => throw new NotImplementedException(); // TODO: implement
}
public class MatchPerspective(Account Account, Match Match)
    : MatchPerspective<Match, MatchPlayer, Account, Champion, Item>(Account, Match);
public class StaticStorage
{
    protected StaticStorage() { }
    public static readonly StaticStorage Instance = new();
}
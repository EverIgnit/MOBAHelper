using Evoloor.MOBACore;
namespace Evoloor.MLBBCore.Drafter;
public static class DraftPlayersOptions
{
    public static Player Evoloor { get; } = new(
        heroPool: [
            (Heroes.Harith, Lane.Mid),
            (Heroes.Harith, Lane.Gold),
            (Heroes.Zhask, Lane.Gold),
            (Heroes.Zhask, Lane.Mid),
            (Heroes.Zhuxin, Lane.Mid),
            (Heroes.Xavier, Lane.Mid),
        ],
        anyName: "Evoloor",
        preferRealName: false)
    {
        Name = "Глеб"
    };
}

using Cards.Web.Models;

namespace Cards.Web.Hubs;

public interface IPokerClient
{
    Task RevealReceived(RevealNotification revealNotification);
    Task PlayersChanged(List<Player> players);
    Task GameStateChanged(GameState gameState);
    Task GameJoined(GameState gameState, Player player);
    Task BoardReset();
}
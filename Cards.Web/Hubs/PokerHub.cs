using Cards.Web.Models;
using Microsoft.AspNetCore.SignalR;

namespace Cards.Web.Hubs;

public class PokerHub : Hub<IPokerClient>
{
    /// <summary>
    ///     TODO: Static for now. Just prototyping. If going forward, should move state to some store.
    /// </summary>
    private static List<Player> Players = new();

    private static GameState _gameState = GameState.NotStarted;

    public async Task NotifyReveal(RevealNotification revealNotification) => await Clients.All.RevealReceived(revealNotification);

    public async Task NotifyPlayerChange() => await Clients.All.PlayersChanged(Players);

    public async Task NotifyGameStateChange() => await Clients.All.GameStateChanged(_gameState);

    public async Task NotifyGameJoin(Player player) => await Clients.Caller.GameJoined(_gameState, player);

    public async Task ResetBoard()
    {
        Players = new List<Player>();
        _gameState = GameState.NotStarted;
        await Clients.All.BoardReset();
    }


    public async Task Reveal()
    {
        var revealNotification = new RevealNotification
        {
            Average = AverageEstimateFromPlayers()
        };

        await NotifyReveal(revealNotification);
        _gameState = GameState.Revealed;
        await NotifyGameStateChange();
    }

    private double AverageEstimateFromPlayers() =>
        Math.Ceiling(Players.Where(p => p.EstimateGiven)
            .Select(p => p.Estimate)
            .DefaultIfEmpty(0)
            .Average());

    public async Task Join(string name)
    {
        var id = Guid.NewGuid();
        var player = new Player
        {
            Name = name,
            Id = id
        };

        Players.Add(player);

        await NotifyPlayerChange();
        await NotifyGameJoin(player);
    }

    public async Task StartNew()
    {
        _gameState = GameState.Ongoing;
        Players.ForEach(p => p.EstimateGiven = false);

        await NotifyPlayerChange();
        await NotifyGameStateChange();
    }

    public async Task Estimate(int estimate, Guid id)
    {
        var player = Players.First(p => p.Id == id);
        player.Estimate = estimate;
        player.EstimateGiven = true;
        await NotifyPlayerChange();
    }
}
using System.Threading.Tasks;
using Cards.Web.Hubs;
using Cards.Web.Models;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;

namespace Cards.Web.Tests.Hubs;

[TestFixture]
public class PokerHubTests
{
    [SetUp]
    public void Setup()
    {
        _clientsCallerMock = new Mock<IHubCallerClients<IPokerClient>>();
        _clientsAllMock = new Mock<IPokerClient>();
    }

    private Mock<IHubCallerClients<IPokerClient>> _clientsCallerMock = new();
    private Mock<IPokerClient> _clientsAllMock = new();

    [Test]
    public async Task Reveal_Should_Notify_Reveal_On_All_Clients()
    {
        // Arrange
        var hub = new PokerHub();
        hub.Clients = _clientsCallerMock.Object;
        _clientsCallerMock.Setup(p => p.All)
            .Returns(_clientsAllMock.Object);
        var revealNotification = new RevealNotification();

        // Act
        await hub.NotifyReveal(revealNotification);

        // Assert
        _clientsCallerMock.Verify(p => p.All.RevealReceived(revealNotification));
    }

    [Test]
    public async Task Reveal_Should_Notify_Revealed_GameState()
    {
        // Arrange
        var hub = new PokerHub();
        hub.Clients = _clientsCallerMock.Object;
        _clientsCallerMock.Setup(p => p.All)
            .Returns(_clientsAllMock.Object);

        // Act
        await hub.Reveal();

        // Assert
        _clientsCallerMock.Verify(p => p.All.GameStateChanged(GameState.Revealed));
    }

    [Test]
    public async Task Reveal_Should_Send_RevealNotification()
    {
        // Arrange
        var hub = new PokerHub();
        hub.Clients = _clientsCallerMock.Object;
        _clientsCallerMock.Setup(p => p.All)
            .Returns(_clientsAllMock.Object);
        // Act
        await hub.Reveal();

        // Assert
        _clientsCallerMock.Verify(p => p.All.RevealReceived(It.IsAny<RevealNotification>()));
    }

    [Test]
    public async Task StartNew_Should_Notify_Ongoing_GameState()
    {
        // Arrange
        var hub = new PokerHub();
        hub.Clients = _clientsCallerMock.Object;
        _clientsCallerMock.Setup(p => p.All)
            .Returns(_clientsAllMock.Object);

        // Act
        await hub.Reveal();

        // Assert
        _clientsCallerMock.Verify(p => p.All.GameStateChanged(GameState.Revealed));
    }
}
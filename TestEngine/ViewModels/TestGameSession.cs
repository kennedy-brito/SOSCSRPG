using Engine.ViewModels;

namespace TestEngine.ViewModels;
public class TestGameSession
{
    [Fact]
    public void TestCreateGameSession()
    {
        var gameSession = new GameSession();

        Assert.NotNull(gameSession.CurrentPlayer);

        var expectedLocation = "Town Square";
        var actualLocation = gameSession.CurrentLocation.Name;

        Assert.Equal(expectedLocation, actualLocation);
    }

    [Fact]
    public void TestPlayerMovesHomeAndIsCompletelyHealedOnKilled()
    {
        var gameSession = new GameSession();

        gameSession.CurrentPlayer.TakeDamage(999);

        //testing moving to home when killed
        var expectedLocation = "Home";
        var actualLocation = gameSession.CurrentLocation.Name;

        Assert.Equal(expectedLocation, actualLocation);

        //testing completely heal when killed
        var expectedHitPoints = gameSession.CurrentPlayer.Level * 10;
        var actualHitPoints = gameSession.CurrentPlayer.CurrentHitPoints;

        Assert.Equal(expectedHitPoints, actualHitPoints);
    }
}

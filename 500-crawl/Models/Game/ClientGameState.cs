using _500_crawl.Models.Cards;

namespace _500_crawl.Models.Game;

/// <summary>
/// This is the game state as the client sees it
/// </summary>
public class ClientGameState
{
    public GamePhase Phase { get; set; }
    public long PlayerHand { get; set; }
    public int PlayerHealth { get; set; }
    public int AiHealth { get; set; }
    public int WonHands { get; set; }
    public int LostHands { get; set; }
    public int RoundTarget { get; set; }
    public bool PlayerLeading { get; set; }
    public Suit Trumps { get; set; }

    // optional mulighan
    public long? Kitty { get; set; }
    // optional opponents card
    public int? AiCard { get; set; }
}
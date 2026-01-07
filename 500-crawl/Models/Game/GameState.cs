using _500_crawl.Models.Cards;

namespace _500_crawl.Models.Game;

/// <summary>
/// This represents the state of a game
/// </summary>
public class GameState
{
    // this belongs to SQL and should not be touched outside of it
    public int Id { get; set; }


    public GamePhase Phase { get; set; }
    public long PlayerHand { get; set; }
    public long AiHand { get; set; }
    public int PlayerHealth { get; set; }
    public int AiHealth { get; set; }
    public int WonHands { get; set; }
    public int LostHands { get; set; }
    public int DeckSeed { get; set; }
    public int DeckPlace { get; set; }
    public int RoundTarget { get; set; }
    public bool PlayerLeading { get; set; }
    public Suit Trumps { get; set; }
    public long AiState { get; set; }
}
namespace _500_crawl.Models.Game;

using System.Security.Cryptography; // we need a cryptographically secure rng
using _500_crawl.Models.Cards;

/// <summary>
/// This class manages the current game session
/// </summary>
public class GameSession
{
    // the database to which stores the games
    private readonly SiteDbContext database;
    private static int STARTING_HEALTH = 500;
    // the game state
    public GameState? State { get; private set; }

    public GameSession(SiteDbContext database)
    {
        this.database = database;
    }

    /// <summary>
    /// Create a new game state and assign it.
    /// 
    /// For now we draw out first hands here since we start
    /// in the deciding phase in which hands should be drawn at
    /// the start of.
    /// </summary>
    public void newGame()
    {
        // generate the seed then use it to make the deck and the initial hands
        int seed = RandomNumberGenerator.GetInt32(int.MaxValue);
        Hand playerHand = new Hand();
        Hand aiHand = new Hand();
        Deck deck = new Deck(seed);
        // the player gets the first 10 cards and the ai gets the next 10
        foreach (int card in deck.DrawCards(10))
        {
            playerHand.addCard(card);
        }
        foreach (int card in deck.DrawCards(10))
        {
            aiHand.addCard(card);
        }

        State = new GameState
        {
            Phase = GamePhase.Deciding,
            PlayerHand = playerHand.HandBits,
            AiHand = aiHand.HandBits,
            PlayerHealth = STARTING_HEALTH,
            AiHealth = STARTING_HEALTH,
            WonHands = 0,
            LostHands = 0,
            DeckSeed = seed,
            DeckPlace = deck.CurrentCard,
            RoundTarget = 0,
            PlayerLeading = true,
            Trumps = Suit.Ground,
            AiState = 0L
        };
        // =/.....
        long overlap = playerHand.HandBits & aiHand.HandBits;
        if (overlap != 0)
        {
            for (int i = 0; i < 41; i++)
            {
                if ((overlap & (1L << i)) != 0)
                {
                    throw new Exception($"DUPLICATE CARD DEALT AT DRAW TIME: {i}");
                }
            }
        }
        database.Games.Add(State);
        database.SaveChanges();
    }

    public void nextRound()
    {
        // generate the seed then use it to make the deck and the initial hands
        int seed = RandomNumberGenerator.GetInt32(int.MaxValue);
        Hand playerHand = new Hand();
        Hand aiHand = new Hand();
        Deck deck = new Deck(seed);
        // the player gets the first 10 cards and the ai gets the next 10
        foreach (int card in deck.DrawCards(10))
        {
            playerHand.addCard(card);
        }
        foreach (int card in deck.DrawCards(10))
        {
            aiHand.addCard(card);
        }

        State.Phase = GamePhase.Deciding;
        State.PlayerHand = playerHand.HandBits;
        State.AiHand = aiHand.HandBits;
        State.WonHands = 0;
        State.LostHands = 0;
        State.DeckSeed = seed;
        State.DeckPlace = deck.CurrentCard;
        State.PlayerLeading = true;
        State.AiState = 0L;
        State.AiCard = 0;
    }

    /// <summary>
    /// Loads the game with the given id.
    /// </summary>
    /// <param name="gameId">The id of the current game</param>
    public void loadGame(int gameId)
    {
        State = database.Games.Single(g => g.Id == gameId);
    }

    /// <summary>
    /// Saves the current game. This assumes the game already exists.
    /// </summary>
    public void saveGame()
    {
        if (State != null)
        {
            database.SaveChanges();
        }
        
    }

    /// <summary>
    /// Get the portion of teh game state visible to the client
    /// </summary>
    /// <returns>The visible portion of the game state</returns>
    public ClientGameState getVisibleState()
    {
        if (State == null) 
            throw new InvalidOperationException("Tried to get game state while game wasn't loaded");
        return new ClientGameState
        {
            Phase = State.Phase,
            PlayerHand = State.PlayerHand,
            PlayerHealth = State.PlayerHealth,
            AiHealth = State.AiHealth,
            WonHands = State.WonHands,
            LostHands = State.LostHands,
            RoundTarget = State.RoundTarget,
            PlayerLeading = State.PlayerLeading,
            Trumps = State.Trumps,
            AiCard = State.AiCard
        };
    }
}
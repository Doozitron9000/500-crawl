namespace _500_crawl.Models.Cards;

public class Deck
{
    // we have 40 regular cards + 1 dragon card
    private const int CARD_COUNT = 41;
    // The card we are currently up to
    public int CurrentCard { get; private set; }
    private int[] deck; // the deck of cards
    
    /// <summary>
    /// Default constructor.
    /// 
    /// This generates the shuffled deck
    /// </summary>
    public Deck(int seed)
    {
        deck = Shuffle(seed);
    }

    /// <summary>
    /// Constructor for use when creating an already partially drawn deck
    /// </summary>
    /// <param name="seed">The deck's seed</param>
    /// <param name="currentCard">The card we have drawn up to</param>
    public Deck(int seed, int currentCard)
    {
        deck = Shuffle(seed);
        CurrentCard = currentCard;
    }

    // /// <summary>
    // /// Draws the given number of cards and returns them.
    // /// </summary>
    // /// <param name="count">The number of cards to draw</param>
    // /// <returns>The cards</returns>
    // public Card[] DrawCards(int count)
    // {
    //     // make sure there are cards left and if we are trying to overdraw throw an exception
    //     if (deck.Length < count + CurrentCard)
    //     {
    //         throw new InvalidOperationException($"Attempted to draw {count} cards while only {deck.Length - CurrentCard} were remaining");
    //     }
    //     // make the array to store the cards
    //     Card[] drawnCards = new Card[count];
    //     // draw the card numbers then generate the cards from them
    //     for (int i = 0; i < count; i++)
    //     {
    //         int cardNumber = deck[CurrentCard];
    //         // if the card number is 53 or higher this is a joker
    //         if (cardNumber > 52)
    //         {
    //             // give the joker an unbeatable value
    //             drawnCards[i] = new Card(99, Suit.NoSuit);
    //         }
    //         else
    //         {
    //             // otherwise generate the card as normal.
    //             Suit suit = (Suit)(cardNumber/13);
    //             int value = cardNumber%13;
    //             drawnCards[i] = new Card(value, suit);
    //         }
    //         CurrentCard++;
    //     }
    //     return drawnCards;
    // }

    /// <summary>
    /// Returns an array of the numbers 1-53 in a random order
    /// 
    /// This uses a Fisher-Yates shuffling algorithim and is intended
    /// to reflect a deck including a joker and 13 cards of each suit
    /// </summary>
    /// <returns>The shuffled deck</returns>
    private int[] Shuffle(int seed)
    {
        // make the random number generator
        Random rng = new Random(seed);
        // make the deck.... this should all 40 cards + the dragon
        int[] deck = Enumerable.Range(0,CARD_COUNT).ToArray();
        // now implement a fisher-yaters shuffling algorithim
        for(int i = CARD_COUNT - 1; i > 0; i--)
        {
            // get the index of the number i should swamp with
            int swap = rng.Next(i+1);
            // now swap the two numbers
            (deck[i], deck[swap]) = (deck[swap], deck[i]);
        }
        // now return the shuffled deck
        return deck;
    }

    /// <summary>
    /// Draw the given number of cards and return their values
    /// </summary>
    /// <param name="count">The number of cards to draw</param>
    /// <returns>The values of the cards</returns>
    public int[] DrawCards(int count)
    {
        if (deck.Length < count + CurrentCard)
         {
             throw new InvalidOperationException($"Attempted to draw {count} cards while only {deck.Length - CurrentCard} were remaining");
         }
        // make the array to store teh cards
        int[] drawn = new int[count];
        for (int i = 0; i < count; i++)
        {
            drawn[i] = deck[i+CurrentCard];
            CurrentCard++;
        }
        return drawn;
    }
}
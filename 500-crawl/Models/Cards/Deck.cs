namespace _500_crawl.Models.Cards;

using System.Security.Cryptography; // we need a cryptographically secure rng

public class Deck
{
    private int _currentCard = 0; // the card we are currently up to
    private int[] deck; // the deck of cards
    
    /// <summary>
    /// Default constructor.
    /// 
    /// This generates the shuffled deck
    /// </summary>
    public Deck()
    {
        deck = Shuffle();
    }

    /// <summary>
    /// Draws the given number of cards and returns them.
    /// </summary>
    /// <param name="count">The number of cards to draw</param>
    /// <returns>The cards</returns>
    public Card[] DrawCards(int count)
    {
        // make sure there are cards left and if we are trying to overdraw throw an exception
        if (deck.Length < count + _currentCard)
        {
            throw new InvalidOperationException($"Attempted to draw {count} cards while only {deck.Length - _currentCard} were remaining");
        }
        // make the array to store the cards
        Card[] drawnCards = new Card[count];
        // draw the card numbers then generate the cards from them
        for (int i = 0; i < count; i++)
        {
            int cardNumber = deck[_currentCard];
            // if the card number is 53 or higher this is a joker
            if (cardNumber > 52)
            {
                // give the joker an unbeatable value
                drawnCards[i] = new Card(99, Suit.NoSuit);
            }
            else
            {
                // otherwise generate the card as normal.
                Suit suit = (Suit)(cardNumber/13);
                int value = cardNumber%13;
                drawnCards[i] = new Card(value, suit);
            }
            _currentCard++;
        }
        return drawnCards;
    }

    /// <summary>
    /// Returns an array of the numbers 1-53 in a random order
    /// 
    /// This uses a Fisher-Yates shuffling algorithim and is intended
    /// to reflect a deck including a joker and 13 cards of each suit
    /// </summary>
    /// <returns>The shuffled deck</returns>
    public static int[] Shuffle()
    {
        // make the deck.... this should include a joker so 53 cards
        int[] deck = Enumerable.Range(1,53).ToArray();
        // now implement a fisher-yaters shuffling algorithim
        for(int i = 52; i > 0; i--)
        {
            // get the index of the number i should swamp with
            int swap = RandomNumberGenerator.GetInt32(i+1);
            // now swap the two numbers
            (deck[i], deck[swap]) = (deck[swap], deck[i]);
        }
        // now return the shuffled deck
        return deck;
    }
}
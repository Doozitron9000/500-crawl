namespace _500_crawl.Models.Cards;


/// <summary>
/// A basic card class containing a value and a suit.
/// </summary>

// apparantly game stuff should be sealed since it closes off another cheat point.
// since this is going to be in a hash set it should also be equatable
public sealed class Card : IEquatable<Card>
{
    public int Value { get; }
    public Suit Suit { get; }

    /// <summary>
    /// Default constructor.
    /// 
    /// This defines the card's value and suit.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="suit"></param>
    public Card(int value, Suit suit)
    {
        Value = value;
        Suit = suit;
    }

    /// <summary>
    /// Equals implementation comparing based on value and suit
    /// </summary>
    /// <param name="other">The card to compare with</param>
    /// <returns>Whether the two cards and the same value and suit</returns>
    public bool Equals(Card? other)
    {
        // handle null case
        if (other == null) return false;
        return other.Value == Value && other.Suit == Suit;
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        // return the combined hash code of the value and suit
        return HashCode.Combine(Value, Suit);
    }
}
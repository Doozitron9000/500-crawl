namespace _500_crawl.Models.Cards;

public class Hand
{
    // the max value a card can be
    private const int MAX_VALUE = 40;
    // The long that actually represents the hand.
    public long HandBits { get; set; }

    /// <summary>
    /// Adds the card with the corresponding value to the hand
    /// </summary>
    /// <param name="value">The value of the card to add</param>
    public void addCard(int value)
    {
        if (!IsValidCard(value)) throw new InvalidOperationException("The given card value is outside the valid range");
        long bits = 1L << value;
        // throw an exception if the car is not present
        if (hasCard(bits)) throw new InvalidOperationException("Card is already in hand");
        // use bitwise operation to add the card to the hand
        HandBits |= bits;
    }

    /// <summary>
    /// Removes the card with the corresponding value from the hand
    /// </summary>
    /// <param name="value">The value of the card to remove</param>
    public void removeCard(int value)
    {
        if (!IsValidCard(value)) throw new InvalidOperationException("The given card value is outside the valid range");
        long bits = 1L << value;
        // throw an exception if the car is not present
        if (!hasCard(bits)) throw new InvalidOperationException("Card is not in hand");
        // make a filter allowing every card through but the one in question
        HandBits &= ~bits;
    }

    /// <summary>
    /// Checks if a card is in a hand
    /// </summary>
    /// <param name="value">The value of the card in the hand</param>
    /// <returns></returns>
    public bool hasCard(int value)
    {
        if (!IsValidCard(value)) throw new InvalidOperationException("The given card value is outside the valid range");
        return (HandBits & (1L << value)) != 0;
    }

    /// <summary>
    /// Checks if a card is in a hand
    /// </summary>
    /// <param name="value">The value of the card in the hand</param>
    /// <returns></returns>
    private bool hasCard(long bits)
    {
        return (HandBits & bits) != 0;
    }

    /// <summary>
    /// Checks if a given card is within the valid range
    /// </summary>
    /// <param name="value">The value of the card</param>
    /// <returns>Whether the card was valid</returns>
    private static bool IsValidCard(int value)
    {
        return value <= MAX_VALUE && value >= 0;
    }
}
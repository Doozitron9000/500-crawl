using _500_crawl.Models.Cards;

namespace _500_crawl.Models.Game;

/// <summary>
/// This is the shape of attacks sent from the client
/// </summary>
public class AI
{
    /// <summary>
    /// For now playing with the lead is very rudimentary and under developed
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="known"></param>
    /// <param name="trumps"></param>
    /// <returns></returns>
    public int playLead(long cards, long known, Suit trumps)
    {
        bool playerGroundless = (known & (1L << 42)) != 0;
        bool playerWaterless = (known & (1L << 43)) != 0;
        bool playerAirless = (known & (1L << 44)) != 0;
        bool playerArcaneless = (known & (1L << 45)) != 0;

        // check if the player has trumps
        bool playerHadTrumps = true;
        if (playerGroundless && trumps == Suit.Ground) playerHadTrumps = false;
        if (playerWaterless && trumps == Suit.Water) playerHadTrumps = false;
        if (playerAirless && trumps == Suit.Air) playerHadTrumps = false;
        if (playerArcaneless && trumps == Suit.Arcane) playerHadTrumps = false;

        bool hasDragon = (cards & (1L << 40)) != 0;

        // for now just play the highest card here
        if (playerHadTrumps && hasDragon) return 40;

        long firstTen = (1L << 10) - 1;
        bool groundless = (cards & firstTen) == 0;
        bool waterless = (cards & firstTen << 10) == 0;
        bool airless = (cards & firstTen << 20) == 0;
        bool arcaneless = (cards & firstTen << 30) == 0;

        
        try
        {
            int highestCard = -1;
            if(!groundless) highestCard = getHighestCardOfSuit(cards, Suit.Ground);
            if(!waterless) highestCard = Math.Max(getHighestCardOfSuit(cards, Suit.Water), highestCard);
            if(!airless) highestCard = Math.Max(getHighestCardOfSuit(cards, Suit.Air), highestCard);
            if(!arcaneless) highestCard = Math.Max(getHighestCardOfSuit(cards, Suit.Arcane), highestCard);
            return highestCard;
        }
        catch
        {
            // double check there are no possible cards to play
            for(int i = 0; i < 41; i++)
            {
                if((cards & (1L << i)) != 0) return i;
            }
            throw new InvalidOperationException("Failed to determine a card to play");
        }
        // double check there are no possible cards to play
        for(int i = 0; i < 41; i++)
        {
            if((cards & (1L << i)) != 0) return i;
        }
        throw new InvalidOperationException("Failed to determine a card to play");
        
    }

    public int playFollow(long cards, long known, int playerCard, Suit trumps)
    {
        // get a bitmask for the first 10 cards
        long firstTen = (1L << 10) - 1;

        bool groundless = (cards & firstTen) == 0;
        bool waterless = (cards & firstTen << 10) == 0;
        bool airless = (cards & firstTen << 20) == 0;
        bool arcaneless = (cards & firstTen << 30) == 0;

        bool hasDragon = (cards & (1L << 40)) != 0;

        bool hasTrumps = (cards & (firstTen << ((int)trumps)*10)) != 0;

        // check if we have any cards of the left card's suit
        int ledSuit = playerCard/10;
        try{
            if (ledSuit == 0)
            {
                if (groundless)
                {
                    if (trumps == Suit.Ground)
                    {
                        return pickNoTrumpsCard(hasDragon, cards);
                    }
                    else if (hasTrumps)
                    {
                        // play the lowest trump card
                        return getLowestCardofSuit(cards, trumps);
                    }
                }
                bool playDragon = trumps == Suit.Ground && hasDragon;
                return getBestCardOfSuit(cards, Suit.Ground, playerCard, playDragon);
            } 
            else if (ledSuit == 1)
            {
                if (waterless)
                {
                    if (trumps == Suit.Water)
                    {
                        return pickNoTrumpsCard(hasDragon, cards);
                    }
                    else if (hasTrumps)
                    {
                        // play the lowest trump card
                        return getLowestCardofSuit(cards, trumps);
                    }
                }
                bool playDragon = trumps == Suit.Water && hasDragon;
                return getBestCardOfSuit(cards, Suit.Water, playerCard, playDragon);
            }
            else if (ledSuit == 2)
            {
            if (airless)
                {
                    if (trumps == Suit.Air)
                    {
                        return pickNoTrumpsCard(hasDragon, cards);
                    }
                    else if (hasTrumps)
                    {
                        // play the lowest trump card
                        return getLowestCardofSuit(cards, trumps);
                    }
                }
                bool playDragon = trumps == Suit.Air && hasDragon;
                return getBestCardOfSuit(cards, Suit.Air, playerCard, playDragon);
            }
            else if (ledSuit == 3)
            {
                if (arcaneless)
                {
                    if (trumps == Suit.Arcane)
                    {
                        return pickNoTrumpsCard(hasDragon, cards);
                    }
                    else if (hasTrumps)
                    {
                        // play the lowest trump card
                        return getLowestCardofSuit(cards, trumps);
                    }
                }
                bool playDragon = trumps == Suit.Arcane && hasDragon;
                return getBestCardOfSuit(cards, Suit.Arcane, playerCard, playDragon);
            }
        }
        catch
        {
            // double check there are no possible cards to play
            for(int i = 0; i < 41; i++)
            {
                if((cards & (1L << i)) != 0) return i;
            }
            throw new InvalidOperationException("Failed to determine a card to play");
        }
        // double check there are no possible cards to play
        for(int i = 0; i < 41; i++)
        {
            if((cards & (1L << i)) != 0) return i;
        }
        throw new InvalidOperationException("Failed to determine a card to play");
    }

    private int pickNoTrumpsCard(bool hasDragon, long cards)
    {
        // if you have the dragon you have to play it since it is trumps
        if(hasDragon) return 40;
        // otherwise play your worst card
        int nextCard = 0;
        while (true)
        {
            if ((cards & (1L << nextCard)) != 0)
            {
                return nextCard;
            }
            nextCard += 10;
            if(nextCard >= 40) nextCard = nextCard % 10 +1;
        }
    }

    private int getLowestCardofSuit(long cards, Suit suit)
    {
        int suitInt = (int)suit * 10;
        for(int i = suitInt; i < suitInt+10; i++)
        {
            if((cards & (1L << i)) != 0) return i;
        }
        throw new InvalidOperationException("Failed to get lowest card of suit as no cards of that suit are present");
    }

    private int getHighestCardOfSuit(long cards, Suit suit)
    {
        int suitInt = (int)suit * 10;
        for(int i = suitInt+9; i >= suitInt; i--)
        {
            if((cards & (1L << i)) != 0) return i;
        }
        throw new InvalidOperationException("Failed to get highest card of suit as no cards of that suit are present");
    }

    /// <summary>
    /// If the given card can be beaten the lowest beating card is returned otherwise the worst card is returned.
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="suit"></param>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    private int getBestCardOfSuit(long cards, Suit suit, int targetValue, bool canPlayDragon)
    {
        int suitInt = (int)suit * 10;
        int currentBest = 100;
        int currentBestI = 0;
        for(int i = suitInt; i < suitInt+10; i++)
        {
            if((cards & (1L << i)) != 0)
            {   
                int iValue = i%10;
                if(iValue < currentBest)
                {
                    currentBest = iValue;
                    currentBestI = i;
                }
                if(iValue > targetValue%10) {
                    Console.WriteLine("b");
                    return i;
                }
            }
        }
        Console.WriteLine("a");
        // if we made it here we are going to lose so play the dragon if applicable
        if (canPlayDragon) return 40;
        if (currentBest == 100)
        {
            throw new InvalidOperationException("Failed to get best card of suit as no cards of that suit are present");
        }
        return currentBestI;
    }
}
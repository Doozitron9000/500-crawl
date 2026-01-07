namespace _500_crawl_tests;

using System.Security.Cryptography;
using _500_crawl.Models.Cards;

public class HandTest
{
    [Fact]
    public void TestHand()
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
        // theere should be no collisions between these two hands
        Assert.True((playerHand.HandBits & aiHand.HandBits) == 0);
    }
}

namespace _500_crawl_tests;

using System.Security.Cryptography;
using _500_crawl.Models.Cards;

public class HandTest
{
    // the max value a hand should even be able to be
    private const long MAX_VALUE = 2199023255551;
    private const int TEST_COUNT = 50;
    [Fact]
    public void TestHand()
    {
        // generate the seed to then seed the rng
        int seed = RandomNumberGenerator.GetInt32(int.MaxValue);

        Random rng = new Random(seed);
        for(int i = 0; i < TEST_COUNT; i++)
        {
            int deckSeed = rng.Next();
            Hand playerHand = new Hand();
            Hand aiHand = new Hand();
            Deck deck = new Deck(deckSeed);
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
            // and no bits past 41
            Assert.True(playerHand.HandBits < MAX_VALUE);
            Assert.True(aiHand.HandBits < MAX_VALUE);
        }
    }
}

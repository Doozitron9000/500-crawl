namespace _500_crawl_tests;

using _500_crawl.Models.Cards;

public class DeckTest
{
    [Fact]
    public void TestCardCount()
    {
        // make the deck and hash set so we can make sure there are no duplicate cards
        Deck deck = new Deck();
        HashSet<Card> cards = new HashSet<Card>();
        // now draw every card in the deck
        for(int i = 0; i < 53; i++)
        {
            Card card = deck.DrawCards(1)[0];
            // this card should always be unique
            Assert.True(cards.Add(card));
        }
        // if we draw another card we should get an exception
        Assert.Throws<InvalidOperationException>(() => deck.DrawCards(1));
    }
}

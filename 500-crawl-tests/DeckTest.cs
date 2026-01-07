namespace _500_crawl_tests;

using _500_crawl.Models.Cards;

public class DeckTest
{
    [Fact]
    public void TestCardCount()
    {
        // make the deck and hash set so we can make sure there are no duplicate cards
        Deck deck = new Deck(123);
        HashSet<int> cards = new HashSet<int>();
        // now draw every card in the deck
        for(int i = 0; i < 41; i++)
        {
            int card = deck.DrawCards(1)[0];
            // this card should always be unique
            Assert.True(cards.Add(card));
        }
        // if we draw another card we should get an exception
        Assert.Throws<InvalidOperationException>(() => deck.DrawCards(1));
    }
}

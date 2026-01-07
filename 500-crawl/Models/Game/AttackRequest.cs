using _500_crawl.Models.Cards;

namespace _500_crawl.Models.Game;

/// <summary>
/// This is the shape of attacks sent from the client
/// </summary>
public class AttackRequest
{
    public Suit Suit { get; set; }
    public int Target { get; set; }
}
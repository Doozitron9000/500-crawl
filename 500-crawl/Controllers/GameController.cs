using Microsoft.AspNetCore.Mvc;
using _500_crawl.Models;
using _500_crawl.Models.Cards;

namespace _500_crawl.Controllers;

public class GameController : Controller
{
    public IActionResult DrawCard()
    {
        Deck deck = new Deck();
        Card card =  deck.DrawCards(1)[0];
        return Ok(card);
    }
}
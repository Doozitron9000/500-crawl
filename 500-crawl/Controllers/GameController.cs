using Microsoft.AspNetCore.Mvc;
using _500_crawl.Authentication;
using _500_crawl.Models.Game;
using _500_crawl.Models.Cards;
using System.Runtime.InteropServices;

namespace _500_crawl.Controllers;

public class GameController : Controller
{

    // the database to which new users are sent and against which users are validated
    private readonly SiteDbContext database;

    public GameController(SiteDbContext database)
    {
        this.database = database;
    }

    public IActionResult Index()
    {
        return View(GetGame());
    }

    [HttpPost]
    public IActionResult PlayCard([FromBody] PlayCardRequest request)
    {
        
    }

    [HttpPost]
    public IActionResult Kitty([FromBody] KittyRequest request)
    {
        // reload the session
        GameSession session = new GameSession(database);
        // we need to get the current user's game if it exists and assign it to a new
        // game session
        int gameId = HttpContext.Session.GetInt32("GameID")!.Value;
        session.loadGame(gameId);
        long kittyBits = getKittyBits(session);
        long returnedCards = long.Parse(request.Cards);
        // we need to make sure the cards we got back are all legitly possible
        if((returnedCards & (kittyBits | session.State.PlayerHand)) != returnedCards)
        {
            // if we are here the cards we got back aren't legit so return the previous valid state
            ClientGameState visibleState = session.getVisibleState();
            visibleState.Kitty = getKittyBits(session);
            return Ok(visibleState);
        }
        // get the new player hand
        long newHand = session.State.PlayerHand ^ returnedCards;
        // now make sure there are exactly to "1s" in the player hand long
        if (System.Numerics.BitOperations.PopCount((ulong)newHand) != 10)
        {
            // if we are here we have the wrong number of cards so just return the last known valid state
            ClientGameState visibleState = session.getVisibleState();
            visibleState.Kitty = getKittyBits(session);
            return Ok(visibleState);
        }
        // if we made it here the new hand is legit so update the game state including drawing the kitty from the deck
        session.State.PlayerHand = newHand;
        session.State.Phase = GamePhase.Attacking;
        session.saveGame();
        return Ok(session.getVisibleState());
    }

    [HttpPost]
    public IActionResult Attack([FromBody] AttackRequest request)
    {
        // reload the session
        GameSession session = new GameSession(database);
        // we need to get the current user's game if it exists and assign it to a new
        // game session
        int gameId = HttpContext.Session.GetInt32("GameID")!.Value;
        session.loadGame(gameId);
        // now update the session state with the user provided info after a quick validation
        if (request.Target > 10 || request.Target < 6 || !Enum.IsDefined(typeof(Suit), request.Suit))
        {
            // for now just return the old game state here in okay so nothing changes since we got an invalid input
            return Ok(session.getVisibleState());
        } 
        session.State!.Trumps = request.Suit;
        session.State!.RoundTarget = request.Target;
        session.State!.Phase = GamePhase.Preparing;
        session.saveGame();

        ClientGameState visibleState = session.getVisibleState();
        visibleState.Kitty = getKittyBits(session);
        return Ok(visibleState);
    }


    /// <summary>
    /// Loads the game or starts a new one if one doesnt already exist
    /// </summary>
    /// <returns>The action result</returns>
    private ClientGameState GetGame()
    {
        // we need to get the current user's game if it exists and assign it to a new
        // game session
        int userID = HttpContext.Session.GetInt32("UserID")!.Value;
        User user = database.Users.Single(u => u.Id == userID);

        // make the session to load the game into or create it from
        GameSession session =  new GameSession(database);

        if (user.GameId.HasValue)
        {
            session.loadGame(user.GameId.Value);
            HttpContext.Session.SetInt32("GameID", user.GameId.Value);
        }
        else
        {
            // create new game
            session.newGame();
            user.GameId = session.State!.Id;
            database.SaveChanges();
            HttpContext.Session.SetInt32("GameID", session.State!.Id);
        }
         // remake the deck so we can draw from it
        Deck deck = new Deck(session.State!.DeckSeed, session.State!.DeckPlace);
        // get the kitty
        Hand kitty = new Hand();
        int[] kittyCards = deck.DrawCards(3);
        foreach (int card in kittyCards)
        {
            kitty.addCard(card);
        }
        ClientGameState visibleState = session.getVisibleState();
        visibleState.Kitty = kitty.HandBits;
        return visibleState;
    }

    private long getKittyBits(GameSession session)
    {
        // remake the deck so we can draw from it
        Deck deck = new Deck(session.State!.DeckSeed, session.State!.DeckPlace);
        // get the kitty
        Hand kitty = new Hand();
        int[] kittyCards = deck.DrawCards(3);
        foreach (int card in kittyCards)
        {
            kitty.addCard(card);
        }
        return kitty.HandBits;
    }
}
//========================= Handle menu =============================//
let pendingMenuAction = null;

const confirmModal = document.getElementById("confirm-box");
const confirmTitle = document.getElementById("confirm-title");
const confirmMessage = document.getElementById("confirm-message");
const confirmYes = document.getElementById("confirm-yes");

confirmModal.addEventListener("show.bs.modal", event => {
    // get whatever triggered the modal box popping up and get ther elevant data from it
    const sender = event.relatedTarget;
    confirmTitle.textContent = sender.getAttribute("data-confirm-title");
    confirmMessage.textContent = sender.getAttribute("data-confirm-message");
    pendingMenuAction = sender.getAttribute("data-confirm-action");
});

// when the user confirms a menu action via the modal box run the relevant command
confirmYes.addEventListener("click", () => {
    if (pendingMenuAction) {
        window.location.href = pendingMenuAction;
    }
});

//========================= Game State =============================//
let serverGameState = receivedGameState;
// the game state "enum"
const GameState = {
    DECIDING: "deciding",
    PREPARING: "preparing",
    PLAYING: "playing"
};

const Suits = {
    GROUND: "Ground",
    WATER: "Water",
    AIR: "Air",
    ARCANE: "Arcane",
    DRAGON: "Dragon"
};

let defending = false;
let leading = true;
let trumps = Suits.GROUND;
let roundTarget = 6;
let selectedCards = new Set()
let currentState = GameState.DECIDING;
let kittyValues = new Set()
document.documentElement.dataset.state = currentState;

function changeState(newState){
    // if we are already in the suggested state jsut return
    if (currentState === newState) return;
    // otherwise, update the game state
    currentState = newState;
    document.documentElement.dataset.state = newState;
}

/*
Below is what the game phase looks like to the server so we need to interpret that we we render
public enum GamePhase
{
    Deciding,
    Preparing,
    AttackingGround,
    AttackingWater,
    AttackingAir,
    AttackingArcane,
    AttackingEverywhere,
    DefendingGround,
    DefendingWater,
    DefendingAir,
    DefendingArcane,
    DefendingEverywhere
}
*/
function updateGame(){
    selectedCards.clear();
    const phase = receivedGameState.phase;
     switch (phase){
        case 0:
            currentState = GameState.DECIDING;
            break;
        case 1:
            currentState = GameState.PREPARING;
            break;
        case 2:
            currentState = GameState.PLAYING;
            defending = false;
            break;
        case 3:
            currentState = GameState.PLAYING;
            defending = true;
            break;
        default:
            throw new Error("Tried to derive game state from invalid value");
    }
    document.documentElement.dataset.state = currentState;
    // the card area
    const cardList = document.getElementById("card-list");
    // clear the current children of teh card list
    // the list of interpreted cards
    cardList.replaceChildren();
    let cards = longToCards(receivedGameState.playerHand);
    cards.forEach(card => {
        // make the new card from teh template
        cardList.appendChild(cardToFragment(card));
    })

    // if we aren't deciding we should display trumps and target
    if (currentState !== GameState.DECIDING){
        trumps = receivedGameState.trumps;
        roundTarget = receivedGameState.roundTarget;
    }
    if (currentState === GameState.PREPARING){
        const kitty = receivedGameState.kitty;
        const kittyList = document.getElementById("kitty-list");
        kittyList.replaceChildren();

        let kittyCards = longToCards(kitty);
        kittyCards.forEach(card => {
            kittyValues.add(Number(card.value));
            // make the new card from teh template
            kittyList.appendChild(cardToFragment(card));
        });
    }
    // now sort leading, trumps, and all teh rest of it
    leading = receivedGameState.leading;

    // get the opponents card
    const opponentCardValue = receivedGameState.aiCard;
    const opponentValue = opponentCardValue%10 + 1;
    const opponentSuitValue = Math.floor(opponentCardValue/10);
    const opponentSuit = Object.values(Suits)[opponentSuitValue];
    
    const opponentCard = document.getElementById("opponent-card");
    const opponentSuitArea = document.getElementById("opponent-card-s");
    const opponentValueArea = document.getElementById("opponent-card-v");
    opponentValueArea.textContent = opponentValue;
    opponentSuitArea.textContent = opponentSuit;

    opponentCard.dataset.suit = opponentSuit;
    opponentCard.dataset.value = opponentValue+1;
    
    // post the player wins and loss text
    const winField = document.getElementById("player-wins");
    winField.textContent = receivedGameState.wonHands;
    const lossField = document.getElementById("ai-wins");
    lossField.textContent = receivedGameState.lostHands;
    const phpField = document.getElementById("player-health");
    phpField.textContent = receivedGameState.playerHealth;
    const aihpField = document.getElementById("ai-health");
    aihpField.textContent = receivedGameState.aiHealth;
    const trumpText = Object.values(Suits)[receivedGameState.trumps];
    const trumpField = document.getElementById("trumps");
    trumpField.textContent = trumpText
    const targetScore = document.getElementById("target-score");
    targetScore.textContent = receivedGameState.roundTarget;
}

// update game should always fire once on load
updateGame();

// ====================== Card Interpretation ======================//
function longToCards(long){
    // cast the long string as a big int
    const bits = BigInt(long)
    // make the card array
    const cards = [];
    // loop through every bit to see which cards we have
    for (let i = 0; i <= 40; i++){
        // use bitwise and and the bit in question to see if it is present
        // i.e. is a non 0 value
        if ((bits & (1n << BigInt(i))) !== 0n) {
            // now we have a card value interpret it
            cards.push(cardFromValue(i))
        }
    }
    return cards;
}

/**
 * Interprets a card value int into a card object
 * @param {The numerical value of the card pre processing} value 
 * @returns The card object
 */
function cardFromValue(value) {
    // now we have a card value interpret it
    let cardValue = (value % 10) + 1;
    const suitValue = Math.floor(value / 10);
    let suit;

    switch (suitValue){
        case 0:
            suit = Suits.GROUND;
            break;
        case 1:
            suit = Suits.WATER;
            break;
        case 2:
            suit = Suits.AIR;
            break;
        case 3:
            suit = Suits.ARCANE;
            break;
        case 4:
            suit = Suits.DRAGON;
            cardValue = 99;
            break;
        default:
            throw new Error("Tried to derive card from invalid value");
    }

    return {value, suit, cardValue}
}

function cardToFragment(card){
    // the card template
    const cardTemplate = document.getElementById("card-template");
    // make the new card from teh template
    const newCard = cardTemplate.content.cloneNode(true);
    const suitArea = newCard.querySelector(".card-suit");
    suitArea.textContent = card.suit;

    const valueArea = newCard.querySelector(".card-value");
    valueArea.textContent = card.cardValue;

    // now give the card its stats as attributes so it knows how to style itself
    const cardElement = newCard.querySelector(".card");
    cardElement.dataset.suit = card.suit;
    cardElement.dataset.value = card.value;

    // make the card's listener
    cardElement.addEventListener("click", async () => {
        const value = Number(cardElement.dataset.value);
        if (currentState === GameState.PLAYING){
            // just remove selected from every card
            document.querySelectorAll(".card.selected").forEach(el => {
                el.classList.remove("selected");
            });
            selectedCards.clear();
            selectedCards.add(value);
            cardElement.classList.add("selected");
            playCard(value);
        } else {
            // when this card is clicked check if it is in the selected cards list
            if(selectedCards.has(value)){
                selectedCards.delete(value)
                cardElement.classList.remove("selected");
            } else {
                selectedCards.add(value);
                cardElement.classList.add("selected");
            }
        }
    });

    return newCard;
}

//==========================Decision==========================//
document.getElementById("attack-form").addEventListener("submit", async (e) => {
    e.preventDefault();

    // get the suit and the number of hands targetted to win
    const suit = document.querySelector('input[name="suit"]:checked').value;
    const target = document.querySelector('input[name="hands"]:checked').value;

    // now send this to the server and get the new game state from it
    const response = await fetch("/Game/Attack", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            suit: Number(suit),
            target: Number(target)
        })
    });

    const newGameState = await response.json();
    // now we have the new game state we can update the game and re-render it
    receivedGameState = newGameState;
    updateGame();
})
//==========================Preparing==========================//
document.getElementById("kitty_select").addEventListener("click", async (e) => {
    e.preventDefault();

    // make the long to return
    let long = 0n;

    // now use a bitwise add to add every selected card to this
    selectedCards.forEach(selected => {
        long |= 1n << BigInt(selected)
    });

    // now send this to the server and get the new game state from it
    const response = await fetch("/Game/Kitty", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            cards: long.toString(),
        })
    });

    const newGameState = await response.json();
    // now we have the new game state we can update the game and re-render it
    receivedGameState = newGameState;
    updateGame();
})
//==========================Playing==========================//
async function playCard(value) {
    // now send this to the server and get the new game state from it
    const response = await fetch("/Game/PlayCard", {
        
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            card: Number(value),
        })
    });
    
    const newGameState = await response.json();
    // now we have the new game state we can update the game and re-render it
    receivedGameState = newGameState;
    updateGame();
}
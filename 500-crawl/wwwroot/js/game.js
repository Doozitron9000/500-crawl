// set up the temp button logic.
document.getElementById("drawBtn").addEventListener("click", async () => {
    const response = await fetch("/Game/DrawCard")

    // make sure we get ok back
    if (!response.ok){
        console.error("Draw card failed");
        return;
    }

    const drawnCard = await response.json();

    // get the card's value and type
    const value = drawnCard.value;
    const suit = drawnCard.suit;

    // get the card area and make the new card
    const cardArea = document.getElementById("cardList");
    const newCard = document.createElement("li");

    // Make arrays of suits and values to make them easier to get later
    const suits = ["Hearts", "Diamonds", "Clubs", "Spades"];
    const values = ["2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace"];

    // first check if this is the joker which is the 5th suit
    if(suit == 4){
        newCard.textContent = "Joker";
    } else {
        newCard.textContent = `${values[value]} of ${suits[suit]}`;
    }

    console.log(drawnCard)
    cardArea.append(newCard);
})
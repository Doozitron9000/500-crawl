// set up the temp button logic.
document.getElementById("drawBtn").addEventListener("click", async () => {
    const response = await fetch("/Game/DrawCard")

    // make sure we get ok back
    if (!response.ok){
        console.error("Draw card failed");
        return;
    }

    const card = await response.json();
    console.log(card);
})
// we need to intercept login attempts in js and validate stuff before letting them continue
// atm we only need to make sure both fields are full.... needs to be async since we will away a response
document.getElementById("login-form").addEventListener("submit", async (e) => {
    e.preventDefault();

    // get and trim the username and get the password
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value;
    
    // get the error message field
    const error = document.querySelector(".error");

    // if either of these are empty you need to return an error message
    if (!username || !password) {
        error.textContent = "Please fill out both fields"
        return;
    }

    // make error field blank so if things take a second the user can see we made it past this
    error.textContent = ""

    // now fetch from the server
    const result = await fetch("/Login/Login", {
        method: "POST",
        // probably unnecessary but just confirm we send html
        headers: { "Content-Type": "application/x-www-form-urlencoded"},
        // send everything in the form with a name attached
        body: new URLSearchParams(new FormData(e.target))
    });

    // see if we were return ok and if not print the text of the result in the error field
    if (!result.ok) {
        error.textContent = await result.text();
        return;
    }

    // if we made it here the signup was a success so navigate to the login page
    window.location.href = "/Home/Index";
})
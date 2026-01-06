// we need to intercept login attempts in js and validate stuff before letting them continue
// atm we only need to make sure both fields are full
document.getElementById("login-form").addEventListener("submit", (e) => {
    e.preventDefault();

    // get and trim the username and get the password
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value;
    
    // get the error message
    const error = document.querySelector(".error");

    // if either of these are empty you need to return an error message
    if (!username || !password) {
        error.textContent = "Please fill out both fields"
        return;
    }

    // make error field blank so if things take a second the user can see we made it past this
    error.textContent = ""

    // if we made it here this is a valid login attempt so send it
    e.target.submit();
})
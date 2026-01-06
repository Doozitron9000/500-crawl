// we need to intercept login attempts in js and validate stuff before letting them continue
// atm we only need to make sure both fields are full
document.getElementById("signup-form").addEventListener("submit", (e) => {
    e.preventDefault();

    // get and trim the username and get the passwords
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value;
    const confirmedPassword = document.getElementById("conf-password").value;
    
    // get the error message
    const error = document.querySelector(".error");

    // if any of these are empty return an error message
    if (!username || !password || !confirmedPassword) {
        error.textContent = "Please fill out all fields"
        return;
    }

    // now make sure both passwords are identical
    if (password !== confirmedPassword) {
        error.textContent = "Passwords must match"
        return;
    }

    // make error field blank so if things take a second the user can see we made it past this
    error.textContent = ""

    // if we made it here this is a valid login attempt so send it
    e.target.submit();
})
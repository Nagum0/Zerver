const btn = document.getElementById("btn")

btn.addEventListener("click", () => {
    fetch("http://localhost:5000/fur", {
        method: "POST",
        headers: {
            "Content-Type": "text/plain"
        },
        body: "1234"
    })
    .then(res => {
        if (!res.ok) {
            throw new Error('Network response was not ok')
        }

        if (res.redirected) {
            console.log(res.url)
            window.location.href = res.url
        }

        return res.text()
    })
    .then(data => {
        console.log(data)
    })
    .catch(e => {
        console.error(e)
    })
})
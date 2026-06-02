// AJAX: send an async request to the Web API and render the JSON
// response into the page without reloading. Satisfies the AJAX +
// Web API requirement.
document.getElementById("loadBooks").addEventListener("click", async () => {
    const list = document.getElementById("apiBooks");
    list.innerHTML = "<li class='list-group-item'>Loading...</li>";

    try {
        const response = await fetch("/api/books");
        if (!response.ok) {
            throw new Error("Request failed: " + response.status);
        }
        const books = await response.json();

        list.innerHTML = "";
        books.forEach(b => {
            const li = document.createElement("li");
            li.className = "list-group-item";
            // textContent (not innerHTML) avoids XSS from data values.
            li.textContent = `${b.title} by ${b.authorName} (${b.year})`;
            list.appendChild(li);
        });
    } catch (err) {
        list.innerHTML = "<li class='list-group-item text-danger'>Failed to load.</li>";
    }
});

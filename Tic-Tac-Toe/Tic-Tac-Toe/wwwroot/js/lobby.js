document.getElementById("createGame").addEventListener("click", () => {
    const modal = document.createElement("div");
    modal.className = "modal-overlay";

    const form = document.createElement("form");
    form.className = "modal-content";
    form.innerHTML = `
        <label>
            Размер поля:
            <input type="number" id="boardSize" min="3" max="10" value="3" required>
        </label>
        <button type="submit">Создать</button>
        <button type="button" id="cancelModal">Отмена</button>
    `;

    modal.appendChild(form);
    document.body.appendChild(modal);

    document.getElementById("cancelModal").addEventListener("click", () => {
        document.body.removeChild(modal);
    });

    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const size = parseInt(document.getElementById("boardSize").value, 10);

        try {
            const res = await fetch("/games", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ size })
            });

            if (res.ok) {
                const data = await res.json();
                location.href = `/game/${data.gameId}`;
            } else {
                alert("Ошибка при создании игры");
            }
        } catch (err) {
            console.error(err);
            alert("Произошла ошибка при отправке запроса");
        }
    });
});

document.getElementById("logoutBtn")?.addEventListener("click", async () => {
    const res = await fetch("/logout");
    if (res.redirected) {
        window.location.href = res.url;
    } else {
        alert("Logout failed");
    }
});

async function loadGames() {
    const res = await fetch("/games");
    const list = document.getElementById("gameList");
    list.innerHTML = "";
    const games = await res.json();
    games.forEach(game => {
        const li = document.createElement("li");
        li.textContent = `Game ${game.gameId}`;
        li.onclick = async () => {
            await fetch(`/games/${game.gameId}/join`, { method: "POST" });
            location.href = `/game/${game.gameId}`;
        };
        list.appendChild(li);
    });
}

loadGames();

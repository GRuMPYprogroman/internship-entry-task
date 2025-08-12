const gameId = location.pathname.split("/").pop();
const board = document.getElementById("gameBoard");
const status = document.getElementById("status");

let isMyTurn = false;
let myUserId;

async function fetchUserId() {
    const res = await fetch('/profile');
    if (res.ok) {
        const data = await res.json();
        myUserId = data.userId;
    } else {
        console.error("Не удалось получить userId");
    }
}

function renderBoard(cells) {
    // Определяем размер поля
    const size = Math.sqrt(cells.length);
    board.style.gridTemplateColumns = `repeat(${size}, 60px)`;
    board.style.gridTemplateRows = `repeat(${size}, 60px)`;

    board.innerHTML = "";
    cells.forEach((cell, i) => {
        const div = document.createElement("div");
        div.textContent = cell || "";
        div.onclick = async () => {
            if (!div.textContent && isMyTurn) {
                await fetch(`/games/${gameId}/moves`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ index: i })
                });
                await loadGame();
            }
        };
        board.appendChild(div);
    });
}

async function loadGame() {
    try {
        const res = await fetch(`/game/${gameId}`);
        if (!res.ok) throw new Error(`Ошибка загрузки игры: ${res.status}`);
        const data = await res.json();

        renderBoard(data.board);
        status.textContent = data.status || "Waiting for game to start"

        isMyTurn = data.currentTurn === myUserId;
    } catch (err) {
        console.error(err);
        status.textContent = "Ошибка загрузки игры";
    }
}

async function init() {
    await fetchUserId();
    await loadGame();
    setInterval(() => loadGame(), 2000);
}

// Первичная загрузка и обновление каждые 2 сек
init();

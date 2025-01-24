const chatWindow = document.getElementById("chat-window");

const sendButton = document.getElementById("send-button");
const messageInput = document.getElementById("message-input");
const messageInputContainer = document.getElementById("message-input-container");

// Ajusta la altura del textarea dinámicamente
messageInput.addEventListener("input", function () {

    // Reseta la altura a 'auto' para que pueda crecer
    messageInput.style.height = 'auto';

    // Setea la altura para matchear la altura del contenido
    messageInput.style.height = messageInput.scrollHeight + 'px';

    if (messageInput.value.length > 0) {
        sendButton.disabled = false;
    } else {
        sendButton.disabled = true;
    }
});

let chatId = null;

// Genera un nuevo chat cada vez que se carga la pagina
window.onload = function () {
    chatId = crypto.randomUUID();
};

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (chatId, message) {
    const newMessageContainer = document.createElement("div");
    const newMessage = document.createElement("div");
    const newMessageIconBorder = document.createElement("div");
    const newMessageIcon = document.createElement("img");
    const loadingIndicatorContainer = document.getElementById("loading-indicator-container");

    newMessage.textContent = message;
    newMessage.classList.add('bot-message');

    newMessageIcon.classList.add('icon');
    newMessageIcon.src = "CZ.png";

    newMessageIconBorder.classList.add('icon-border');
    newMessageIconBorder.appendChild(newMessageIcon);

    newMessageContainer.appendChild(newMessageIconBorder);
    newMessageContainer.appendChild(newMessage);

    chatWindow.removeChild(loadingIndicatorContainer);
    chatWindow.appendChild(newMessageContainer)
    chatWindow.scrollTop = chatWindow.scrollHeight;

    messageInputContainer.style.display = 'flex';
});

connection.start().catch(err => console.error(err.toString()));

sendButton.addEventListener("click", async () => {
    const text = messageInput.value;

    // Clear input field after sending
    messageInput.value = '';

    // Reseta la altura a 'auto' para que pueda crecer
    messageInput.style.height = 'auto';

    messageInputContainer.style.display = 'none';

    // Display message in chat window with styling for the user's message
    const userMessage = document.createElement("div");
    userMessage.textContent = text;
    userMessage.classList.add('user-message');
    chatWindow.appendChild(userMessage);

    // Auto-scroll to bottom
    chatWindow.scrollTop = chatWindow.scrollHeight;

    connection.invoke("SendMessageAsync", chatId, text);

    // Indicador de carga
    const newMessageContainer = document.createElement("div");
    const loadingIndicator = document.createElement("div");
    const newMessageIconBorder = document.createElement("div");
    const newMessageIcon = document.createElement("img");

    loadingIndicator.classList.add('loader');

    newMessageIcon.classList.add('icon');
    newMessageIcon.src = "CZ.png";

    newMessageIconBorder.classList.add('icon-border');
    newMessageIconBorder.appendChild(newMessageIcon);

    newMessageContainer.appendChild(newMessageIconBorder);
    newMessageContainer.appendChild(loadingIndicator);
    newMessageContainer.id = "loading-indicator-container";

    chatWindow.appendChild(newMessageContainer)
    chatWindow.scrollTop = chatWindow.scrollHeight;
});

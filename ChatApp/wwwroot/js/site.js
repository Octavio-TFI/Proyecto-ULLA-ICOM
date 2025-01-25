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

    // Habilita el botón de enviar si hay texto en el input
    if (messageInput.value.length > 0) {
        sendButton.disabled = false;
    } else {
        sendButton.disabled = true;
    }
});

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (text) {
    // Muestra el mensaje del bot en el chat
    const newMessageContainer = document.createElement("div");
    const newMessage = document.createElement("div");
    const newMessageIconBorder = document.createElement("div");
    const newMessageIcon = document.createElement("img");
    const loadingIndicatorContainer = document.getElementById("loading-indicator-container");

    newMessage.textContent = text;
    newMessage.classList.add('bot-message');
    newMessage.style = 'white-space: pre-line;'

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

    // Limpia el input al enviar mensaje
    messageInput.value = '';

    // Reseta la altura a 'auto' para que pueda crecer
    messageInput.style.height = 'auto';

    // Oculta el input de mensaje mientras se envía el mensaje
    messageInputContainer.style.display = 'none';

    // Muestra el mensaje del usuario en el chat
    const userMessage = document.createElement("div");
    userMessage.textContent = text;
    userMessage.classList.add('user-message');
    userMessage.style = 'white-space: pre-line;'
    chatWindow.appendChild(userMessage);

    // Hace scroll al final del chat
    chatWindow.scrollTop = chatWindow.scrollHeight;

    // Envía el mensaje al servidor
    connection.invoke("SendMessageAsync", text);

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

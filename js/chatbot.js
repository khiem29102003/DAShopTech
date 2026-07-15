document.getElementById("chatbot-send").addEventListener("click", function () {
    var userInput = document.getElementById("chatbot-input").value;

    fetch('/ChatBot/GetResponse', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userInput)
    })
        .then(response => response.json())
        .then(data => {
            var messageContainer = document.getElementById("chatbot-messages");
            messageContainer.innerHTML += `<div class="bot-message">${data.response}</div>`;
        });
});

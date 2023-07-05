"use strict";
// Create connection
// Basically it's a connection string that build a connection
// The url comes from Program.cs file maphub
let chatConnection = new signalR.HubConnectionBuilder().withUrl("/Chat").build();


// Connect to methods that hub invokes receive notifications from hub
chatConnection.on("ReceiveMessages", addMessageToConversation);

// Connect to methods that hub invokes group send method from hub
//chatConnection.on("ReceiveGroupMessages", addMessageToGroupConversation);
chatConnection.on("ReceiveGroupMessages", addMessageToConversation);

// Connect to methods that hub invokes receive chat notifications from hub
chatConnection.on("ReceiveConNotify", userConnectivity);

// Connect to methods that hub invokes receive notifications from hub
chatConnection.on("ReceiveNotifications", getNotifications);

// Invoke the hub methods send notification to hub
function sendMessageToUser(conversation) {
    chatConnection.invoke("SendToUserAsync", conversation)
        .catch((err) => {
            return console.error(err.toString());
        });
};

// Start connection
function fulfilled() {
    console.log("User successful");
    
}

function rejected() {

}

chatConnection.start().then(fulfilled, rejected);

//document.getElementById("sendMsgBtn").addEventListener("click", function (event) {
//    sendNotificationToUser();
//    event.preventDefault();
//});
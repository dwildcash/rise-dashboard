﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/Hub/notificationHub").build();

connection.on("Send", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = msg;
        var li = document.createElement("li");
        li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);

    var element = document.getElementById("messagesList");
    element.scrollTop = element.scrollHeight - element.clientHeight;
});

connection.start().then(function () {
    var li = document.createElement("li");
    li.textContent = "Connected to server...";
    document.getElementById("messagesList").appendChild(li);
}).catch(function (err) {
    return console.error(err.toString());
});
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/Hub/notificationHub").build();

connection.on("Send", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var element = document.getElementById("master");
    element.innerHTML += msg + '<br>';
    element.scrollTop = element.scrollHeight - element.clientHeight;
});

connection.start().then(function () {
    var element = document.getElementById("master");
    element.innerHTML += "<p class='success'>Connected to server</p>";
}).catch(function (err) {
    return console.error(err.toString());
});

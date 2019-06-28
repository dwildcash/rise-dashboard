"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/Hub/notificationHub").build();

connection.on("ShowForged", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    document.getElementById("mainHeader").innerHTML = msg;
});

connection.start().then(function () {
    document.getElementById("mainHeader").innerHTML = 'Connected to rise.coinquote node feed';
}).catch(function (err) {
    return console.error(err.toString());
});
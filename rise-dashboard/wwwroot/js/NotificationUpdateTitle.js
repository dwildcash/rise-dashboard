"use strict";

const sleep = (milliseconds) => {
    return new Promise(resolve => setTimeout(resolve, milliseconds));
};

var connection = new signalR.HubConnectionBuilder().withUrl("/Hub/notificationHub").build();

connection.on("ShowForged", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    document.getElementById("mainHeader").innerHTML = msg;
});

"use strict";

const sleep = (milliseconds) => {
    return new Promise(resolve => setTimeout(resolve, milliseconds));
};

var connection = new signalR.HubConnectionBuilder().withUrl("/Hub/notificationHub").build();

connection.on("Send", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var element = document.getElementById("master");
    element.innerHTML += msg + '<br>';
    element.scrollTop = element.scrollHeight - element.clientHeight;
});

connection.on("ShowForged", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var element = document.getElementById("MasterHeader");
    element.innerHTML = '<p>' + msg + '</p>';
});

connection.start().then(function () {
    var element = document.getElementById("master");
    element.innerHTML += "<p class='text-success'>Connected to server</p>";

    sleep(1000).then(() => {
        element.innerHTML += "<p class='text-success'>Waiting for next log...</p>";
    });
}).catch(function (err) {
    return console.error(err.toString());
});
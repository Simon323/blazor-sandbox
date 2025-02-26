const express = require('express');
const http = require('http');
const signalR = require('@microsoft/signalr');
const signalRServer = require('@microsoft/signalr-protocol-msgpack');

const app = express();
const server = http.createServer(app);
const port = 3000;

// Tworzenie serwera SignalR
const hub = new signalR.HubConnectionBuilder()
    .withUrl(`http://localhost:${port}/currencyHub`)
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Middleware SignalR
app.use('/currencyHub', (req, res) => {
    hub.start()
        .then(() => res.send('SignalR Hub started'))
        .catch(err => res.status(500).send(err.toString()));
});

// Obsługa połączeń
hub.on('connection', (connection) => {
    console.log('Client connected');

    // Przykładowe dane walut
    const currencies = [
        { name: 'USD', buyPrice: 3.80, sellPrice: 3.90 },
        { name: 'EUR', buyPrice: 4.50, sellPrice: 4.60 },
        // Dodaj więcej walut według potrzeb
    ];

    // Wysyłanie danych do klienta co 5 sekund
    const intervalId = setInterval(() => {
        connection.send('ReceiveCurrencyUpdates', currencies);
    }, 5000);

    connection.on('close', () => {
        clearInterval(intervalId);
        console.log('Client disconnected');
    });
});

server.listen(port, () => {
    console.log(`Server is running on http://localhost:${port}`);
    console.log(`SignalR Hub is running on http://localhost:${port}/currencyHub`);
});

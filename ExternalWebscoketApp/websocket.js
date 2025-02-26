const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 8080 });

// Funkcja generująca losowe kursy walut
const generateRandomRates = () => {
    return [
        { name: 'USD', buyPrice: (3.70 + Math.random() * 0.20).toFixed(2), sellPrice: (3.80 + Math.random() * 0.20).toFixed(2) },
        { name: 'EUR', buyPrice: (4.40 + Math.random() * 0.20).toFixed(2), sellPrice: (4.50 + Math.random() * 0.20).toFixed(2) },
        { name: 'GBP', buyPrice: (5.00 + Math.random() * 0.20).toFixed(2), sellPrice: (5.10 + Math.random() * 0.20).toFixed(2) },
        { name: 'JPY', buyPrice: (0.03 + Math.random() * 0.01).toFixed(4), sellPrice: (0.04 + Math.random() * 0.01).toFixed(4) },
        { name: 'CHF', buyPrice: (4.00 + Math.random() * 0.20).toFixed(2), sellPrice: (4.10 + Math.random() * 0.20).toFixed(2) },
        { name: 'AUD', buyPrice: (2.70 + Math.random() * 0.20).toFixed(2), sellPrice: (2.80 + Math.random() * 0.20).toFixed(2) },
        { name: 'CAD', buyPrice: (3.00 + Math.random() * 0.20).toFixed(2), sellPrice: (3.10 + Math.random() * 0.20).toFixed(2) },
        // Dodaj więcej walut według potrzeb
    ];
};

// Funkcja wysyłająca aktualne kursy walut do wszystkich klientów
const broadcastCurrencyRates = () => {
    const allCurrencies = generateRandomRates();
    wss.clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            const requestedCurrencies = client.requestedCurrencies || [];
            const currencies = requestedCurrencies.length > 0
                ? allCurrencies.filter(currency => requestedCurrencies.includes(currency.name))
                : allCurrencies;
            client.send(JSON.stringify(currencies));
        }
    });
};

// Wysyłaj kursy walut co 5 sekund
setInterval(broadcastCurrencyRates, 5000);

wss.on('connection', (ws) => {
    console.log('Client connected');
    console.log(`Current number of clients: ${wss.clients.size}`);

    // Wysyłaj aktualne kursy walut natychmiast po połączeniu
    ws.send(JSON.stringify(generateRandomRates()));

    ws.on('message', (message) => {
        const requestedCurrencies = JSON.parse(message);
        console.log('Requested currencies:', requestedCurrencies);
        if (requestedCurrencies.length == 1 && requestedCurrencies[0] == '') {
            ws.requestedCurrencies = [];
        } else {
            ws.requestedCurrencies = requestedCurrencies;
        }
        broadcastCurrencyRates();
    });

    ws.on('close', () => {
        console.log('Client disconnected');
        console.log(`Current number of clients: ${wss.clients.size}`);
    });
});

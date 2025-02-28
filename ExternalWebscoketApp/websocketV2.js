const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 8080 });

// Function generating random currency rates
const generateRandomRates = () => {
    return [
        { name: 'USD', buyPrice: (3.70 + Math.random() * 0.20).toFixed(2), sellPrice: (3.80 + Math.random() * 0.20).toFixed(2) },
        { name: 'EUR', buyPrice: (4.40 + Math.random() * 0.20).toFixed(2), sellPrice: (4.50 + Math.random() * 0.20).toFixed(2) },
        { name: 'GBP', buyPrice: (5.00 + Math.random() * 0.20).toFixed(2), sellPrice: (5.10 + Math.random() * 0.20).toFixed(2) },
        { name: 'JPY', buyPrice: (0.03 + Math.random() * 0.01).toFixed(4), sellPrice: (0.04 + Math.random() * 0.01).toFixed(4) },
        { name: 'CHF', buyPrice: (4.00 + Math.random() * 0.20).toFixed(2), sellPrice: (4.10 + Math.random() * 0.20).toFixed(2) },
        { name: 'AUD', buyPrice: (2.70 + Math.random() * 0.20).toFixed(2), sellPrice: (2.80 + Math.random() * 0.20).toFixed(2) },
        { name: 'CAD', buyPrice: (3.00 + Math.random() * 0.20).toFixed(2), sellPrice: (3.10 + Math.random() * 0.20).toFixed(2) },
    ];
};

// Function sending current currency rates to all clients
const broadcastCurrencyRates = () => {
    const allCurrencies = generateRandomRates();
    wss.clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            const requestedCurrencies = client.requestedCurrencies || [];
            const currencies = requestedCurrencies.length > 0
                ? allCurrencies.filter(currency => requestedCurrencies.includes(currency.name))
                : allCurrencies;
            client.send(JSON.stringify(currencies), (err) => {
                if (err) {
                    console.error('Error sending to client:', err);
                }
            });
        }
    });
};

// Send currency rates every 5 seconds
setInterval(broadcastCurrencyRates, 5000);

wss.on('connection', (ws) => {
    console.log('Client connected');
    console.log(`Current number of clients: ${wss.clients.size}`);

    // Send current currency rates immediately after connection
    ws.send(JSON.stringify(generateRandomRates()), (err) => {
        if (err) console.error('Error sending initial data:', err);
    });

    ws.on('message', (message) => {
        try {
            const requestedCurrencies = JSON.parse(message);
            console.log('Requested currencies:', requestedCurrencies);
            if (requestedCurrencies.length === 1 && requestedCurrencies[0] === '') {
                ws.requestedCurrencies = [];
            } else {
                ws.requestedCurrencies = requestedCurrencies;
            }
            broadcastCurrencyRates();
        } catch (e) {
            console.error('Error parsing message:', e);
        }
    });

    ws.on('close', (code, reason) => {
        console.log('Client disconnected', code, reason);
        console.log(`Current number of clients: ${wss.clients.size}`);
    });

    ws.on('error', (err) => {
        // If the error is about an unfinished handshake, it can be ignored
        if (err.message && err.message.includes('closed without completing the close handshake')) {
            console.log('Ignored error of unfinished close handshake.');
        } else {
            console.error('WebSocket error:', err);
        }
    });
});

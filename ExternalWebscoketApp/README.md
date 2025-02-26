npx wscat -c wss://ws.postman-echo.com/raw
npx wscat -c ws://localhost:5183/ws

wss://echo.websocket.org


"ws://localhost:5183/ws"


let socket = new WebSocket("wss://localhost:7223/ws");

socket.onmessage = function(event) {
    console.log("📩 Otrzymane dane:", event.data);
};
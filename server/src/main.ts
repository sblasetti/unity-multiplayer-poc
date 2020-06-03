import socketio from 'socket.io';
import { OnPlayerDataReceived, OnSocketDisconnection, OnSocketConnection } from './api/Socket.service';

const io = socketio(3000);

console.log('started');

io.on('connection', (socket) => {
    OnSocketConnection(socket);

    socket.on('player:data', () => OnPlayerDataReceived(socket));

    socket.on('disconnect', () => OnSocketDisconnection(socket));
});

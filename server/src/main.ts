import socketio from 'socket.io';
import { OnPlayerDataReceived, OnSocketDisconnection, OnSocketConnection } from './api/Socket.service';
import { SOCKET_EVENTS } from './api/entities/Constants';

const io = socketio(3000);

console.log('started');

io.on(SOCKET_EVENTS.Socket.Connect, (socket: SocketIO.Socket) => {
    OnSocketConnection(socket);

    socket.on(SOCKET_EVENTS.Player.Data, () => OnPlayerDataReceived(socket));

    socket.on(SOCKET_EVENTS.Socket.Disconnect, () => OnSocketDisconnection(socket));
});

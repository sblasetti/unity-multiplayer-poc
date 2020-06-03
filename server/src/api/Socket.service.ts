import { apiService } from './Api.service';

function logMessage(msg: string, socket: SocketIO.Socket): void {
    console.log(`[${socket.id}] ${msg}`);
}

export function OnSocketConnection(socket: SocketIO.Socket): void {
    logMessage('new connection', socket);

    apiService.addPlayer(socket.id);
    socket.broadcast.emit('player:new', { id: socket.id });
}

export function OnPlayerDataReceived(socket: SocketIO.Socket): void {
    logMessage('player useful data received', socket);
}

export function OnSocketDisconnection(socket: SocketIO.Socket): void {
    apiService.removePlayer(socket.id);
    socket.broadcast.emit('player:gone', { id: socket.id });
    logMessage('connection ended', socket);
}

import { apiService } from './Api.service';
import { SOCKET_EVENTS } from './entities/Constants';

function logMessage(msg: string, socket: SocketIO.Socket): void {
    console.log(`[${socket.id}] ${msg}`);
}

export function OnSocketConnection(socket: SocketIO.Socket): void {
    logMessage('new connection', socket);

    // Get existing players before adding the new one
    const otherPlayers = apiService.getPlayers();
    apiService.addPlayer(socket.id);

    // Only communicate the new player if there are other players
    if (!otherPlayers.length) return;

    socket.broadcast.emit(SOCKET_EVENTS.Player.New, { id: socket.id });
    logMessage('sent new player data to others', socket);

    socket.emit(SOCKET_EVENTS.Player.OtherPlayers, { players: otherPlayers });
    logMessage('sent list of players', socket);
}

export function OnPlayerDataReceived(socket: SocketIO.Socket): void {
    logMessage('player useful data received', socket);
}

export function OnSocketDisconnection(socket: SocketIO.Socket): void {
    apiService.removePlayer(socket.id);
    socket.broadcast.emit(SOCKET_EVENTS.Player.Gone, { id: socket.id });
    logMessage('connection ended', socket);
}

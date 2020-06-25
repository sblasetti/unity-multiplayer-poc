import { apiService } from './Api.service';
import { SOCKET_EVENTS } from './entities/Constants';
import { newPlayer } from './entities/PlayerBuilder';

function logMessage(msg: string, socket: SocketIO.Socket): void {
    console.log(`[${socket.id}] ${msg}`);
}

export function OnSocketConnection(socket: SocketIO.Socket): void {
    logMessage('new connection', socket);

    // If socket already registered, exit
    const otherPlayers = apiService.getPlayers();
    if (otherPlayers.some((x) => x.id === socket.id)) return;

    // Define position
    const position = {
        x: 0,
        y: 0
    };

    // Send initial position to new player
    socket.emit(SOCKET_EVENTS.Player.InitialPosition, position);
    logMessage('sent position to new player', socket);
}

export function OnPlayerJoin(socket: SocketIO.Socket) {
    logMessage('player join', socket);

    const otherPlayers = apiService.getPlayers();
    logMessage(`other players count: ${otherPlayers.length}`, socket);

    // If player already registered, exit
    if (otherPlayers.some((x) => x.id === socket.id)) return;

    // Register new player
    const player = newPlayer(socket.id);
    apiService.addPlayer(player);

    // Only communicate the new player if there are other players
    if (!otherPlayers.length) return;

    socket.broadcast.emit(SOCKET_EVENTS.Player.New, player);
    logMessage('sent new player to others', socket);

    socket.emit(SOCKET_EVENTS.Player.OtherPlayers, otherPlayers);
    logMessage('sent other players to new player', socket);
}

export function OnLocalPlayerMovement(socket: SocketIO.Socket, data: Position & Movement): void {
    logMessage('player local move ' + JSON.stringify(data), socket);
    socket.broadcast.emit(SOCKET_EVENTS.Player.RemoteMove, {
        id: socket.id,
        horizontal: data.horizontalMovement,
        vertical: data.verticalMovement,
    });
}

export function OnSocketDisconnection(socket: SocketIO.Socket): void {
    apiService.removePlayer(socket.id);
    socket.broadcast.emit(SOCKET_EVENTS.Player.Gone, { id: socket.id });
    logMessage('connection ended', socket);
}

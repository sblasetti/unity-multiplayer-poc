import { logicService } from './Logic.service';
import { SOCKET_EVENTS } from './entities/Constants';
import { newPlayer } from './entities/PlayerBuilder';
import { buildPayload, buildLocalMoveValidationPayload, buildRemoteMove } from './entities/PayloadBuilder';

function logMessage(msg: string, socket: SocketIO.Socket): void {
    console.log(`[${socket.id}] ${msg}`);
}

export function OnSocketConnection(socket: SocketIO.Socket): void {
    logMessage('new connection', socket);

    // If socket already registered, exit
    const otherPlayers = logicService.getPlayers();
    if (otherPlayers.some((x) => x.id === socket.id)) return;

    // Define position
    const position = logicService.calculateInitialPosition();

    // Send initial position to new player
    socket.emit(SOCKET_EVENTS.Server.Welcome, buildPayload(position));
    logMessage('sent position to new player', socket);
}

export function OnPlayerJoin(socket: SocketIO.Socket, data: GameEvent<any>) {
    logMessage('player join: ' + data, socket);

    const otherPlayers = logicService.getPlayers();
    logMessage(`other players: ${JSON.stringify(otherPlayers)}`, socket);

    // If player already registered, exit
    if (otherPlayers.some((x) => x.id === socket.id)) return;

    // Register new player
    const player = newPlayer(socket.id);
    logicService.addPlayer(player);

    // Only communicate the new player if there are other players
    if (!otherPlayers.length) return;

    socket.emit(SOCKET_EVENTS.Player.OtherPlayers, buildPayload(otherPlayers));
    logMessage('sent other players to new player', socket);

    socket.broadcast.emit(SOCKET_EVENTS.Player.New, buildPayload(player));
    logMessage('sent new player to others', socket);
}

export function OnPlayerLocalMovement(socket: SocketIO.Socket, data: GameEvent<PlayerLocalMovementPayload>): void {
    logMessage('player local move ' + JSON.stringify(data), socket);

    const player = getPlayerById(socket.id);
    if (!player) {
        logMessage('ERR: player NOT FOUND!', socket);
        return;
    }

    const { position, rotation } = data.payload;
    var valid = logicService.isValidMovement(player, position);
    if (!valid) {
        logMessage('ERR: INVALID movement!', socket);
        return;
    }

    replyWithLocalMovementValidationResult(socket, position, rotation);
    updatePlayerPositionOnServer(socket.id, position, rotation);
    broadcastPlayerPositionToOthers(socket, position, rotation);
}

function broadcastPlayerPositionToOthers(socket: SocketIO.Socket, position: PlayerPosition, rotation: PlayerRotation) {
    socket.broadcast.emit(SOCKET_EVENTS.Player.RemoteMove, buildRemoteMove(socket.id, position, rotation));
}

function updatePlayerPositionOnServer(playerId: string, position: PlayerPosition, rotation: PlayerRotation) {
    logicService.updatePlayerPosition(playerId, position, rotation);
}

function replyWithLocalMovementValidationResult(
    socket: SocketIO.Socket,
    position: PlayerPosition,
    rotation: PlayerRotation,
) {
    socket.emit(SOCKET_EVENTS.Server.LocalMoveValidation, buildLocalMoveValidationPayload(true, position, rotation));
}

function getPlayerById(playerId: string) {
    return logicService.getPlayer(playerId);
}

export function OnSocketDisconnection(socket: SocketIO.Socket): void {
    logicService.removePlayer(socket.id);
    socket.broadcast.emit(SOCKET_EVENTS.Player.Gone, buildPayload({ id: socket.id }));
    logMessage('connection ended', socket);
}

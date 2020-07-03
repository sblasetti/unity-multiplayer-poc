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

export function OnPlayerJoin(socket: SocketIO.Socket, data: any) {
    logMessage('player join: ' + data, socket);

    const otherPlayers = logicService.getPlayers();
    logMessage(`other players count: ${otherPlayers.length}`, socket);

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

export function OnPlayerLocalMovement(socket: SocketIO.Socket, data: PlayerId & PlayerMovement): void {
    logMessage('player local move ' + JSON.stringify(data), socket);

    const player = getPlayerById(data);
    if (!player) return;

    const validationResult = validatePlayerLocalMovement(data, player);
    replyWithLocalMovementValidationResult(socket, validationResult);
    updatePlayerPositionOnServer(player, validationResult);
    broadcastPlayerPositionToOthers(socket, player, validationResult);
}

function broadcastPlayerPositionToOthers(
    socket: SocketIO.Socket,
    player: Player,
    validationResult: MovementValidationResult,
) {
    socket.broadcast.emit(SOCKET_EVENTS.Player.RemoteMove, buildRemoteMove(player.id, validationResult.position));
}

function updatePlayerPositionOnServer(player: Player, validationResult: MovementValidationResult) {
    logicService.updatePlayerPosition(player.id, validationResult.position);
}

function replyWithLocalMovementValidationResult(socket: SocketIO.Socket, response: MovementValidationResult) {
    socket.emit(SOCKET_EVENTS.Server.LocalMoveValidation, buildLocalMoveValidationPayload(true, response.position));
}

function validatePlayerLocalMovement(data: PlayerMovement, player: Player) {
    const { horizontal, vertical } = data;
    const response = logicService.calculateMovement(player, { horizontal, vertical });
    return response;
}

function getPlayerById(data: PlayerId & PlayerMovement) {
    return logicService.getPlayer(data.playerId);
}

export function OnSocketDisconnection(socket: SocketIO.Socket): void {
    logicService.removePlayer(socket.id);
    socket.broadcast.emit(SOCKET_EVENTS.Player.Gone, buildPayload({ id: socket.id }));
    logMessage('connection ended', socket);
}

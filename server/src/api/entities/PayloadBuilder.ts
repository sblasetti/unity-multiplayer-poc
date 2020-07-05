import { SOCKET_EVENTS } from './Constants';

export function buildPayload<T>(data: T): GameEvent<T> {
    return { payload: data };
}

export function buildLocalMoveValidationPayload(isValid: boolean, position: PlayerPosition, rotation: PlayerRotation) {
    return buildPayload({
        isValid,
        position,
        rotation,
    });
}

export function buildRemoteMove(playerId: string, position: PlayerPosition, rotation: PlayerRotation) {
    return buildPayload({
        id: playerId, // TODO: constants?
        position,
        rotation,
    });
}

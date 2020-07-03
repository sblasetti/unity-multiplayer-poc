export function buildPayload(data: any) {
    return { payload: data };
}

export function buildLocalMoveValidationPayload(isValid: boolean, position: MapCoordinates) {
    return buildPayload({ isValid, position });
}

export function buildRemoteMove(playerId: string, position: MapCoordinates) {
    return buildPayload({
        playerId,
        position,
    });
}

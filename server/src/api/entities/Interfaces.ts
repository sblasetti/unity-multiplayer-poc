interface PlayerId {
    playerId: string;
}

interface PlayerMovement {
    horizontal: number;
    vertical: number;
}

interface PlayerPosition {
    initialX: number;
    initialY: number;
}

interface MapCoordinates {
    x: number;
    y: number;
    z: number;
}

interface MovementValidationResult {
    position: MapCoordinates;
}

export function newPlayer(
    id: string,
    position: PlayerPosition = { x: 0, y: 0, z: 0 },
    rotation: PlayerRotation = { x: 0, y: 0, z: 0, w: 0 },
): Player {
    return {
        id,
        position,
        rotation,
    };
}

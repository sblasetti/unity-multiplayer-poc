export function newPlayer(id: string, x: number = 0, y: number = 0, z: number = 0): Player {
    return {
        id,
        position: {
            x,
            y,
            z,
        },
    };
}

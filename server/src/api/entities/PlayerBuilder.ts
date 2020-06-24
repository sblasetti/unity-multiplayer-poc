export function newPlayer(id: string): Player {
    return {
        id,
        position: {
            x: 0,
            y: 0,
        },
    };
}

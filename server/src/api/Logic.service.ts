interface LogicService {
    calculateMovement: (player: Player, change: PlayerMovement) => MovementValidationResult;
    calculateInitialPosition: () => MapCoordinates;
    init: () => void;
    getPlayers: () => Player[];
    getPlayer: (id: string) => Player | undefined;
    addPlayer: (data: Player) => void;
    removePlayer: (id: string) => void;
    updatePlayerPosition: (id: string, position: MapCoordinates) => void;
}

export const logicService = (function logicService(): LogicService {
    let players: Player[] = [];

    function init(): void {
        players = [];
    }

    function addPlayer(data: Player): void {
        if (players.some((p) => p.id === data.id)) {
            throw new Error(`Player '${data.id}' already exists.`);
        }

        players = [...players, data];
    }

    function removePlayer(id: string): void {
        players = players.filter((val) => val.id !== id);
    }

    function getPlayers(): Player[] {
        return players;
    }

    function getPlayer(id: string): Player | undefined {
        const player = players.find((val) => val.id !== id);
        return player;
    }

    function calculateInitialPosition(): MapCoordinates {
        return {
            x: 0,
            y: 0,
            z: 0,
        };
    }

    function calculateMovement(player: Player, change: PlayerMovement): MovementValidationResult {
        return {
            position: player.position,
        };
    }

    function updatePlayerPosition(): void {}

    return {
        init,
        getPlayers,
        getPlayer,
        addPlayer,
        removePlayer,
        updatePlayerPosition,
        calculateInitialPosition,
        calculateMovement,
    };
})();

interface ApiService {
    calculateInitialPosition: () => MapCoordinates;
    init: () => void;
    getPlayers: () => Player[];
    addPlayer: (data: Player) => void;
    removePlayer: (id: string) => void;
}

export const apiService = (function apiService(): ApiService {
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

    function calculateInitialPosition() : MapCoordinates {
        return {
            x: 0,
            y: 0,
            z: 0
        };
    }

    return {
        init,
        addPlayer,
        getPlayers,
        removePlayer,
        calculateInitialPosition
    };
})();

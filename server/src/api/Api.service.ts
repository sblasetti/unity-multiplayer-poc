interface ApiService {
    init: () => void;
    getPlayers: () => Player[];
    addPlayer: (id: string) => void;
    removePlayer: (id: string) => void;
}

export const apiService = (function apiService(): ApiService {
    let players: Player[] = [];

    function init(): void {
        players = [];
    }

    function addPlayer(id: string): void {
        if (players.some((p) => p.id === id)) {
            throw new Error(`Player '${id}' already exists.`);
        }

        players = [...players, { id }];
    }

    function removePlayer(id: string): void {
        players = players.filter((val) => val.id !== id);
    }

    function getPlayers(): Player[] {
        return players;
    }

    return {
        init,
        addPlayer,
        getPlayers,
        removePlayer,
    };
})();

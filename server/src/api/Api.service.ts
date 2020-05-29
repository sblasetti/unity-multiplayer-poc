interface Player {
    id: string;
}

interface ApiService {
    getPlayers: () => Player[];
    addPlayer: (id: string) => void;
    removePlayer: (id: string) => void;
}

export const apiService = (function apiService(): ApiService {
    let players: Player[] = [];

    function addPlayer(id: string): void {
        players = [...players, { id }];
    }

    function removePlayer(id: string): void {
        players = [];
    }

    function getPlayers(): Player[] {
        return players;
    }

    return {
        addPlayer,
        getPlayers,
        removePlayer,
    };
})();

import { Vector3, Vec2, Euler } from 'three';

interface LogicService {
    isValidMovement: (player: Player, newPosition: PlayerPosition) => boolean;
    calculateInitialPosition: () => PlayerPosition;
    init: () => void;
    getPlayers: () => Player[];
    getPlayer: (id: string) => Player | undefined;
    addPlayer: (data: Player) => void;
    removePlayer: (id: string) => void;
    updatePlayerPosition: (id: string, position: PlayerPosition, rotation: PlayerRotation) => void;
}

export const logicService = (function logicService(): LogicService {
    let players: Player[] = [];
    const settings = {
        player: {
            speed: 10,
            rotationSpeed: 100,
        },
        game: {
            frameRate: 0.2,
        },
    };
    const mapDimensions: Vec2 = { x: 20, y: 20 };

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
        const player = players.find((val) => val.id === id);
        return player;
    }

    function calculateInitialPosition(): PlayerPosition {
        return {
            x: 0,
            y: 0.5,
            z: 0,
        };
    }

    function isValidMovement(player: Player, newPosition: PlayerPosition): boolean {
        // TODO: validate

        return true;
    }

    function updatePlayerPosition(playerId: string, position: PlayerPosition, rotation: PlayerRotation): void {
        const playerIndex = players.findIndex((x) => x.id === playerId);
        if (playerIndex < 0) {
            console.log('ERR: player not found!');
            return;
        }

        players = [
            ...players.slice(0, playerIndex),
            { ...players[playerIndex], position: { ...position }, rotation: { ...rotation } },
            ...players.slice(playerIndex + 1),
        ];

        console.log(JSON.stringify(players));
    }

    return {
        init,
        getPlayers,
        getPlayer,
        addPlayer,
        removePlayer,
        updatePlayerPosition,
        calculateInitialPosition,
        isValidMovement,
    };
})();

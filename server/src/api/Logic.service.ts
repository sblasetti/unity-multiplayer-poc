import { Vector3, Vec2, Euler } from 'three';

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
    const characterSpecs = {
        speed: 10,
        rotationSpeed: 100,
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
        /*
        How to validate?
        We need the new position to check if it's valid or not
        - validate player is still within the limits of the map
        - validate the distance change is not too high

        a) if we receive distance & direction changes, we have to calculate rotation (need deltaTime)
        b) we could receive the change of direction in degrees
        c) if we receive new position, we need the time of the last change of position
        */
        const degreesY = change.horizontal * characterSpecs.rotationSpeed * deltaTime;
        const euler = new Euler(0, degreesY, 0);

        const {
            position: { x, y, z },
        } = player;
        const initialPosition = new Vector3(x, y, z);
        initialPosition.applyEuler(euler);

        const newPosition = initialPosition.add();

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

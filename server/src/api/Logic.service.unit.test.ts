import { logicService } from './Logic.service';

describe('LogicService', () => {
    const fakePlayer = { id: 'SOME_ID', position: { x: 1, y: 2, z: 3 }, rotation: { x: 4, y: 5, z: 6, w: 7 } };
    const zeroPosition: PlayerPosition = { x: 0, y: 0, z: 0 };
    const zeroRotation: PlayerRotation = { x: 0, y: 0, z: 0, w: 0 };

    beforeEach(() => {
        logicService.init();
    });

    it('Add a new player', () => {
        givenPlayerNotInTheServer(fakePlayer);

        whenAddingThePlayer(fakePlayer);

        thenPlayerIsAddedToTheServer(fakePlayer);
    });

    it('Error when adding a player twice', () => {
        givenPlayerAlreadyInTheServer(fakePlayer);

        const whenAddingAnotherPlayerWithTheSameId = () => {
            const anotherPlayer = { id: 'SOME_ID', position: zeroPosition, rotation: zeroRotation };
            logicService.addPlayer(anotherPlayer);
        };

        thenThrowDuplicatePlayerError(whenAddingAnotherPlayerWithTheSameId);
    });

    it('Delete a player', () => {
        givenThreePlayersInTheServer(zeroPosition, zeroRotation);

        whenRemovingPlayer2();

        thenOnlyPlayersOneAndThreeAreInTheServer(zeroPosition);
    });

    it('Valid local movement', () => {
        // Given
        const fakePosition: PlayerPosition = {
            x: 0.2,
            y: 0.5,
            z: -2.3,
        };

        // When
        const result = logicService.isValidMovement(fakePlayer, fakePosition);

        // Then
        expect(result).toBeFalsy();
    });
});

function thenOnlyPlayersOneAndThreeAreInTheServer(zeroPosition: { x: number; y: number; z: number }) {
    const players = logicService.getPlayers();
    expect(players).toHaveLength(2);
    expect(players).not.toContainEqual({ id: 'P2', position: zeroPosition });
}

function whenRemovingPlayer2() {
    logicService.removePlayer('P2');
}

function givenThreePlayersInTheServer(position: PlayerPosition, rotation: PlayerRotation) {
    logicService.addPlayer({ id: 'P1', position, rotation });
    logicService.addPlayer({ id: 'P2', position, rotation });
    logicService.addPlayer({ id: 'P3', position, rotation });
    expect(logicService.getPlayers()).toHaveLength(3);
}

function thenThrowDuplicatePlayerError(whenAddingAnotherPlayerWithTheSameId: () => void) {
    expect(whenAddingAnotherPlayerWithTheSameId).toThrowError("Player 'SOME_ID' already exists.");
}

function givenPlayerAlreadyInTheServer(playerData: Player) {
    logicService.addPlayer(playerData);
    expect(logicService.getPlayers()).toContainEqual(playerData);
}

function thenPlayerIsAddedToTheServer(playerData: { id: string; position: { x: number; y: number; z: number } }) {
    expect(logicService.getPlayers()).toContainEqual(playerData);
}

function whenAddingThePlayer(playerData: Player) {
    logicService.addPlayer(playerData);
}

function givenPlayerNotInTheServer(playerData: { id: string; position: { x: number; y: number; z: number } }) {
    expect(logicService.getPlayers()).not.toContainEqual(playerData);
}

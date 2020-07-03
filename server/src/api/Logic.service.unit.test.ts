import { logicService } from './Logic.service';

describe('LogicService', () => {
    const fakePlayer = { id: 'SOME_ID', position: { x: 1, y: 2, z: 3 } };
    const zeroPosition = { x: 0, y: 0, z: 0 };

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
            const anotherPlayer = { id: 'SOME_ID', position: zeroPosition };
            logicService.addPlayer(anotherPlayer);
        };

        thenThrowDuplicatePlayerError(whenAddingAnotherPlayerWithTheSameId);
    });

    it('Delete a player', () => {
        givenThreePlayersInTheServer(zeroPosition);

        whenRemovingPlayer2();

        thenOnlyPlayersOneAndThreeAreInTheServer(zeroPosition);
    });

    it('Valid local movement', () => {
        // Given
        const fakeMovement: PlayerMovement = {
            horizontal: 0.98,
            vertical: 0.2,
        };

        // When
        const result = logicService.calculateMovement(fakePlayer, fakeMovement);

        // Then
        expect(result.position).toBe({});
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

function givenThreePlayersInTheServer(zeroPosition: { x: number; y: number; z: number }) {
    logicService.addPlayer({ id: 'P1', position: zeroPosition });
    logicService.addPlayer({ id: 'P2', position: zeroPosition });
    logicService.addPlayer({ id: 'P3', position: zeroPosition });
    expect(logicService.getPlayers()).toHaveLength(3);
}

function thenThrowDuplicatePlayerError(whenAddingAnotherPlayerWithTheSameId: () => void) {
    expect(whenAddingAnotherPlayerWithTheSameId).toThrowError("Player 'SOME_ID' already exists.");
}

function givenPlayerAlreadyInTheServer(playerData: { id: string; position: { x: number; y: number; z: number } }) {
    logicService.addPlayer(playerData);
    expect(logicService.getPlayers()).toContainEqual(playerData);
}

function thenPlayerIsAddedToTheServer(playerData: { id: string; position: { x: number; y: number; z: number } }) {
    expect(logicService.getPlayers()).toContainEqual(playerData);
}

function whenAddingThePlayer(playerData: { id: string; position: { x: number; y: number; z: number } }) {
    logicService.addPlayer(playerData);
}

function givenPlayerNotInTheServer(playerData: { id: string; position: { x: number; y: number; z: number } }) {
    expect(logicService.getPlayers()).not.toContainEqual(playerData);
}

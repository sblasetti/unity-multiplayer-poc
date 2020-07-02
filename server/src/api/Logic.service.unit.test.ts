import { logicService } from './Logic.service';

describe('LogicService', () => {
    const playerData = { id: 'SOME_ID', position: { x: 1, y: 2 } };
    const zeroPosition = { x: 0, y: 0 };

    beforeEach(() => {
        logicService.init();
    });

    it('Add a new player', () => {
        // Given user ID not in the list
        expect(logicService.getPlayers()).not.toContainEqual(playerData);

        // When adding the user
        logicService.addPlayer(playerData);

        // Then the user is added
        expect(logicService.getPlayers()).toContainEqual(playerData);
    });

    it('Error when adding a player twice', () => {
        // Given a player in the list
        logicService.addPlayer(playerData);
        expect(logicService.getPlayers()).toContainEqual(playerData);

        // When adding another user with the same ID
        // Then throw error with message
        expect(() => {
            const anotherPlayer = { id: 'SOME_ID', position: zeroPosition };
            logicService.addPlayer(anotherPlayer);
        }).toThrowError("Player 'SOME_ID' already exists.");
    });

    it('Delete a player', () => {
        // Given
        logicService.addPlayer({ id: 'P1', position: zeroPosition });
        logicService.addPlayer({ id: 'P2', position: zeroPosition });
        logicService.addPlayer({ id: 'P3', position: zeroPosition });
        expect(logicService.getPlayers()).toHaveLength(3);

        // When
        logicService.removePlayer('P2');

        // Then
        const players = logicService.getPlayers();
        expect(players).toHaveLength(2);
        expect(players).not.toContainEqual({ id: 'P2', position: zeroPosition });
    });
});

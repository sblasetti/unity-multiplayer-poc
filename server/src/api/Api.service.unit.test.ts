import { apiService } from './Api.service';

describe('ApiService', () => {
    const playerData = { id: 'SOME_ID', position: { x: 1, y: 2 } };
    const zeroPosition = { x: 0, y: 0 };

    beforeEach(() => {
        apiService.init();
    });

    it('Add a new player', () => {
        // Given user ID not in the list
        expect(apiService.getPlayers()).not.toContainEqual(playerData);

        // When adding the user
        apiService.addPlayer(playerData);

        // Then the user is added
        expect(apiService.getPlayers()).toContainEqual(playerData);
    });

    it('Error when adding a player twice', () => {
        // Given a player in the list
        apiService.addPlayer(playerData);
        expect(apiService.getPlayers()).toContainEqual(playerData);

        // When adding another user with the same ID
        // Then throw error with message
        expect(() => {
            const anotherPlayer = { id: 'SOME_ID', position: zeroPosition };
            apiService.addPlayer(anotherPlayer);
        }).toThrowError("Player 'SOME_ID' already exists.");
    });

    it('Delete a player', () => {
        // Given
        apiService.addPlayer({ id: 'P1', position: zeroPosition });
        apiService.addPlayer({ id: 'P2', position: zeroPosition });
        apiService.addPlayer({ id: 'P3', position: zeroPosition });
        expect(apiService.getPlayers()).toHaveLength(3);

        // When
        apiService.removePlayer('P2');

        // Then
        const players = apiService.getPlayers();
        expect(players).toHaveLength(2);
        expect(players).not.toContainEqual({ id: 'P2', position: zeroPosition });
    });
});

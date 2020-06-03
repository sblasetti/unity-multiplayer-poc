import { apiService } from './Api.service';

describe('ApiService', () => {
    beforeEach(() => {
        apiService.init();
    });

    it('Add a new player', () => {
        // Given user ID not in the list
        expect(apiService.getPlayers()).not.toContainEqual({ id: 'SOME_ID' });

        // When adding the user
        apiService.addPlayer('SOME_ID');

        // Then the user is added
        expect(apiService.getPlayers()).toContainEqual({ id: 'SOME_ID' });
    });

    it('Error when adding a player twice', () => {
        // Given a player in the list
        apiService.addPlayer('SOME_ID');
        expect(apiService.getPlayers()).toContainEqual({ id: 'SOME_ID' });

        // When adding another user with the same ID
        // Then throw error with message
        expect(() => {
            apiService.addPlayer('SOME_ID');
        }).toThrowError("Player 'SOME_ID' already exists.");
    });

    it('Delete a player', () => {
        // Given
        apiService.addPlayer('P1');
        apiService.addPlayer('P2');
        apiService.addPlayer('P3');
        expect(apiService.getPlayers()).toHaveLength(3);

        // When
        apiService.removePlayer('P2');

        // Then
        const players = apiService.getPlayers();
        expect(players).toHaveLength(2);
        expect(players).not.toContainEqual({ id: 'P2' });
    });
});

import { apiService } from './Api.service';

describe('ApiService', () => {
    it('Add a new player', () => {
        apiService.addPlayer('SOME_ID');

        expect(apiService.getPlayers()).toContainEqual({ id: 'SOME_ID' });
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

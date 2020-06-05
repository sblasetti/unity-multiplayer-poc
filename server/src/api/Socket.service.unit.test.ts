import { mock, MockProxy, mockReset } from 'jest-mock-extended';
import { OnSocketConnection, OnSocketDisconnection } from './Socket.service';
import { SOCKET_EVENTS } from './entities/Constants';
import { apiService } from './Api.service';

jest.mock('./Api.service');

const consoleLogSpy = jest.spyOn(console, 'log').mockImplementation();

const socketMock: MockProxy<SocketIO.Socket> = mock<SocketIO.Socket>();
socketMock.broadcast = mock<SocketIO.Socket>();

describe('SocketService.OnSocketConnection', () => {
    beforeEach(() => {
        mockReset(socketMock);
        jest.resetAllMocks();
    });

    it('On first connection, register player and do not send any other message', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        const playerData = { id: socketMock.id };
        apiService.getPlayers = jest.fn(() => []);

        // When
        OnSocketConnection(socketMock);

        // Then: player registered in server
        expect(apiService.addPlayer).toHaveBeenCalledTimes(1);
        expect(apiService.addPlayer).toHaveBeenCalledWith(playerData.id);
        // Then: no broadcasting to other players
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(0);
        // Then: no other players to inform back
        expect(socketMock.emit).toHaveBeenCalledTimes(0);
    });

    it('On any other connection, register new player, communicate players list delta to players', () => {
        // Given
        socketMock.id = 'THIRD_ID';
        const playerData = { id: socketMock.id };
        const existingPlayers = [{ id: 'FIRST_ID' }, { id: 'SECOND_ID' }];
        apiService.getPlayers = jest.fn(() => existingPlayers);

        // When
        OnSocketConnection(socketMock);

        // Then: both players registered in server
        expect(apiService.addPlayer).toHaveBeenCalledTimes(1);
        expect(apiService.addPlayer).toHaveBeenCalledWith(playerData.id);
        // Then: new player is broadcasted to existing players
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(1);
        expect(socketMock.broadcast.emit).toHaveBeenCalledWith(SOCKET_EVENTS.Player.New, { id: 'THIRD_ID' });
        // Then: existing players informed back to new player
        expect(socketMock.emit).toHaveBeenCalledTimes(1);
        expect(socketMock.emit).toHaveBeenCalledWith(SOCKET_EVENTS.Player.OtherPlayers, {
            players: existingPlayers,
        });
    });

    it('OnSocketDisconnection remove player and broadcast removal to other connections', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        const playerData = { id: socketMock.id };

        // When
        OnSocketDisconnection(socketMock);

        // Then
        expect(apiService.removePlayer).toHaveBeenCalledTimes(1);
        expect(apiService.removePlayer).toHaveBeenCalledWith(playerData.id);
        expect(socketMock.broadcast.emit).toHaveBeenCalledWith(SOCKET_EVENTS.Player.Gone, { id: 'MOCK_ID' });
    });

    afterAll(() => {
        consoleLogSpy.mockRestore();
    });
});

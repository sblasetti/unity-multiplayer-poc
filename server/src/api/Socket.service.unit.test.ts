import { mock, MockProxy, mockReset } from 'jest-mock-extended';
import { OnSocketConnection, OnSocketDisconnection } from './Socket.service';
import { SOCKET_EVENTS } from './entities/Constants';
import { apiService } from './Api.service';

jest.mock('./Api.service');

const consoleLogSpy = jest.spyOn(console, 'log').mockImplementation();

const socketMock: MockProxy<SocketIO.Socket> = mock<SocketIO.Socket>();
socketMock.broadcast = mock<SocketIO.Socket>();

describe('SocketService', () => {
    beforeEach(() => {
        mockReset(socketMock);
    });

    it('OnSocketConnection add player and broadcast it to other connections', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        const playerData = { id: socketMock.id };

        // When
        OnSocketConnection(socketMock);

        // Then
        expect(apiService.addPlayer).toHaveBeenCalledTimes(1);
        expect(apiService.addPlayer).toHaveBeenCalledWith(playerData.id);
        expect(socketMock.broadcast.emit).toHaveBeenCalledWith(SOCKET_EVENTS.Player.New, playerData);
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

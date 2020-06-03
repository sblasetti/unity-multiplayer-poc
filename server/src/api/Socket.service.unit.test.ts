import { mock, MockProxy } from 'jest-mock-extended';
import { OnSocketConnection } from './Socket.service';

const consoleSpy = jest.spyOn(console, 'log').mockImplementation();

const socketMock: MockProxy<SocketIO.Socket> = mock<SocketIO.Socket>();
socketMock.broadcast = mock<SocketIO.Socket>();

describe('SocketService', () => {
    beforeEach(() => {
        // TBD
    });

    it('OnSocketConnection add player and broadcast it to other connections', () => {
        // Given
        socketMock.id = 'MOCK_ID';

        // When
        OnSocketConnection(socketMock);

        // Then
        expect(socketMock.broadcast.emit).toHaveBeenCalledWith('player:new', { id: 'MOCK_ID' });
    });

    afterAll(() => {
        consoleSpy.mockRestore();
    });
});

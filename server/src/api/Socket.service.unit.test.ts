import { mock, MockProxy, mockReset } from 'jest-mock-extended';
import { OnSocketConnection, OnSocketDisconnection, OnPlayerJoin } from './Socket.service';
import { SOCKET_EVENTS } from './entities/Constants';
import { apiService } from './Api.service';
import { newPlayer } from './entities/PlayerBuilder';

jest.mock('./Api.service');

jest.spyOn(console, 'log').mockImplementation();

const socketMock: MockProxy<SocketIO.Socket> = mock<SocketIO.Socket>();
socketMock.broadcast = mock<SocketIO.Socket>();

describe('Player Connects - SocketService.OnSocketConnection', () => {
    beforeEach(() => {
        mockReset(socketMock);
        jest.resetAllMocks();
    });

    it('On a connection from an already connected socket, do nothing', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        const playerData: Player = newPlayer(socketMock.id);
        apiService.getPlayers = jest.fn(() => [playerData]);

        // When
        OnSocketConnection(socketMock);

        // Then: do nothing (the player has already connected before)
        expect(socketMock.emit).toHaveBeenCalledTimes(0);
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(0);
    });

    it('On a new connection, define initial position and send to new player', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        apiService.getPlayers = jest.fn(() => []);
        const playerData: Player = newPlayer(socketMock.id);

        // When
        OnSocketConnection(socketMock);

        // Then: do not store the player yet
        expect(apiService.addPlayer).toHaveBeenCalledTimes(0);
        // Then: inform position to new player
        expect(socketMock.emit).toHaveBeenCalledTimes(1);
        expect(socketMock.emit).toHaveBeenCalledWith(SOCKET_EVENTS.Player.InitialPosition, playerData.position);
        // Then: don't broadcast new player to other players
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(0);
    });

    afterAll(() => {
        // TBD
    });
});

describe('Player Joins The Game - SocketService.OnPlayerJoin', () => {
    beforeEach(() => {
        mockReset(socketMock);
        jest.resetAllMocks();
    });

    it('On a new join from an already existing player, do nothing', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        const playerData: Player = newPlayer(socketMock.id);
        apiService.getPlayers = jest.fn(() => [playerData]);

        // When
        OnPlayerJoin(socketMock);

        // Then: do nothing (the player has already joined)
        expect(apiService.addPlayer).toHaveBeenCalledTimes(0);
        expect(socketMock.emit).toHaveBeenCalledTimes(0);
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(0);
    });

    it('On a new join and no other players, store the player and nothing else', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        apiService.getPlayers = jest.fn(() => []);
        const playerData: Player = newPlayer(socketMock.id);

        // When
        OnPlayerJoin(socketMock);

        // Then: store new player
        expect(apiService.addPlayer).toHaveBeenCalledTimes(1);
        expect(apiService.addPlayer).toHaveBeenCalledWith(playerData);
        // Then: don-t send other players info to new player
        expect(socketMock.emit).toHaveBeenCalledTimes(0);
        // Then: don't broadcast new player to other players
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(0);
    });

    it('On a new join with other players, send existing players to new player and new player info to other players', () => {
        // Given
        socketMock.id = 'THIRD_ID';
        const playerData = newPlayer(socketMock.id);
        const existingPlayers: Player[] = [
            { id: 'FIRST_ID', position: { x: 0, y: 0 } },
            { id: 'SECOND_ID', position: { x: 0, y: 0 } },
        ];
        apiService.getPlayers = jest.fn(() => existingPlayers);

        // When
        OnPlayerJoin(socketMock);

        // Then: store new player
        expect(apiService.addPlayer).toHaveBeenCalledTimes(1);
        expect(apiService.addPlayer).toHaveBeenCalledWith(playerData);
        // Then: broadcast new player to other players
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(1);
        expect(socketMock.broadcast.emit).toHaveBeenCalledWith(SOCKET_EVENTS.Player.New, playerData);
        // Then: inform other players to new player
        expect(socketMock.emit).toHaveBeenCalledTimes(1);
        expect(socketMock.emit).toHaveBeenCalledWith(SOCKET_EVENTS.Player.OtherPlayers, existingPlayers);
    });

    afterAll(() => {
        // TBD
    });
});

describe('Player Disconnects - SocketService.OnSocketDisconnection', () => {
    beforeEach(() => {
        mockReset(socketMock);
        jest.resetAllMocks();
    });

    it('On disconnection remove player and broadcast removal to other connections', () => {
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
        // TBD
    });
});

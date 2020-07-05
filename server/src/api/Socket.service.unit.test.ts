import { mock, MockProxy, mockReset } from 'jest-mock-extended';
import { OnSocketConnection, OnSocketDisconnection, OnPlayerJoin, OnPlayerLocalMovement } from './Socket.service';
import { SOCKET_EVENTS } from './entities/Constants';
import { logicService } from './Logic.service';
import { newPlayer } from './entities/PlayerBuilder';
import { buildPayload } from './entities/PayloadBuilder';

jest.mock('./Logic.service');

jest.spyOn(console, 'log').mockImplementation();

const socketMock: MockProxy<SocketIO.Socket> = mock<SocketIO.Socket>();
const initialPositionMock = { x: 0, y: 0, z: 0 };
socketMock.broadcast = mock<SocketIO.Socket>();

describe('Player Connects - SocketService.OnSocketConnection', () => {
    beforeEach(() => {
        resetMocks();

        logicService.calculateInitialPosition = jest.fn(() => initialPositionMock);
    });

    it('On a connection from an already connected socket, do nothing', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        const playerData: Player = newPlayer(socketMock.id);
        logicService.getPlayers = jest.fn(() => [playerData]);

        // When
        OnSocketConnection(socketMock);

        // Then: do nothing (the player has already connected before)
        expect(socketMock.emit).toHaveBeenCalledTimes(0);
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(0);
    });

    it('On a new connection, define initial position and send to new player', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        logicService.getPlayers = jest.fn(() => []);

        // When
        OnSocketConnection(socketMock);

        // Then: do not store the player yet
        expect(logicService.addPlayer).toHaveBeenCalledTimes(0);
        // Then: inform position to new player
        expect(logicService.calculateInitialPosition).toHaveBeenCalledTimes(1);
        expect(socketMock.emit).toHaveBeenCalledTimes(1);
        expect(socketMock.emit).toHaveBeenCalledWith(SOCKET_EVENTS.Server.Welcome, buildPayload(initialPositionMock));
        // Then: don't broadcast new player to other players
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(0);
    });

    afterAll(() => {
        // TBD
    });
});

describe('Player Joins The Game - SocketService.OnPlayerJoin', () => {
    const fakeGameEvent: GameEvent<any> = { payload: {} };

    beforeEach(() => {
        resetMocks();
    });

    it('On a new join from an already existing player, do nothing', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        const playerData: Player = newPlayer(socketMock.id);
        logicService.getPlayers = jest.fn(() => [playerData]);

        // When
        OnPlayerJoin(socketMock, fakeGameEvent);

        // Then: do nothing (the player has already joined)
        expect(logicService.addPlayer).toHaveBeenCalledTimes(0);
        expect(socketMock.emit).toHaveBeenCalledTimes(0);
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(0);
    });

    it('On a new join and no other players, store the player and nothing else', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        logicService.getPlayers = jest.fn(() => []);
        const playerData: Player = newPlayer(socketMock.id);

        // When
        OnPlayerJoin(socketMock, fakeGameEvent);

        // Then: store new player
        expect(logicService.addPlayer).toHaveBeenCalledTimes(1);
        expect(logicService.addPlayer).toHaveBeenCalledWith(playerData);
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
            { id: 'FIRST_ID', position: { x: 0, y: 0, z: 0 }, rotation: { x: 0, y: 0, z: 0, w: 0 } },
            { id: 'SECOND_ID', position: { x: 0, y: 0, z: 0 }, rotation: { x: 0, y: 0, z: 0, w: 0 } },
        ];
        logicService.getPlayers = jest.fn(() => existingPlayers);

        // When
        OnPlayerJoin(socketMock, fakeGameEvent);

        // Then: store new player
        expect(logicService.addPlayer).toHaveBeenCalledTimes(1);
        expect(logicService.addPlayer).toHaveBeenCalledWith(playerData);
        // Then: broadcast new player to other players
        expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(1);
        expect(socketMock.broadcast.emit).toHaveBeenCalledWith(SOCKET_EVENTS.Player.New, buildPayload(playerData));
        // Then: inform other players to new player
        expect(socketMock.emit).toHaveBeenCalledTimes(1);
        expect(socketMock.emit).toHaveBeenCalledWith(SOCKET_EVENTS.Player.OtherPlayers, buildPayload(existingPlayers));
    });

    afterAll(() => {
        // TBD
    });
});

describe('Player Moves - SocketService.OnPlayerLocalMovement', () => {
    socketMock.id = 'MOCK_ID';
    const fakePlayer: Player = newPlayer(socketMock.id);
    const fakePayload: PlayerLocalMovementPayload = {
        position: {
            x: 0.123,
            y: 0.5,
            z: -0.456,
        },
        rotation: {
            x: 0.333,
            y: 0.444,
            z: -0.555,
            w: 0.666,
        },
    };

    beforeEach(() => {
        resetMocks();
    });

    xit('On a local move, validate payload format', () => {
        // Given
        // When
        // Then
    });

    xit('On a local move, validate player exists', () => {
        // Given
        // When
        // Then
    });

    it('On a valid local move, send response, store new position and broadcast position change', () => {
        givenGetPlayerReturns(fakePlayer);
        givenIsValidMovementReturns(true);

        // When
        OnPlayerLocalMovement(socketMock, { payload: fakePayload });

        thenPlayerIsRetrieved(socketMock);
        thenPlayerLocalMovementIsValidated(fakePlayer, fakePayload.position);
        thenLocalMovementValidationIsSentBack(fakePayload.position, fakePayload.rotation);
        thenPlayerPositionIsUpdatedInTheServer(socketMock, fakePayload.position, fakePayload.rotation);
        thenPlayerPositionIsSentToOtherPlayers(socketMock, fakePayload.position, fakePayload.rotation);
    });

    afterAll(() => {
        // TBD
    });
});

describe('Player Disconnects - SocketService.OnSocketDisconnection', () => {
    beforeEach(() => {
        resetMocks();
    });

    it('On disconnection remove player and broadcast removal to other connections', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        const playerData = { id: socketMock.id };

        // When
        OnSocketDisconnection(socketMock);

        // Then
        expect(logicService.removePlayer).toHaveBeenCalledTimes(1);
        expect(logicService.removePlayer).toHaveBeenCalledWith(playerData.id);
        expect(socketMock.broadcast.emit).toHaveBeenCalledWith(
            SOCKET_EVENTS.Player.Gone,
            buildPayload({ id: 'MOCK_ID' }),
        );
    });

    afterAll(() => {
        // TBD
    });
});

function resetMocks() {
    mockReset(socketMock);
    jest.resetAllMocks();
}

function givenIsValidMovementReturns(isValidMovementResponse: boolean) {
    logicService.isValidMovement = jest.fn(() => isValidMovementResponse);
}

function givenGetPlayerReturns(fakePlayer: Player) {
    logicService.getPlayer = jest.fn(() => fakePlayer);
}

function thenPlayerPositionIsSentToOtherPlayers(
    socket: SocketIO.Socket,
    fakePosition: PlayerPosition,
    fakeRotation: PlayerRotation,
) {
    expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(1);
    expect(socketMock.broadcast.emit).toHaveBeenCalledWith(
        SOCKET_EVENTS.Player.RemoteMove,
        buildPayload({
            playerId: socketMock.id,
            position: fakePosition,
            rotation: fakeRotation,
        }),
    );
}

function thenPlayerPositionIsUpdatedInTheServer(
    socket: SocketIO.Socket,
    fakePosition: PlayerPosition,
    fakeRotation: PlayerRotation,
) {
    expect(logicService.updatePlayerPosition).toHaveBeenCalledTimes(1);
    expect(logicService.updatePlayerPosition).toHaveBeenCalledWith(socket.id, fakePosition, fakeRotation);
}

function thenLocalMovementValidationIsSentBack(fakePosition: PlayerPosition, fakeRotation: PlayerRotation) {
    expect(socketMock.emit).toHaveBeenCalledTimes(1);
    expect(socketMock.emit).toHaveBeenCalledWith(
        SOCKET_EVENTS.Server.LocalMoveValidation,
        buildPayload({
            isValid: true,
            position: fakePosition,
            rotation: fakeRotation,
        }),
    );
}

function thenPlayerLocalMovementIsValidated(fakePlayer: Player, fakePosition: PlayerPosition) {
    expect(logicService.isValidMovement).toHaveBeenCalledTimes(1);
    expect(logicService.isValidMovement).toHaveBeenCalledWith(fakePlayer, fakePosition);
}

function thenPlayerIsRetrieved(socket: SocketIO.Socket) {
    expect(logicService.getPlayer).toHaveBeenCalledTimes(1);
    expect(logicService.getPlayer).toHaveBeenCalledWith(socket.id);
}

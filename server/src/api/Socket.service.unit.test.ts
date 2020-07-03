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
    const fakeEventPayload = {};

    beforeEach(() => {
        resetMocks();
    });

    it('On a new join from an already existing player, do nothing', () => {
        // Given
        socketMock.id = 'MOCK_ID';
        const playerData: Player = newPlayer(socketMock.id);
        logicService.getPlayers = jest.fn(() => [playerData]);

        // When
        OnPlayerJoin(socketMock, fakeEventPayload);

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
        OnPlayerJoin(socketMock, {});

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
            { id: 'FIRST_ID', position: { x: 0, y: 0, z: 0 } },
            { id: 'SECOND_ID', position: { x: 0, y: 0, z: 0 } },
        ];
        logicService.getPlayers = jest.fn(() => existingPlayers);

        // When
        OnPlayerJoin(socketMock, {});

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
    const fakeMovementResponse: MovementValidationResult = {
        position: {
            x: 0.45,
            y: 0,
            z: 0.123,
        },
    };
    const fakePayload: PlayerId & PlayerMovement = {
        playerId: 'FAKE_ID',
        horizontal: 0.123,
        vertical: -0.456,
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
        givenCalculateMovementReturns(fakeMovementResponse);

        // When
        OnPlayerLocalMovement(socketMock, fakePayload);

        thenPlayerIsRetrieved(fakePayload);
        thenPlayerLocalMovementIsValidated(fakePlayer, fakePayload);
        thenLocalMovementValidationIsSentBack(fakeMovementResponse);
        thenPlayerPositionIsUpdatedInTheServer(fakePlayer, fakeMovementResponse);
        thenPlayerPositionIsSentToOtherPlayers(fakePlayer, fakeMovementResponse);
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

function givenCalculateMovementReturns(fakeMovementResponse: MovementValidationResult) {
    logicService.calculateMovement = jest.fn(() => fakeMovementResponse);
}

function givenGetPlayerReturns(fakePlayer: Player) {
    logicService.getPlayer = jest.fn(() => fakePlayer);
}

function thenPlayerPositionIsSentToOtherPlayers(fakePlayer: Player, fakeMovementResponse: MovementValidationResult) {
    expect(socketMock.broadcast.emit).toHaveBeenCalledTimes(1);
    expect(socketMock.broadcast.emit).toHaveBeenCalledWith(
        SOCKET_EVENTS.Player.RemoteMove,
        buildPayload({
            playerId: fakePlayer.id,
            position: fakeMovementResponse.position,
        }),
    );
}

function thenPlayerPositionIsUpdatedInTheServer(fakePlayer: Player, fakeMovementResponse: MovementValidationResult) {
    expect(logicService.updatePlayerPosition).toHaveBeenCalledTimes(1);
    expect(logicService.updatePlayerPosition).toHaveBeenCalledWith(fakePlayer.id, fakeMovementResponse.position);
}

function thenLocalMovementValidationIsSentBack(fakeMovementResponse: MovementValidationResult) {
    expect(socketMock.emit).toHaveBeenCalledTimes(1);
    expect(socketMock.emit).toHaveBeenCalledWith(
        SOCKET_EVENTS.Server.LocalMoveValidation,
        buildPayload({
            isValid: true,
            position: fakeMovementResponse.position,
        }),
    );
}

function thenPlayerLocalMovementIsValidated(fakePlayer: Player, fakePayload: PlayerId & PlayerMovement) {
    expect(logicService.calculateMovement).toHaveBeenCalledTimes(1);
    expect(logicService.calculateMovement).toHaveBeenCalledWith(fakePlayer, {
        horizontal: fakePayload.horizontal,
        vertical: fakePayload.vertical,
    });
}

function thenPlayerIsRetrieved(fakePayload: PlayerId & PlayerMovement) {
    expect(logicService.getPlayer).toHaveBeenCalledTimes(1);
    expect(logicService.getPlayer).toHaveBeenCalledWith(fakePayload.playerId);
}

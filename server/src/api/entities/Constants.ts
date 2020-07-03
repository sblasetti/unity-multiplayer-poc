export const SOCKET_EVENTS = {
    Socket: {
        Connect: 'connection',
        Disconnect: 'disconnect',
    },
    Player: {
        New: 'player:new',
        Gone: 'player:gone',
        Join: 'player:join',
        OtherPlayers: 'player:other-players',
        LocalMove: 'player:local-movement',
        RemoteMove: 'player:remote-movement',
    },
    Server: {
        Welcome: 'server:welcome',
        LocalMoveValidation: 'server:local-movement-validation',
    },
};

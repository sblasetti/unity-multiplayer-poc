export const SOCKET_EVENTS = {
    Socket: {
        Connect: 'connection',
        Disconnect: 'disconnect',
    },
    Player: {
        New: 'player:new',
        Gone: 'player:gone',
        Data: 'player:data',
        InitialPosition: 'player:initial-position',
        OtherPlayers: 'player:other-players',
        LocalMove: 'player:local-movement',
        RemoteMove: 'player:remote-movement',
    },
};

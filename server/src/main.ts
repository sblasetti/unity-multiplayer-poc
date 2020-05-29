import socketio from 'socket.io';
import { apiService } from './api/Api.service';

function getMessage(msg: string, socket: socketio.Socket): string {
    return `[${socket.id}] ${msg}`;
}

const io = socketio(3000);

console.log('started');

io.on('connection', (socket) => {
    console.log(getMessage('new connection', socket));

    apiService.addPlayer(socket.id);
    socket.broadcast.emit('player:new', { id: socket.id });

    socket.on('player:data', () => {
        console.log(getMessage('player useful data received', socket));
    });

    socket.on('disconnect', () => {
        apiService.removePlayer(socket.id);
        socket.broadcast.emit('player:gone', { id: socket.id });
        console.log(getMessage('connection ended', socket));
    });
});

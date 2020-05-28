import socketio from 'socket.io';
import { getDummyContent } from './api/Api.service';

const io = socketio(3000);

console.log('started');

io.on('connection', (socket) => {
    console.log(`new connection with id ${socket.id}`);
    console.log(getDummyContent());

    socket.on('disconnect', () => {
        console.log(`connection ${socket.id} ended`);
    });
});

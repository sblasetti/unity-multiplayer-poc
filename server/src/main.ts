import path from 'path';
import express from 'express';
import apiController from './api/Api.controller';

const app = express();

app.use('/', express.static(path.join('dist', 'static')));

app.use('/api', apiController);

app.get('*', (req, res) => {
    res.sendStatus(404);
});

app.listen(3000, () => {
    console.log('Listening in port 3000!...');
});

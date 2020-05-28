import express from 'express';
import { getRoot } from './Api.service';

const apiRouter = express.Router();

apiRouter.get('/', (req, res) => {
    const content = getRoot();
    res.send(content);
});

export default apiRouter;

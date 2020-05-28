import { getRoot } from './Api.service';

describe('ApiService', () => {
    it('GET /', () => {
        expect(getRoot()).toBe('Some dummy content');
    });
});

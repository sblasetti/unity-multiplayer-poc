import { getDummyContent } from './Api.service';

describe('ApiService', () => {
    it('GET /', () => {
        expect(getDummyContent()).toBe('Some dummy content');
    });
});

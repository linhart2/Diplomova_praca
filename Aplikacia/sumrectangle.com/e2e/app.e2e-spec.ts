import { DiaangularPage } from './app.po';

describe('diaangular App', () => {
  let page: DiaangularPage;

  beforeEach(() => {
    page = new DiaangularPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});

import { SumrectanglePage } from './app.po';

describe('sumrectangle App', () => {
  let page: SumrectanglePage;

  beforeEach(() => {
    page = new SumrectanglePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});

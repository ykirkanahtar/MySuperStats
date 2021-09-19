import { MySuperStatsTemplatePage } from './app.po';

describe('MySuperStats App', function() {
  let page: MySuperStatsTemplatePage;

  beforeEach(() => {
    page = new MySuperStatsTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});

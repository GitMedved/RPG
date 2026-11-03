# M4 — Production & Order Board Prototype

## Implemented
- Added one persistent production job slot.
- Added three recipes aligned with the MVP economy document:
  - Carrot Soup: 3 carrots, 3 minutes, 1 soup.
  - Strawberry Juice: 3 strawberries, 4 minutes, 1 juice.
  - Wooden Toy: 2 wood, 5 minutes, 1 toy.
- Production uses UTC timestamps; the job continues while the app is closed.
- Added a workshop interaction in the backyard. It starts the first recipe whose ingredients are available; interacting again collects a finished product.
- Added a persistent order completion ledger and three fixed orders for soup, juice and wooden toy.
- Delivering an order consumes the requested product and grants coins and XP once. Completed order IDs are saved to prevent duplicate claims.
- Added save validation for new production and order fields.

## Prototype controls
Approach the workshop or order board until its interaction prompt appears, then use the existing interaction control.
The workshop currently auto-selects a recipe in carrot soup → strawberry juice → wooden toy order. The order board automatically delivers the first order the player can fulfil.

## Deliberate limitations
- No recipe picker, production queue UI, order cards, refresh timer or order expiry.
- One production slot only.
- Recipes and orders are fixed in code rather than content assets.
- Starter wood is included to let a new player test the wooden toy recipe.
- Rewards are initial prototype values, not final economy balance.
- No Unity Editor compilation, Play Mode, save migration test or Android/iOS device build has been run here.

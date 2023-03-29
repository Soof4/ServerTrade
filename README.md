
# ServerTrade
A TShock plugin that let's players trade items with the server.

## Permission and Command
Add `servertrade.trade` permission for players to use `/trade <item name>` 

## Configuration
When plugin runs for the first time it'll create a file named "ServerTradeConfig.json". You need to add items in this file.

Under the shopList dictionary you need to add items like: 
```
"ItemName": [itemID, itemAmount, RequirementItemID, AmountofRequirementItem]
```
Note: ItemName should not any include whitespaces. Plus, if requirement item is not stackable and requirement amount is greater than 1, then players won't be able to trade that item due to how plugins checks items in the inventory.



Example config file:
```
{
  "shopList": {
      "TheDirtiestBlock": [5400, 1, 2, 2000],
      "Fish": [669, 1, 2290, 50],
      "LuckyHorseshoe": [158, 1, 159, 1]
  }
}

```

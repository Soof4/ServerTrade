using Terraria;
using System;
using TerrariaApi.Server;
using TShockAPI;
using On.Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using TShockAPI.Hooks;
using Microsoft.Xna.Framework.Input;
using NuGet.Packaging.Signing;
using ReLogic.Peripherals.RGB;

namespace ServerTrade {

    [ApiVersion(2, 1)]
    public class ServerTrade : TerrariaPlugin {

        public override string Name => "ServerTrade";
        public override Version Version => new Version(1, 1, 1);
        public override string Author => "Soofa";
        public override string Description => "Let's users trade items with the server";

        public ServerTrade(Main game) : base(game) { }

        public static string path = Path.Combine(TShock.SavePath + "/ServerTradeConfig.json");
        public static Config Config = new Config();

        public override void Initialize() {
            GeneralHooks.ReloadEvent += OnReload;
            Commands.ChatCommands.Add(new Command("servertrade.trade", TradeCmd, "trade") {
                AllowServer = false,
                HelpText = "Usage: \"/trade ItemName\" (Case sensitive)"
            });

            if (File.Exists(path)) {
                Config = Config.Read();
            }
            else {
                Config.Write();
            }
        }
        private void OnReload(ReloadEventArgs e) {
            if (File.Exists(path)) {
                Config = Config.Read();
            }
            else {
                Config.Write();
            }
            e.Player.SendSuccessMessage("ServerTrade plugin has been reloaded.");
        }
        public void TradeCmd(CommandArgs args) {
            TSPlayer Player = args.Player;
            if(args.Parameters.Count == 0) {
                Player.SendErrorMessage("No item name given.");
                return;
            }

            int amount, givenAmount;
            string itemName;
            if (int.TryParse(args.Parameters[^1], out amount)) {
                itemName = string.Join("", args.Parameters.GetRange(0, args.Parameters.Count - 1));
            }
            else {
                amount = 1;
                itemName = string.Join("", args.Parameters);
            }

            int[]? offer = null;
            foreach(var kvp in Config.shopList) {
                if(kvp.Key.ToLowerInvariant().Replace(" ", "").Equals(itemName.ToLowerInvariant())) {
                    offer = kvp.Value;
                    break;
                }    
            }

            if (offer == null) {
                Player.SendErrorMessage("Item not found.");
                return;
            }


            for (int i = givenAmount = 0; i < NetItem.InventorySlots && amount > givenAmount; i++) {
                if (Player.TPlayer.inventory[i].netID == offer[2] && Player.TPlayer.inventory[i].stack >= offer[3]) {
                    // 0: item id, 1: item amount, 2: itemreq id, 3: itemreq amount
                    Player.TPlayer.inventory[i].stack -= offer[3];
                    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(Player.TPlayer.inventory[i].Name), Player.Index, i);
                    NetMessage.SendData((int)PacketTypes.PlayerSlot, Player.Index, -1, NetworkText.FromLiteral(Player.TPlayer.inventory[i].Name), Player.Index, i);
                    Player.GiveItem(offer[0], offer[1]);
                    givenAmount++;
                    i--;
                }
            }

            if (givenAmount == 0) {
                Player.SendErrorMessage($"You do not have {offer[3]} {TShock.Utils.GetItemById(offer[2]).Name}.");
            }
            else if (amount > givenAmount) {
                Player.SendErrorMessage($"You could only afford {givenAmount} {TShock.Utils.GetItemById(offer[0]).Name}.");
            }
            else {
                Player.SendSuccessMessage("Trade was succesful!");
            }

            return;
        }
    }
}
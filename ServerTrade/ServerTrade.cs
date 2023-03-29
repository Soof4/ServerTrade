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
        public override Version Version => new Version(1, 1);
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
            string itemName = args.Parameters[0];
            for(int i=1; i<args.Parameters.Count; i++) {
                itemName += args.Parameters[i];
            }

            foreach(var kvp in Config.shopList) {
                if(kvp.Key.ToLowerInvariant().Equals(itemName.ToLowerInvariant())) {
                    for (int i = 0; i < NetItem.InventorySlots; i++) {
                        if (Player.TPlayer.inventory[i].netID == kvp.Value.ToArray()[2] && Player.TPlayer.inventory[i].stack >= kvp.Value.ToArray()[3]) {
                            // 0 = itemget id, 1 = itemget amount, 2 itemrequired id, 3 = itemreq amount
                            Player.TPlayer.inventory[i].stack -= kvp.Value.ToArray()[3];
                            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(Player.TPlayer.inventory[i].Name), Player.Index, i);
                            NetMessage.SendData((int)PacketTypes.PlayerSlot, Player.Index, -1, NetworkText.FromLiteral(Player.TPlayer.inventory[i].Name), Player.Index, i);
                            Player.GiveItem(kvp.Value.ToArray()[0], kvp.Value.ToArray()[1]);
                            Player.SendSuccessMessage("Trade was succesful!");
                            return;
                        }
                    }
                    Player.SendErrorMessage($"You do not have {kvp.Value.ToArray()[3]} {TShock.Utils.GetItemById(kvp.Value.ToArray()[2]).Name}.");
                    return;
                }
            }
            Player.SendErrorMessage("Item not found.");
            return;
        }
    }
}
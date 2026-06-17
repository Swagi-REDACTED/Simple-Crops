using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimpleCrops
{
	public class HerbLiquidGlobalTile : GlobalTile
	{
		public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
		{
			var config = ModContent.GetInstance<SimpleCropsConfig>();

			if (type == TileID.ImmatureHerbs || type == TileID.MatureHerbs || type == TileID.BloomingHerbs)
			{
				Tile tile = Main.tile[i, j];
				int style = tile.TileFrameX / 18;

				if (style == 5) // Fireblossom
				{
					if (!config.LavaDestroysFireblossom)
					{
						// If submerged in lava, completely prevent breaking
						if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Lava)
						{
							return false;
						}
					}
				}
			}

			return base.CanKillTile(i, j, type, ref blockDamaged);
		}

		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			var config = ModContent.GetInstance<SimpleCropsConfig>();

			if (type == TileID.ImmatureHerbs || type == TileID.MatureHerbs || type == TileID.BloomingHerbs)
			{
				Tile tile = Main.tile[i, j];
				int style = tile.TileFrameX / 18;

				if (style == 5) // Fireblossom
				{
					if (!config.LavaDestroysFireblossom)
					{
						// Intercept liquid update kills (and pickaxe mining while in lava)
						if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Lava)
						{
							fail = true;
						}
					}
				}
			}
		}
	}
}

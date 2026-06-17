using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimpleCrops
{
	public class HerbBloomSystem : ModSystem
	{
		// Store an explicit Hook so we can Dispose() it in Unload().
		// The On_ shorthand never disposes its internal Hook, causing MonoMod to
		// hold a strong reference to our delegate even after unloading the mod.
		private Hook _growAlchHook;

		public override void Load()
		{
			var target = typeof(WorldGen).GetMethod(nameof(WorldGen.GrowAlch), BindingFlags.Public | BindingFlags.Static);
			_growAlchHook = new Hook(target, Hook_GrowAlch);
		}

		public override void Unload()
		{
			// Dispose explicitly — this releases MonoMod's internal strong reference
			// to our delegate, allowing the GC to collect the mod assembly cleanly.
			_growAlchHook?.Dispose();
			_growAlchHook = null;
		}

		private bool HasLiquid(int x, int y, int liquidType, bool requireSubmergence)
		{
			// 1x2 check (strictly plant and soil below it)
			if (requireSubmergence)
			{
				if (Main.tile[x, y].LiquidAmount > 0 && Main.tile[x, y].LiquidType == liquidType) return true;
				if (WorldGen.InWorld(x, y + 1) && Main.tile[x, y + 1].LiquidAmount > 0 && Main.tile[x, y + 1].LiquidType == liquidType) return true;
				return false;
			}

			// 3x4 check (generous 1-block radius)
			for (int k = x - 1; k <= x + 1; k++)
			{
				for (int l = y - 1; l <= y + 2; l++)
				{
					if (WorldGen.InWorld(k, l))
					{
						if (Main.tile[k, l].LiquidAmount > 0 && Main.tile[k, l].LiquidType == liquidType)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Delegate signature for the explicit Hook — matches WorldGen.GrowAlch(int, int)
		private delegate void orig_GrowAlch(int x, int y);

		private void Hook_GrowAlch(orig_GrowAlch orig, int x, int y)
		{
			// Call the original vanilla code first (runs its own 1/50 growth roll and all other tile logic)
			orig(x, y);

			// Re-read tile — orig may have already grown or destroyed it
			Tile tile = Main.tile[x, y];
			var config = ModContent.GetInstance<SimpleCropsConfig>();

			if (!tile.HasTile) return;

			// --- IMMATURE -> MATURE (Submergence Growth Boost) ---
			// Vanilla rain gives Waterleaf a SECOND independent 1/50 roll layered on top of the normal one.
			// We replicate this exactly: orig ran its 1/50 roll above. If the plant is STILL Immature
			// (orig's roll failed), we apply our own extra 1/50 chance when submerged.
			// No double-success is possible — if orig succeeded, tile is now MatureHerbs and we skip this.
			if (tile.TileType == TileID.ImmatureHerbs)
			{
				int style = tile.TileFrameX / 18;

				if (style == 4 && config.ApplySubmergenceBoostToWaterleaf && !Main.raining) // Waterleaf (no rain stack)
				{
					if (HasLiquid(x, y, LiquidID.Water, true)) // growth boost ALWAYS uses 1x2 check
					{
						if (Main.rand.Next(50) == 0)
						{
							tile.TileType = TileID.MatureHerbs;
							if (Main.netMode == NetmodeID.Server) NetMessage.SendTileSquare(-1, x, y, 1);
						}
					}
				}
				else if (style == 5 && config.ApplySubmergenceBoostToFireblossom) // Fireblossom
				{
					if (HasLiquid(x, y, LiquidID.Lava, true)) // growth boost ALWAYS uses 1x2 check
					{
						if (Main.rand.Next(50) == 0)
						{
							tile.TileType = TileID.MatureHerbs;
							if (Main.netMode == NetmodeID.Server) NetMessage.SendTileSquare(-1, x, y, 1);
						}
					}
				}
			}

			// Re-read tile again in case we just grew it above
			tile = Main.tile[x, y];
			if (!tile.HasTile) return;

			// --- MATURE -> BLOOMING (Bloom Logic) ---
			if (tile.TileType == TileID.MatureHerbs)
			{
				int style = tile.TileFrameX / 18;

				if (config.EnableWaterleafWaterBloom && style == 4) // Waterleaf
				{
					if (HasLiquid(x, y, LiquidID.Water, config.WaterleafRequiresSubmergence))
					{
						tile.TileType = TileID.BloomingHerbs;
						if (Main.netMode == NetmodeID.Server) NetMessage.SendTileSquare(-1, x, y, 1);
					}
				}
				else if (config.EnableFireblossomLavaBloom && style == 5) // Fireblossom
				{
					if (HasLiquid(x, y, LiquidID.Lava, config.FireblossomRequiresSubmergence))
					{
						if (!config.RainStopsFireblossomWhenSubmerged || !Main.raining)
						{
							tile.TileType = TileID.BloomingHerbs;
							if (Main.netMode == NetmodeID.Server) NetMessage.SendTileSquare(-1, x, y, 1);
						}
					}
				}
			}
		}
	}
}

using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SimpleCrops
{
	public class SimpleCropsConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[Label("Enable Waterleaf Water Bloom")]
		[Tooltip("If enabled, Waterleaf will bloom when submerged in water.")]
		[DefaultValue(true)]
		public bool EnableWaterleafWaterBloom;

		[Label("Enable Fireblossom Lava Bloom")]
		[Tooltip("If enabled, Fireblossom will bloom when submerged in lava or if lava is within a 1 block radius.")]
		[DefaultValue(true)]
		public bool EnableFireblossomLavaBloom;

		[Label("Lava Destroys Fireblossom")]
		[Tooltip("If true, pouring lava directly onto the leaves will incinerate it (vanilla behavior), but putting it in the planter is safe.\nIf false, it will be completely immune to lava.")]
		[DefaultValue(true)]
		public bool LavaDestroysFireblossom;

		[Label("Waterleaf Requires Submergence")]
		[Tooltip("If enabled, Waterleaf will only bloom if water is directly on the plant or in its planter block.\nIf disabled, a generous 1-block radius is checked.")]
		[DefaultValue(true)]
		public bool WaterleafRequiresSubmergence;

		[Label("Fireblossom Requires Submergence")]
		[Tooltip("If enabled, Fireblossom will only bloom if lava is directly on the plant or in its planter block.\nIf disabled, a generous 1-block radius is checked.")]
		[DefaultValue(false)]
		public bool FireblossomRequiresSubmergence;

		[Label("Apply Submergence Boost to Waterleaf")]
		[Tooltip("If enabled, applies a growth speed boost to Waterleaf when submerged or when water is in its planter block.\nThis replicates the vanilla rain boost, and does not stack with it.")]
		[DefaultValue(true)]
		public bool ApplySubmergenceBoostToWaterleaf;

		[Label("Apply Submergence Boost to Fireblossom")]
		[Tooltip("If enabled, applies a growth speed boost to Fireblossom when submerged or when lava is in its planter block.")]
		[DefaultValue(true)]
		public bool ApplySubmergenceBoostToFireblossom;

		[Label("Rain Stops Fireblossom When Submerged")]
		[Tooltip("If enabled, Fireblossoms submerged in lava will un-bloom when it rains, matching vanilla behavior.\nIf disabled, lava-submerged Fireblossoms will stay blooming even during rain.")]
		[DefaultValue(false)]
		public bool RainStopsFireblossomWhenSubmerged;
	}
}

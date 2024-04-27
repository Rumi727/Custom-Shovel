using BepInEx.Configuration;
using LethalConfig.ConfigItems.Options;
using LethalConfig.ConfigItems;
using LethalConfig;

namespace Rumi.CustomShovel
{
    public class CustomShovelConfig
    {
        public float radius
        {
            get => _radius?.Value ?? dRadius;
            set => _radius.Value = value;
        }
        readonly ConfigEntry<float> _radius;
        public const float dRadius = 0.8f;

        public float maxDistance
        {
            get => _maxDistance?.Value ?? dMaxDistance;
            set => _maxDistance.Value = value;
        }
        readonly ConfigEntry<float> _maxDistance;
        public const float dMaxDistance = 1.5f;

        public bool showShovelHitbox
        {
            get => _showShovelHitbox?.Value ?? dShowShovelHitbox;
            set => _showShovelHitbox.Value = value;
        }
        readonly ConfigEntry<bool> _showShovelHitbox;
        public const bool dShowShovelHitbox = false;

        public CustomShovelConfig(ConfigFile config)
        {
            _radius = config.Bind("General", "Radius", dRadius, "Sets the radius of the hitbox circle.");
            _radius.SettingChanged += (sender, e) => CustomShovelPatches.ShovelPatchUpdate();

            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(_radius, new FloatSliderOptions() { Min = 0.1f, Max = 10f, RequiresRestart = false }));

            _maxDistance = config.Bind("General", "Max Distance", dMaxDistance, "Determines the max distance of the hitbox");
            _maxDistance.SettingChanged += (sender, e) => CustomShovelPatches.ShovelPatchUpdate();

            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(_maxDistance, new FloatSliderOptions() { Min = 0.1f, Max = 10f, RequiresRestart = false }));

            _showShovelHitbox = config.Bind("Debug", "Show Shovel Hitbox", dShowShovelHitbox, "Shows the shovel's hitbox with a line.");
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(_showShovelHitbox, false));
        }
    }
}

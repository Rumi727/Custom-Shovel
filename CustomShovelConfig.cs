using BepInEx.Configuration;
using LethalConfig.ConfigItems.Options;
using LethalConfig.ConfigItems;
using LethalConfig;
using System.IO;
using System;

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


            _maxDistance = config.Bind("General", "Max Distance", dMaxDistance, "Determines the max distance of the hitbox");
            _maxDistance.SettingChanged += (sender, e) => CustomShovelPatches.ShovelPatchUpdate();

            _showShovelHitbox = config.Bind("Debug", "Show Shovel Hitbox", dShowShovelHitbox, "Shows the shovel's hitbox with a line.");

            try
            {
                LethalConfigPatch();
            }
            catch (FileNotFoundException e)
            {
                CustomShovel.logger?.LogError(e);
                CustomShovel.logger?.LogWarning("Lethal Config Patch Fail! (This is not a bug and occurs when LethalConfig is not present)");
            }
            catch (Exception e)
            {
                CustomShovel.logger?.LogError(e);
                CustomShovel.logger?.LogError("Lethal Config Patch Fail!");
            }
        }

        void LethalConfigPatch()
        {
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(_radius, new FloatSliderOptions() { Min = 0.1f, Max = 10f, RequiresRestart = false }));
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(_maxDistance, new FloatSliderOptions() { Min = 0.1f, Max = 10f, RequiresRestart = false }));
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(_showShovelHitbox, false));
        }
    }
}

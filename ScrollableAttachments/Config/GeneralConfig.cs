using AttachmentScrolling.Attributes;
using AttachmentScrolling.Components;
using BepInEx.Configuration;

namespace AttachmentScrolling.Config;

public static class GeneralConfig
{
    public static ConfigEntry<float> ScrollSpeed { get; set; }
    public static ConfigEntry<int> GridColumns { get; set; }
    public static ConfigEntry<float> ViewHeight { get; set; }

    private static bool _initialized;

    public static void Initialize(ConfigFile config)
    {
        if (_initialized)
        {
            return;
        }

        ScrollSpeed = config.Bind("General", "Scroll speed", 32f, new ConfigDescription(
            "Changes the scroll speed of the attachment menu",
            new AcceptableValueRange<float>(0f, 100f),
            new ConfigurationManagerAttributes() {Order = 1000}
        ));

        GridColumns = config.Bind("General", "Attachment menu columns", 6, new ConfigDescription(
            "Changes the amount of columns shown in the attachment menu",
            new AcceptableValueRange<int>(1, 16),
            new ConfigurationManagerAttributes() {Order = 990}
        ));

        ViewHeight = config.Bind("General", "Attachment menu height", 420f, new ConfigDescription(
            "Changes the height of the attachment menu",
            new AcceptableValueRange<float>(0f, 1080f),
            new ConfigurationManagerAttributes() {Order = 980}
        ));

        ScrollSpeed.SettingChanged += (_, _) => AttachmentScrollComponent.ApplySettings();
        GridColumns.SettingChanged += (_, _) => AttachmentScrollComponent.ApplySettings();
        ViewHeight.SettingChanged += (_, _) => AttachmentScrollComponent.ApplySettings();

        _initialized = true;
        AttachmentScrollComponent.ApplySettings();
    }
}

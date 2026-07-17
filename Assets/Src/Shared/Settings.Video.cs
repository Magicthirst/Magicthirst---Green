using System;
using System.Linq;
using UnityEngine;

namespace Shared
{
    public static partial class Settings
    {
        [Serializable]
        public class Visual
        {
            public event Action<FullScreenMode> ScreenModeChanged;
            public event Action<Resolution> ResolutionChanged;
            public event Action<RefreshRate> RefreshRateChanged;

            private const string ScreenModeKey = "SETTINGS/VIDEO/SCREEN_MODE";
            private const string ResolutionKey = "SETTINGS/VIDEO/RESOLUTION";

            public Visual()
            {
                ScreenModeChanged += _ => Resolution = AvailableResolutions[^1];
                ResolutionChanged += r => RefreshRateChanged?.Invoke(r.refreshRateRatio);
            }

            public void Init()
            {
                if (!PlayerPrefs.HasKey(ScreenModeKey))
                {
                    ScreenMode = FullScreenMode.FullScreenWindow;
                }

                if (!PlayerPrefs.HasKey(ResolutionKey))
                {
                    Resolution = AvailableResolutions[^1];
                }
            }

            public FullScreenMode ScreenMode
            {
                get => Enum.TryParse<FullScreenMode>(PlayerPrefs.GetString(ScreenModeKey), out var mode) ? mode : FullScreenMode.MaximizedWindow;
                set
                {
                    PlayerPrefs.SetString(ScreenModeKey, value.ToString());
                    PlayerPrefs.Save();

                    UpdateResolutionSettings();

                    ScreenModeChanged?.Invoke(value);
                }
            }

            public Resolution Resolution
            {
                get
                {
                    var value = PlayerPrefs.GetString(ResolutionKey);

                    if (string.IsNullOrEmpty(value))
                    {
                        return AvailableResolutions[^1];
                    }

                    var parts = value.Split('x', '@', '/');
                    if (parts.Length != 2)
                    {
                        return AvailableResolutions[^1];
                    }

                    if
                    (
                        !int.TryParse(parts[0], out var width) ||
                        !int.TryParse(parts[1], out var height) ||
                        !uint.TryParse(parts[1], out var rrNum) ||
                        !uint.TryParse(parts[1], out var rrDen)
                    )
                    {
                        return AvailableResolutions[^1];
                    }

                    foreach (var r in AvailableResolutions)
                    {
                        var rr = r.refreshRateRatio;

                        if
                        (
                            r.width == width &&
                            r.height == height &&
                            rr.numerator == rrNum &&
                            rr.denominator == rrDen
                        )
                        {
                            return r;
                        }
                    }

                    return AvailableResolutions[^1];
                }

                set
                {
                    foreach (var resolution in AvailableResolutions)
                    {
                        if (resolution.width == value.width && resolution.height == value.height)
                        {
                            value = resolution;
                        }
                    }

                    PlayerPrefs.SetString
                    (
                        ResolutionKey,
                        $"{value.width}" +
                        $"x{value.height}" +
                        $"@{value.refreshRateRatio.numerator}" +
                        $"/{value.refreshRateRatio.denominator}"
                    );
                    PlayerPrefs.Save();

                    UpdateResolutionSettings();

                    ResolutionChanged?.Invoke(value);
                }
            }

            public RefreshRate RefreshRate
            {
                get => Resolution.refreshRateRatio;
                set => Resolution = new Resolution
                {
                    width = Resolution.width,
                    height = Resolution.height,
                    refreshRateRatio = value
                };
            }

            public Resolution[] AvailableResolutions => Screen.resolutions
                .GroupBy(r => (r.width, r.height))
                .Select(g => g.First())
                .ToArray();

            public RefreshRate[] AvailableRates => Screen.resolutions
                .Where(r => r.width == Resolution.width && r.height == Resolution.height)
                .Select(r => r.refreshRateRatio)
                .ToArray();

            private void UpdateResolutionSettings()
            {
                var width = Resolution.width;
                var height = Resolution.height;
                var refreshRate = Resolution.refreshRateRatio;

                Screen.SetResolution(width, height, ScreenMode, refreshRate);
            }
        }
    }
}
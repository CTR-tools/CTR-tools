using System;
using System.Collections.Generic;
using CTRFramework;
using Locale = ctrviewer.Resources.Localization;

namespace ctrviewer.Engine.Gui
{
    public class MenuFactory
    {
        static EngineSettings settings => EngineSettings.Instance;

        public static List<MenuItem> CreateMainMenu()
        {
            return new List<MenuItem>() {
                new MenuItem(Locale.MainMenu_Resume, "close", "", true),
                //new MenuItem("reload level", "load", "", true),
                new MenuItem(Locale.MainMenu_LoadLevel, "link", "cupmenu", Game1.BigFileExists),
                new MenuItem(Locale.MainMenu_LevelOptions, "link", "level", true),
                new MenuItem(Locale.MainMenu_VideoOptions, "link", "video", true),
                new MenuItem(Locale.MainMenu_GeneralOptions, "link", "general", true),
                new BoolMenuItem() { Text = Locale.MainMenu_KartMode, Name = "kart", Value = settings.KartMode },
                //new MenuItem("Prev QuadBlock", "prevblock", "", true),
                //new MenuItem("Next QuadBlock", "nextblock", "", true),
                //new MenuItem("Open settings file", "settings", "", true),
                new MenuItem(Locale.MainMenu_Quit, "exit", "", true),
            };
        }

        public static List<MenuItem> CreateLevelOptionsMenu()
        {
            var flagtitles = new List<(int, string)>() { (-1, "None") };

            foreach (int i in Enum.GetValues(typeof(QuadFlags)))
            {
                if (i == 0 || i == -1) continue;
                flagtitles.Add((i, ((QuadFlags)i).ToString()));
            }

            return new List<MenuItem>()
            {
                new BoolMenuItem() { Text = Locale.VideoMenu_Wireframe, Name = "wire", Value = settings.DrawWireframe, HelperText = "Toggles wireframe." },
                new BoolMenuItem() { Text = Locale.VideoMenu_Replacements, Name = "newtex", Value = settings.UseTextureReplacements, HelperText = "Toggles custom texture replacements from NEWTEX folder." },
                new BoolMenuItem() { Text = Locale.VideoMenu_VertexLighting, Name = "vcolor", Value = settings.VertexLighting, HelperText = "Toggles baked vertex lighting." },
                new BoolMenuItem() { Text = Locale.VideoMenu_Textures, Name = "textures", Value = settings.Textures, HelperText = "Toggles texture rendering." },
                new BoolMenuItem() { Text = Locale.VideoMenu_BackfaceCulling, Name = "nocull", Value = settings.BackFaceCulling, HelperText = "Toggle backface culling (polygons rendered from both sides)." },
                new BoolMenuItem() { Text = Locale.VideoMenu_Skybox, Name = "skybox", Value = settings.ShowSky, HelperText = "Toggles level skybox." },
                new BoolMenuItem() { Text = Locale.VideoMenu_Water, Name = "water", Value = settings.ShowWater, HelperText = "Toggles meshes marked as water." },
                new BoolMenuItem() { Text = Locale.VideoMenu_InvisibleMeshes, Name = "invis", Value = settings.ShowInvisible, HelperText = "Toggles invisible meshes (limiting walls, script triggers, etc." },
                new BoolMenuItem() { Text = Locale.VideoMenu_VisibilityTree, Name = "visbox", Value = settings.VisData, HelperText = "Renders visibility BSP tree as a set of bounding boxes." },
                new BoolMenuItem() { Text = Locale.VideoMenu_RenderBranches, Name = "visboxleaf", Value = settings.VisDataLeaves, Enabled = settings.VisData, HelperText = "Toggles visibility tree branches." },
                new BoolMenuItem() { Text = Locale.VideoMenu_GameObjects, Name = "inst", Value = settings.ShowModels, HelperText = "Toggles instanced 'foreground' models in the level (boxes, hazards, etc.)" },
                new BoolMenuItem() { Text = Locale.VideoMenu_BotPaths, Name = "paths", Value = settings.ShowBotPaths, HelperText = "Toggles bot paths rendered as a set of points." },
                new MenuItem(Locale.VideoMenu_ToggleLod, "toggle", "lod", true),
                new IntRangeMenuItem() { Text = Locale.VideoMenu_QuadFlag, Name = "flag", SelectedValue = 0, Values = flagtitles, HelperText = "Highlights a specific quadblock flag in the level." },
                new MenuItem(Locale.MenuGeneric_Back, "link", "main", true)
            };
        }

        public static List<MenuItem> CreateVideoOptionsMenu()
        {
            return new List<MenuItem>()
            {
                new BoolMenuItem() { Text = Locale.VideoMenu_Windowed, Name = "window", Value = settings.Windowed, HelperText = "Toggles fullscreen and windowed modes." },
                new BoolMenuItem() { Text = Locale.VideoMenu_Vsync, Name = "vsync", Value = settings.VerticalSync, HelperText = "Toggles vertical synchronization." },
                //new BoolMenuItem() { Text = Locale.VideoMenu_30fps, Name = "fps30", Value = settings.Fps30, Enabled = settings.VerticalSync },
                new BoolMenuItem() { Text = Locale.VideoMenu_Antialias, Name = "antialias", Value = settings.AntiAlias, HelperText = "Toggles 4x MSAA." },
                new BoolMenuItem() { Text = Locale.VideoMenu_Filtering, Name = "filter", Value = settings.EnableFiltering, HelperText = "Toggles texture filtering." },
                new IntRangeMenuItem() { Text = Locale.VideoMenu_Anisotropy, Name = "aniso", Enabled = settings.EnableFiltering, SelectedValue = settings.AnisotropyLevel, Values = new List<(int, string)>() { (1, "1x"), (2, "2x"), (4, "4x"), (8, "8x"), (16, "16x") }, HelperText = "Improves texture sharpness at steep angles."},
                new BoolMenuItem() { Text = Locale.VideoMenu_GenerateMipmaps, Name = "genmips", Value = settings.GenerateMips, HelperText = "Reduces texture flickering in the distance, requires level restart." },
                new BoolMenuItem() { Text = Locale.VideoMenu_InternalResolution, Name = "intpsx", Value = settings.InternalPSXResolution, HelperText = "Toggles between full and internal PSX resolution." },
                new BoolMenuItem() { Text = Locale.VideoMenu_StereoMode, Name = "stereo", Value = settings.StereoPair, HelperText = "Toggles stereoscopic (VR) mode" },
                new BoolMenuItem() { Text = Locale.VideoMenu_Crosseyed, Name = "crosseyed", Value = settings.StereoCrossEyed, Enabled = settings.StereoPair, HelperText = "Swaps left and right eye image." },
                new MenuItem(Locale.MenuGeneric_Back, "link", "main", true)
            };
        }

        public static List<MenuItem> CreateCupMenu()
        {
            return new List<MenuItem>() {
                new MenuItem("Level type", "link", "level_type", true),
                new MenuItem("Wumpa cup", "link", "cup_wumpa", true),
                new MenuItem("Crystal cup", "link", "cup_cryst", true),
                new MenuItem("Nitro cup", "link", "cup_nitro", true),
                new MenuItem("Crash cup", "link", "cup_crash", true),
                new MenuItem("Bonus tracks", "link", "bonus_levels", true),
                new MenuItem("Battle arenas", "link", "battle_arenas", true),
                new MenuItem("Adventure", "link", "adventure", true),
                new MenuItem("Cutscenes", "link", "cutscenes", true),
                new MenuItem("Custom levels", "link", "custom_levels", true),
                new MenuItem(Locale.MenuGeneric_Back, "link", "main", true)
            };
        }
    }
}
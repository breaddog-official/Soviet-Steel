using NaughtyAttributes;
using System;
using System.Linq;
using System.Reflection;
using Unity.Burst;
using UnityEngine;

namespace Scripts.TranslateManagement
{
    [BurstCompile, Serializable]
    public class Translation
    {
        [Header("Global")]
        public string back;
        public string loading;
        public string none;
        [Header("Menu")]
        public string menu_play;
        public string menu_settings;
        public string menu_quit;
        public string menu_garage;
        public string menu_region;
        public string menu_join;
        public string menu_bots_count;
        public string menu_previous;
        public string menu_next;
        public string menu_select;
        [Header("Menu: Settings")]
        public string menu_settings_audio;
        public string menu_settings_graphics;
        public string menu_settings_shadows;
        public string menu_settings_other;
        [Space]
        public string menu_settings_volume_sounds;
        public string menu_settings_volume_music;
        public string menu_settings_volume_ambient;
        [Space]
        public string menu_settings_geometry_quality;
        public string menu_settings_shaders_quality;
        public string menu_settings_resolution_type;
        public string menu_settings_antialiasing;
        public string menu_settings_render_distance;
        public string menu_settings_fog_quality;
        public string menu_settings_fullscreen_mode;
        public string menu_settings_style_mode;
        public string menu_settings_mirror_quality;
        public string menu_settings_shadows_mode;
        public string menu_settings_shadows_quality;
        public string menu_settings_shadows_sources;
        [Space]
        public string menu_settings_language;
        public string menu_settings_enable_vsync;
        [Space]
        public string menu_settings_color_blind;
        public string menu_settings_color_blind_protanopia;
        public string menu_settings_color_blind_deuteranopia;
        public string menu_settings_color_blind_tritanopia;
        [Space]
        public string menu_settings_windowed;
        public string menu_settings_maximized;
        public string menu_settings_fullscreen;
        public string menu_settings_exclusive;
        [Space]
        public string menu_settings_perfomance_level_worst;
        public string menu_settings_perfomance_level_bad;
        public string menu_settings_perfomance_level_not_bad;
        public string menu_settings_perfomance_level_normal;
        public string menu_settings_perfomance_level_good;
        public string menu_settings_perfomance_level_excellent;
        public string menu_settings_perfomance_level_excess;
        [Space]
        public string menu_settings_low;
        public string menu_settings_medium;
        public string menu_settings_high;
        public string menu_settings_ultra;
        public string menu_settings_maximum;
        public string menu_settings_full;
        public string menu_settings_simple;
        public string menu_settings_volumetric;
        public string menu_settings_standart;
        public string menu_settings_advanced;
        [Space]
        public string menu_settings_style_modern;
        public string menu_settings_style_retro;
        [Space]
        public string menu_settings_shadows_only_sun;
        public string menu_settings_shadows_everything;

        [Header("Menu: join")]
        public string menu_join_joinGame;
        public string menu_join_find;
        public string menu_join_manual;
        public string menu_join_manual_address;
        public string menu_join_manual_port;

        [Header("Menu: server find")]
        public string menu_server_players;
        public string menu_server_maxPlayers;

        [Header("Menu: levels")]
        public string menu_levels_tundra;
        [ResizableTextArea]
        public string menu_levels_tundra_description;
        [Space]
        public string menu_levels_ural;
        [ResizableTextArea]
        public string menu_levels_ural_description;
        [Space]
        public string menu_levels_blacksea;
        [ResizableTextArea]
        public string menu_levels_blacksea_description;
        [Space]
        public string menu_levels_saratov;
        [ResizableTextArea]
        public string menu_levels_saratov_description;
        [Space]
        public string menu_levels_hills;
        [ResizableTextArea]
        public string menu_levels_hills_description;
        [Space]
        public string menu_levels_darkforest;
        [ResizableTextArea]
        public string menu_levels_darkforest_description;
        [Space]
        public string menu_levels_fields;
        [ResizableTextArea]
        public string menu_levels_fields_description;

        [Header("Menu: cars")]
        public string menu_cars_21099;
        [ResizableTextArea]
        public string menu_cars_21099_description;
        [Space]
        public string menu_cars_2101;
        [ResizableTextArea]
        public string menu_cars_2101_description;
        [Space]
        public string menu_cars_uaz;
        [ResizableTextArea]
        public string menu_cars_uaz_description;
        [Space]
        public string menu_cars_2106;
        [ResizableTextArea]
        public string menu_cars_2106_description;
        [Space]
        public string menu_cars_2108;
        [ResizableTextArea]
        public string menu_cars_2108_description;
        [Space]
        public string menu_cars_volga;
        [ResizableTextArea]
        public string menu_cars_volga_description;
        [Space]
        public string menu_cars_prototype;
        [ResizableTextArea]
        public string menu_cars_prototype_description;

        [Header("Game")]
        public string game_leave;
        public string game_start_match;
        public string game_enter_nickname;

        [Header("Game: Score")]
        public string game_score_title;
        public string game_time;






        /// <summary>
        /// Applies the new translation only to empty fields
        /// </summary>
        public void SetToEmptyFields(Translation newTranslation)
        {
            FieldInfo[] fields = typeof(Translation).GetFields();
            FieldInfo[] emptyFields = fields
                                      .Where(f => string.IsNullOrWhiteSpace(f.GetValue(this) as string))
                                      .ToArray();

            // Set values only to empty fields
            foreach (FieldInfo field in emptyFields)
            {
                field.SetValue(this, field.GetValue(newTranslation));
            }
        }
    }
}

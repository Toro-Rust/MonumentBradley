using Oxide.Core.Plugins;
using UnityEngine;
using Rust;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("MonumentBradley", "Yac Vaguer", "0.0.1")]
    class MonumentBradley : RustPlugin
    {

        private PluginConfig? config;
        protected override void SaveConfig() => Config.WriteObject(config);
        private MissionZone? missionZone;


        private void SpawnAllBradley()
        {

        }

        void OnServerInitialized()
        {

            missionZone = new MissionZone();

            timer.Every(config.eventInterval, () =>
            {
                DespawnBradley();
                SpawnAllBradley();
            });
        }

        private class BradleyOnMonuments
        {
            private Dictionary<string, bool> monuments = new Dictionary<string, bool>
            {
                { "airfield", true },
                { "harbor", true },
                { "junkyard", true },
                { "military Tunnels", true },
                { "power Plant", true },
                { "satellite Dish", true },
                { "sewer Branch", true },
                { "train uard", true },
                { "water treatment plant", true },
            };

            public BradleyOnMonuments()
            {
                foreach (var monument in TerrainMeta.Path.Monuments)
                {
                    string monumentKey = monument.displayPhrase.english.ToLower();
                    if (monuments.TryGetValue(monumentKey, out bool isEnabled) && isEnabled)
                    {
                        //Monument enabled with bradley
                    }

                }
            }
        }

        protected override void LoadDefaultConfig()
        {
            config = PluginConfig.DefaultConfig();
            SaveConfig();
            Log("Creation of the configuration file completed");
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            config = Config.ReadObject<PluginConfig>();
            Log("Loading existing config");
        }

        private class PluginConfig
        {
            [JsonProperty("Respawn Bradley every [sec]")] public float eventInterval { get; set; }

            [JsonProperty("Activate Bradley on Water Treatment ")] public bool inWaterTreatment { get; set; }
            [JsonProperty("Activate Bradley on Airfield ")] public bool inAirfield { get; set; }
            [JsonProperty("Activate Bradley on Satelite Dish")] public bool inSateliteDish { get; set; }
            [JsonProperty("Activate Bradley on Military Silo")] public bool inMilitarySilo { get; set; }
            [JsonProperty("Activate Bradley on Sewer Branch")] public bool inSewerBranch { get; set; }

            [JsonProperty("Activate Developer verbosity?")] public bool debug { get; set; }

            public static PluginConfig DefaultConfig()
            {

                return new PluginConfig()
                {
                    // Timing
                    eventInterval = 7200f,

                    // Monuments to activate Bradley
                    inWaterTreatment = true,
                    inAirfield = true,
                    inMilitarySilo = true,
                    inSateliteDish = true,
                    inSewerBranch = true,

                    // Developer only
                    debug = false
                    
                };
            }
        }

        private class MissionZone
        {
            private MonumentInfo eventLocation;
            private Vector3? eventCenterLocation = null;
            private readonly float _zoneRadius = 120f;
            private string monumentName = "water treatment plant";

            public MissionZone()
            {
                foreach (var monument in TerrainMeta.Path.Monuments)
                {
                    if (monument.displayPhrase.english.ToLower().Contains(monumentName))
                    {
                        eventLocation = monument;
                        return;
                    }
                }

                throw new InvalidOperationException("Water Treatment Plant monument not found. This plugin only works in the Water Treatment Plant monument");
            }

            public Vector3 EventCenter
            {
                get
                {
                    if (!eventCenterLocation.HasValue)
                    {
                        Vector3 newCenter = new Vector3(-13.62f, 4.36f, -69.07f);
                        eventCenterLocation = eventLocation.transform.TransformPoint(newCenter);
                    }

                    return eventCenterLocation.Value; // Use the Value property for nullable Vector3
                }
            }

            public float ZoneRadius
            {
                get { return _zoneRadius; }
            }

            public MonumentInfo Location
            {
                get { return eventLocation; }
            }
        }

        private void Log(string message)
        {
            if (config.debug)
            {
                Puts(message);
            }
        }
    }

}

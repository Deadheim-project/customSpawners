using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Linq;
using System.IO;
using UnityEngine;
using Jotunn.Managers;
using System.Collections.Generic;
using Jotunn.Utils;

namespace CustomSpawners
{
    [BepInPlugin(PluginGUID, PluginGUID, Version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class CustomSpawners : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.CustomSpawners";
        public const string Version = "1.0.5";
        Harmony harmony = new Harmony(PluginGUID);
        public static JsonLoader root = new JsonLoader();
        public static bool hasSpawned = false;
        public static readonly string ModPath = Path.GetDirectoryName(typeof(CustomSpawners).Assembly.Location);

        public static ConfigEntry<bool> IsSinglePlayer;

        public static string FileDirectory = BepInEx.Paths.ConfigPath + @"/Detalhes.CustomOferringBowls.json";

        private void Awake()
        {
            Config.SaveOnConfigSet = true;

            IsSinglePlayer = Config.Bind("Server config", "IsSinglePlayer", false,
                    new ConfigDescription("IsSinglePlayer", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                if (attr.InitialSynchronization)
                {
                    Jotunn.Logger.LogMessage("Config sync event received");
                }
                else
                {
                    Jotunn.Logger.LogMessage("Config sync event received");
                }
            };

            harmony.PatchAll();
        }

        public static bool hasAwake = false;
        [HarmonyPatch(typeof(Game), "Logout")]
        public static class Logout
        {
            private static void Postfix()
            {
                hasAwake = false;
            }
        }

        [HarmonyPatch(typeof(Player), "OnSpawned")]
        public static class OnSpawned
        {
            private static void Postfix()
            {
                if (hasAwake == true) return;
                hasAwake = true;

                if (IsSinglePlayer.Value) AddClonedItems(root.GetSpawnAreaConfigs());
                else                
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "FileSync", new ZPackage());                
            }
        }

        public static void AddClonedItems(List<Spawner> list)
        {
            var hammer = ObjectDB.instance.m_items.FirstOrDefault(x => x.name == "Hammer");

            if (!hammer)
            {
                Debug.LogError("Custom Spawners - Hammer could not be loaded"); return;
            }

            PieceTable table = hammer.GetComponent<ItemDrop>().m_itemData.m_shared.m_buildPieces;     

            foreach (Spawner areaConfig in list)
            {
                string newName = "CS_" + string.Join("_", areaConfig.name);

                if (table.m_pieces.Exists(x => x.name == newName))
                {
                    continue;
                }
                GameObject customSpawner = PrefabManager.Instance.CreateClonedPrefab(newName, areaConfig.prefabToCopy);
                if (customSpawner == null)
                {
                    Debug.LogError("original prefab not found for " + areaConfig.prefabToCopy);
                    continue;
                }

                customSpawner.GetComponent<ZNetView>().m_syncInitialScale = true;

                SpawnArea area = customSpawner.AddComponent<SpawnArea>();
                Piece piece = customSpawner.GetComponent<Piece>();
                if (piece is null) piece = customSpawner.AddComponent<Piece>();

                piece.m_description = areaConfig.name + " ";
                piece.name = customSpawner.name;

                Object.Destroy(customSpawner.GetComponent<Destructible>());
                Object.Destroy(customSpawner.GetComponent<WearNTear>());

                area.m_spawnTimer = areaConfig.m_spawnTimer;
                area.m_onGroundOnly = areaConfig.m_onGroundOnly;
                area.m_maxTotal = areaConfig.m_maxTotal;
                area.m_maxNear = areaConfig.m_maxNear;
                area.m_farRadius = areaConfig.m_farRadius;
                area.m_spawnRadius = areaConfig.m_spawnRadius;
                area.m_setPatrolSpawnPoint = areaConfig.m_setPatrolSpawnPoint;
                area.m_triggerDistance = areaConfig.m_triggerDistance;
                area.m_spawnIntervalSec = areaConfig.m_spawnIntervalSec;
                area.m_levelupChance = areaConfig.m_levelupChance;
                area.m_nearRadius = areaConfig.m_nearRadius;
                area.m_prefabs = new List<SpawnArea.SpawnData>();

                foreach (string prefab in areaConfig.m_prefabName.Split(','))
                {
                    var newArea = new SpawnArea.SpawnData();
                    newArea.m_weight = 100 / areaConfig.m_prefabName.Split(',').Count();
                    newArea.m_minLevel = areaConfig.minLevel;
                    newArea.m_maxLevel = areaConfig.maxLevel;
                    newArea.m_prefab = PrefabManager.Instance.GetPrefab(prefab);
                    piece.m_description += prefab + " ";
                    if (newArea.m_prefab == null) continue;

                    area.m_prefabs.Add(newArea);
                }

                PieceManager.Instance.RegisterPieceInPieceTable(customSpawner, "Hammer", "Custom Spawners");

                if (!SynchronizationManager.Instance.PlayerIsAdmin)
                {
                    table.m_pieces.Remove(customSpawner);
                }
            }
        }

        [HarmonyPatch(typeof(SpawnArea), "UpdateSpawn")]
        public class UpdateSpawn
        {
            public static bool Prefix(SpawnArea __instance) => __instance.m_nview;
        }
    }
}

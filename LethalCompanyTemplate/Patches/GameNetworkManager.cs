using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

namespace PB
{



    [HarmonyPatch]
    public class NetworkObjectManager
    {

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
        public static void Init()
        {

            Pandora.Instance.logSource.LogInfo("Running Init Function");

            if (networkPrefab != null)
                return;


            networkPrefab = (GameObject)Pandora.Instance.MainAssetBundle.LoadAsset("NetworkHandler");
            networkPrefab.AddComponent<NetworkHandler>();
            

            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);

            if (networkPrefab == null)
                Pandora.Instance.logSource.LogInfo("Network Prefab is also null lmao");

        }

        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Awake))]
        static void SpawnNetworkHandler()
        {
            Pandora.Instance.logSource.LogInfo("Running SpawnNetworkHandler Function");

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {

                Pandora.Instance.logSource.LogInfo("Setting NetworkHandler");

                var networkHandlerHost = UnityEngine.Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();

            }

        }

        

        [HarmonyPostfix, HarmonyPatch(typeof(RoundManager), nameof(RoundManager.GenerateNewFloor))]
        static void SubscribeToHandler()
        {
            NetworkHandler.ReviveEvent += ReceivedEventFromServer;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DespawnPropsAtEndOfRound))]
        static void UnsubscribeFromHandler()
        {
            NetworkHandler.ReviveEvent -= ReceivedEventFromServer;
        }

        static void ReceivedEventFromServer(string eventName)
        {
            // Event Code Here
        }

        static void SendEventToClients(string eventName)
        {
            if (!(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer))
                return;

            NetworkHandler.Instance.EventClientRpc(eventName);
        }


        static GameObject networkPrefab;
    }
}

using BepInEx.AssemblyPublicizer;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PB
{

    public class NetworkHandler : NetworkBehaviour
    {
        public static NetworkHandler Instance { get; private set; }

        public static event Action<String> ReviveEvent;
        
        public override void OnNetworkSpawn()
        {
            ReviveEvent = null;

            Pandora.Instance.logSource.LogInfo("Running OnNetworkSpawn Function");

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                Instance?.gameObject.GetComponent<NetworkObject>().Despawn();
            Instance = this;

            if (Instance == null)
                Pandora.Instance.logSource.LogInfo("Instance is null still lmaoo");

            base.OnNetworkSpawn();
        }


        [ClientRpc]
        public void EventClientRpc(string eventName)
        {
            ReviveEvent?.Invoke(eventName);
        }

        [ServerRpc(RequireOwnership = false)]
        public void EventServerRpc(string eventName)
        {

        }

        
    }

}

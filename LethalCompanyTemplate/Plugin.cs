using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil.Cil;
using PB;
using PB.Patches;
using Unity.Netcode;
using UnityEngine;

namespace PB
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Pandora : BaseUnityPlugin
    {

        private const string modGUID = "YouveMadeAMistake";
        private const string modName = "PandorasBox";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        internal static Pandora Instance;

        internal ManualLogSource logSource;

        internal static PlayerInputs pInputs;
        internal Vector3 deathPos = Vector3.zero;

        private static string aName = Assembly.GetExecutingAssembly().GetName().Name;


        internal AssetBundle MainAssetBundle;



        private static void NetcodePatcher()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            logSource = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            logSource.LogInfo("Shit");
            logSource.LogInfo(aName);

            MainAssetBundle = AssetBundle.LoadFromMemory(NetworkAsset.assets);

            NetcodePatcher(); 

            var gameObject = new UnityEngine.GameObject("pInputs");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            gameObject.AddComponent<PlayerInputs>();
            pInputs = (PlayerInputs)gameObject.GetComponent("pInputs");

            harmony.PatchAll(typeof(PlayerContB_P));
            harmony.PatchAll(typeof(GameNetworkManager));
            harmony.PatchAll(typeof(Pandora));
            


            if (NetworkHandler.Instance == null)
            {

                logSource.LogInfo("It's null lmaoooooo");

            }
            
        }

    }

}












        
          
        

       
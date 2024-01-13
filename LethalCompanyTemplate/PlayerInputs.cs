using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BepInEx;
using GameNetcodeStuff;
using JetBrains.Annotations;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.Windows;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Input = UnityEngine.Input;

namespace PB
{
    internal class PlayerInputs : MonoBehaviour
    {

        private KeyboardShortcut f2Press;
        internal bool wasKeyDown;
        internal static bool triggerRespawn;
       

        private void Awake()
        {
            Pandora.Instance.logSource.LogInfo("I get up");

            f2Press = new KeyboardShortcut(KeyCode.F2);                      
            wasKeyDown = false;

        }



        public void Update()
        {

            if (f2Press.IsDown())
            {
                if (!wasKeyDown)
                {
                    wasKeyDown = true;
                }

            }

            if (f2Press.IsUp())
            {
                if (wasKeyDown)
                {
                    wasKeyDown = false;
                    triggerRespawn = true;
                }

            }       

        }

    }

}

using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using JetBrains.Annotations;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.Windows;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace PB.Patches
{

    public class NetworkHandler : NetworkBehaviour
    {

        public static NetworkHandler Instance { get; private set; }

    }

    




    [HarmonyPatch(typeof(PlayerControllerB))]

    internal class PlayerContB_P
    {

        

        internal static void Revive(Vector3 deathPos)
        {

            if (StartOfRound.Instance.localPlayerController.isPlayerDead)
            {

                StartOfRound.Instance.allPlayersDead = false;

                Pandora.Instance.logSource.LogInfo("Revive attempted");
                StartOfRound.Instance.localPlayerController.ResetPlayerBloodObjects(StartOfRound.Instance.localPlayerController.isPlayerDead);

                StartOfRound.Instance.localPlayerController.isClimbingLadder = false;
                StartOfRound.Instance.localPlayerController.ResetZAndXRotation();
                StartOfRound.Instance.localPlayerController.thisController.enabled = true;
                StartOfRound.Instance.localPlayerController.health = 100;
                StartOfRound.Instance.localPlayerController.disableLookInput = false;

                Pandora.Instance.logSource.LogInfo("Revive: Player death state = " + StartOfRound.Instance.localPlayerController.isPlayerDead.ToString());
                GameNetworkManager.Instance.localPlayerController.isPlayerDead = false;
                StartOfRound.Instance.localPlayerController.isPlayerControlled = true;
                StartOfRound.Instance.localPlayerController.isInElevator = true;
                StartOfRound.Instance.localPlayerController.isInHangarShipRoom = true;
                StartOfRound.Instance.localPlayerController.isInsideFactory = false;
                StartOfRound.Instance.localPlayerController.wasInElevatorLastFrame = false;
                StartOfRound.Instance.SetPlayerObjectExtrapolate(enable: false);
                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(deathPos);
                StartOfRound.Instance.localPlayerController.setPositionOfDeadPlayer = false;
                GameNetworkManager.Instance.localPlayerController.DisablePlayerModel(StartOfRound.Instance.localPlayerController.thisPlayerModel.gameObject, enable: true, disableLocalArms: true);
                StartOfRound.Instance.localPlayerController.helmetLight.enabled = false;
                StartOfRound.Instance.localPlayerController.Crouch(crouch: false);
                StartOfRound.Instance.localPlayerController.criticallyInjured = false;
                if (StartOfRound.Instance.localPlayerController.playerBodyAnimator != null)
                {
                    StartOfRound.Instance.localPlayerController.playerBodyAnimator.SetBool("Limp", value: false);
                }
                StartOfRound.Instance.localPlayerController.bleedingHeavily = false;
                StartOfRound.Instance.localPlayerController.activatingItem = false;
                StartOfRound.Instance.localPlayerController.twoHanded = false;
                StartOfRound.Instance.localPlayerController.inSpecialInteractAnimation = false;
                StartOfRound.Instance.localPlayerController.disableSyncInAnimation = false;
                StartOfRound.Instance.localPlayerController.inAnimationWithEnemy = null;
                StartOfRound.Instance.localPlayerController.holdingWalkieTalkie = false;
                StartOfRound.Instance.localPlayerController.speakingToWalkieTalkie = false;
                StartOfRound.Instance.localPlayerController.isSinking = false;
                StartOfRound.Instance.localPlayerController.isUnderwater = false;
                StartOfRound.Instance.localPlayerController.sinkingValue = 0f;
                StartOfRound.Instance.localPlayerController.statusEffectAudio.Stop();
                StartOfRound.Instance.localPlayerController.DisableJetpackControlsLocally();
                StartOfRound.Instance.localPlayerController.health = 100;
                StartOfRound.Instance.localPlayerController.mapRadarDotAnimator.SetBool("dead", value: false);

                PlayerControllerB playerControllerB = GameNetworkManager.Instance.localPlayerController;
                playerControllerB.bleedingHeavily = false;
                playerControllerB.criticallyInjured = false;
                playerControllerB.playerBodyAnimator.SetBool("Limp", value: false);
                playerControllerB.health = 100;
                HUDManager.Instance.UpdateHealthUI(100, hurtPlayer: false);
                playerControllerB.spectatedPlayerScript = null;
                HUDManager.Instance.audioListenerLowPass.enabled = false;
                StartOfRound.Instance.SetSpectateCameraToGameOverMode(enableGameOver: false, playerControllerB);
                StartOfRound.Instance.livingPlayers = StartOfRound.Instance.connectedPlayersAmount + 1;
                StartOfRound.Instance.allPlayersDead = false;
                
           
                if (StartOfRound.Instance.localPlayerController.IsOwner)
                {
                    HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", value: false);
                    StartOfRound.Instance.localPlayerController.hasBegunSpectating = false;
                    HUDManager.Instance.RemoveSpectateUI();
                    HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
                    HUDManager.Instance.HideHUD(hide: false);
                    StartOfRound.Instance.localPlayerController.hinderedMultiplier = 1f;
                    StartOfRound.Instance.localPlayerController.isMovementHindered = 0;
                    StartOfRound.Instance.localPlayerController.sourcesCausingSinking = 0;
                    StartOfRound.Instance.localPlayerController.reverbPreset = StartOfRound.Instance.shipReverb;
                }

                for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
                {
                    SoundManager.Instance.playerVoicePitchTargets[i] = 1f;
                    SoundManager.Instance.SetPlayerPitch(1f, i);
                }

                SoundManager.Instance.earsRingingTimer = 0f;
                StartOfRound.Instance.localPlayerController.voiceMuffledByEnemy = false;
               
                if (StartOfRound.Instance.localPlayerController.currentVoiceChatIngameSettings == null)
                {
                    StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();
                }
                if (StartOfRound.Instance.localPlayerController.currentVoiceChatIngameSettings != null)
                {
                    if (StartOfRound.Instance.localPlayerController.currentVoiceChatIngameSettings.voiceAudio == null)
                    {
                        StartOfRound.Instance.localPlayerController.currentVoiceChatIngameSettings.InitializeComponents();
                    }
                    if (StartOfRound.Instance.localPlayerController.currentVoiceChatIngameSettings.voiceAudio == null)
                    {
                        return;
                    }
                    StartOfRound.Instance.localPlayerController.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
                }
               
                StartOfRound.Instance.UpdatePlayerVoiceEffects();
                StartOfRound.Instance.AllPlayersHaveRevivedClientRpc();

                Pandora.Instance.logSource.LogInfo("Player got revived");
           
            }
            else
            {
                Pandora.Instance.logSource.LogInfo("Revive Failed: Player death state = " + StartOfRound.Instance.localPlayerController.isPlayerDead.ToString());
            }
                
            
            
            

        }


        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void uPostFix()
        {


            if (StartOfRound.Instance.localPlayerController != null)
            {

                if (StartOfRound.Instance.localPlayerController.health != 0)
                {

                    Pandora.Instance.deathPos = StartOfRound.Instance.localPlayerController.transform.position;
                    
                } else
                {
                    if (PlayerInputs.triggerRespawn)
                    {

                        PlayerInputs.triggerRespawn = false;
                        Revive(Pandora.Instance.deathPos);

                    }

                }

            }

        }

        [HarmonyPatch("KillPlayer")]
        [HarmonyPrefix]
        static void KillPlayer(Vector3 bodyVelocity, bool spawnBody = true, CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0)
        {
            if (StartOfRound.Instance.localPlayerController.IsOwner && !StartOfRound.Instance.localPlayerController.isPlayerDead && StartOfRound.Instance.localPlayerController.AllowPlayerDeath())
            {

                StartOfRound.Instance.localPlayerController.isPlayerDead = true;

                StartOfRound.Instance.localPlayerController.isPlayerControlled = false;
                StartOfRound.Instance.localPlayerController.thisPlayerModelArms.enabled = false;
                StartOfRound.Instance.localPlayerController.localVisor.position = StartOfRound.Instance.localPlayerController.playersManager.notSpawnedPosition.position;
                StartOfRound.Instance.localPlayerController.DisablePlayerModel(StartOfRound.Instance.localPlayerController.thisPlayerModel.gameObject);
                StartOfRound.Instance.localPlayerController.isInsideFactory = false;
                StartOfRound.Instance.localPlayerController.IsInspectingItem = false;
                StartOfRound.Instance.localPlayerController.inTerminalMenu = false;
                StartOfRound.Instance.localPlayerController.twoHanded = false;
                StartOfRound.Instance.localPlayerController.carryWeight = 1f;
                StartOfRound.Instance.localPlayerController.fallValue = 0f;
                StartOfRound.Instance.localPlayerController.fallValueUncapped = 0f;
                StartOfRound.Instance.localPlayerController.takingFallDamage = false;
                StartOfRound.Instance.localPlayerController.isSinking = false;
                StartOfRound.Instance.localPlayerController.isUnderwater = false;
                StartOfRound.Instance.drowningTimer = 1f;
                HUDManager.Instance.setUnderwaterFilter = false;
                StartOfRound.Instance.localPlayerController.wasUnderwaterLastFrame = false;
                StartOfRound.Instance.localPlayerController.sourcesCausingSinking = 0;
                StartOfRound.Instance.localPlayerController.sinkingValue = 0f;
                StartOfRound.Instance.localPlayerController.hinderedMultiplier = 1f;
                StartOfRound.Instance.localPlayerController.isMovementHindered = 0;
                StartOfRound.Instance.localPlayerController.inAnimationWithEnemy = null;
                UnityEngine.Object.FindObjectOfType<Terminal>().terminalInUse = false;
                StartOfRound.Instance.localPlayerController.ChangeAudioListenerToObject(StartOfRound.Instance.localPlayerController.playersManager.spectateCamera.gameObject);
                SoundManager.Instance.SetDiageticMixerSnapshot();
                HUDManager.Instance.SetNearDepthOfFieldEnabled(enabled: true);
                HUDManager.Instance.HUDAnimator.SetBool("biohazardDamage", value: false);
                HUDManager.Instance.gameOverAnimator.SetTrigger("gameOver");
                HUDManager.Instance.HideHUD(hide: true);
                StartOfRound.Instance.localPlayerController.StopHoldInteractionOnTrigger();
                StartOfRound.Instance.localPlayerController.KillPlayerServerRpc((int)StartOfRound.Instance.localPlayerController.playerClientId, spawnBody, bodyVelocity, (int)causeOfDeath, deathAnimation);
                if (spawnBody)
                {
                    StartOfRound.Instance.localPlayerController.SpawnDeadBody((int)StartOfRound.Instance.localPlayerController.playerClientId, bodyVelocity, (int)causeOfDeath, StartOfRound.Instance.localPlayerController, deathAnimation);
                }
                StartOfRound.Instance.SwitchCamera(StartOfRound.Instance.spectateCamera);
                StartOfRound.Instance.localPlayerController.isInGameOverAnimation = 1.5f;
                StartOfRound.Instance.localPlayerController.cursorTip.text = "";
                StartOfRound.Instance.localPlayerController.cursorIcon.enabled = false;
                StartOfRound.Instance.localPlayerController.DropAllHeldItems(spawnBody);
                StartOfRound.Instance.localPlayerController.DisableJetpackControlsLocally();

                Pandora.Instance.logSource.LogInfo("Player got oofed");

                return;
            }
        }

    }

}

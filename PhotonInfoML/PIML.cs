using MelonLoader;
using PhotonInfoML;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using PhotonInfoML.Utils;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using HarmonyLib;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(PIML), "PhotonInfo", "0.2.0", "Luna")]
[assembly: MelonGame("Another Axiom", "Gorilla Tag")]
[assembly: MelonColor(255, 0, 255, 0)]

namespace PhotonInfoML
{
    public class PIML : MelonMod
    {
        public static long rpcIn;
        public static long eventIn;
        public static long rpcOut;
        public static long reportsSent;
        public static long outboundByteCount;
        public static List<byte> eventCodesToIgnore = new List<byte> { 0, 1, 2, 3, 4, 5, 8, 9, 50, 51, 176, 199 };

        public override void OnLateInitializeMelon()
        {
            LoggerInstance.Msg("PhotonInfo is a diagnostic mod intended for monitoring networking events. It is only for developers. You will need to have some basic Photon networking knowledge to make use of its output.");
            UnityEngine.Object.FindObjectOfType<CinemachineBrain>().gameObject.SetActive(false);
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code < 200 && !eventCodesToIgnore.Contains(photonEvent.Code)) // These event codes are either used by Photon or Gorilla Tag
            {
                eventIn++;
                LoggerInstance.Msg($"EVENT - Event no. {photonEvent.Code} sent by {PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender).NickName}");
            }
        }

        public override void OnFixedUpdate()
        {
            PhotonNetworkController.Instance.disableAFKKick = true;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Ray ray = GorillaTagger.Instance.mainCamera.GetComponent<Camera>().ScreenPointToRay(UnityEngine.InputSystem.Pointer.current.position.value);
                if (Physics.Raycast(ray, out RaycastHit hit, 5000f))
                {
                    GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>().enabled = false;
                    GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                    GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>().enabled = true;

                }
            }
        }
    }

    [HarmonyPatch(typeof(CreditsView), "GetPage")]
    static class ComputerPatch
    {
        static bool Prefix(CreditsView __instance, ref string __result)
        {
            __result = $"RPCs EXECUTED: {PIML.rpcIn}\n" +
                       $"EVENTS RECIEVED: {PIML.eventIn}\n" +
                       $"RPCs SENT: {PIML.rpcOut}\n" +
                       $"REPORTS SENT: {PIML.reportsSent}\n" +
                       $"DATA SENT: {PIML.outboundByteCount.ToSize(DataUtil.SizeUnits.KB)}KB | {PIML.outboundByteCount.ToSize(DataUtil.SizeUnits.MB)}MB";
            return false;
        }
    }
}
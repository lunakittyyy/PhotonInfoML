using System;
using MelonLoader;
using HarmonyLib;
using PhotonInfoML;
using Photon.Pun;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using Photon.Realtime;
using GorillaNetworking;
[assembly: MelonInfo(typeof(PIML), "PhotonInfo", "0.1.0", "Luna")]
[assembly: MelonGame("Another Axiom", "Gorilla Tag")]
[assembly: MelonColor(255, 0, 255, 0)]
namespace PhotonInfoML
{
    public class PIML : MelonMod
    {
        public static long rpcIn;
        public static long rpcOut;
        public static long reportsSent;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("PhotonInfo is a diagnostic mod intended for monitoring networking events. It is only for developers. You will need to have some basic Photon networking knowledge to make use of its output.");
            //PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
            //PhotonNetwork.LogLevel = PunLogLevel.Full;
        }

        public override void OnFixedUpdate()
        {
            GorillaLevelScreen[] levelScreens = GorillaComputer.instance.levelScreens;
            for (int i = 0; i < levelScreens.Length; i++)
            {
                levelScreens[i].UpdateText($"RPCS EXECUTED: {rpcIn}\n" +
                                           $"RPCS SENT: {rpcOut}\n" +
                                           $"REPORTS SENT: {reportsSent}", setToGoodMaterial: true);
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            LoggerInstance.Msg($"EVENT - Event no. {photonEvent.Code} sent by {photonEvent.Sender} with content {photonEvent.Parameters}");
        }
    }
    
    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCallLocal")]
    [HarmonyWrapSafe]
    static class IncrementRPCPatch
    {
        static void Prefix(ref PhotonMessageInfo info, ref string rpcFunction, GorillaNot __instance)
        {
            PIML.rpcIn++;
            if (rpcFunction != "PlayHandTap") 
                Melon<PIML>.Logger.Msg($"RPC - {rpcFunction} sent by {info.Sender.NickName}");
        }
    }
    [HarmonyPatch(typeof(PhotonView), "RPC", new Type[] { typeof(string), typeof(RpcTarget), typeof(object[]) })]
    [HarmonyWrapSafe]
    static class OutboundRPCPatch
    {
        static void Prefix(ref string methodName, ref RpcTarget target)
        {
            PIML.rpcOut++;
            if (methodName != "PlayHandTap")
                Melon<PIML>.Logger.Warning($"OUTBOUND RPC - {methodName} to {target}");
        }
    }
    [HarmonyPatch(typeof(PhotonView), "RPC", new Type[] { typeof(string), typeof(Player), typeof(object[]) })]
    [HarmonyWrapSafe]
    static class OutboundRPCPlayerPatch
    {
        static void Prefix(ref string methodName, ref Player targetPlayer)
        {
            PIML.rpcOut++;
            if (methodName != "PlayHandTap")
                Melon<PIML>.Logger.Warning($"OUTBOUND RPC - {methodName} to {targetPlayer.NickName}");
        }
    }
    [HarmonyPatch(typeof(GorillaNot), "SendReport")]
    [HarmonyWrapSafe]
    static class ReportPatch
    {
        static void Prefix(ref string susReason, ref string susNick)
        {
            PIML.reportsSent++;
            Melon<PIML>.Logger.Error($"REPORT - {susNick} reported for {susReason}");
        }
    }
    /*
    [HarmonyPatch(typeof(PhotonNetwork), "RaiseEvent")]
    [HarmonyWrapSafe]
    static class OutboundEventPatch
    {
        static void Prefix(ref byte eventCode, ref object eventContent)
        {
            Melon<PIML>.Logger.Warning($"OUTBOUND EVENT - Event no. {eventCode} with content {eventContent}");
        }
    }
    */
}

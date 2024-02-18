using HarmonyLib;
using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using System;

namespace PhotonInfoML.Patches
{
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

    [HarmonyPatch(typeof(LoadBalancingPeer), "OpRaiseEvent")]
    [HarmonyWrapSafe]
    static class EventPatch
    {
        static void Postfix(LoadBalancingPeer __instance)
        {
            PIML.outboundByteCount += __instance.ByteCountLastOperation;
        }
    }
}

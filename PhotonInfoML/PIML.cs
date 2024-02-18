using MelonLoader;
using PhotonInfoML;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using PhotonInfoML.Utils;

[assembly: MelonInfo(typeof(PIML), "PhotonInfo", "0.2.0", "Luna")]
[assembly: MelonGame("Another Axiom", "Gorilla Tag")]
[assembly: MelonColor(255, 0, 255, 0)]

namespace PhotonInfoML
{
    public class PIML : MelonMod
    {
        public static long rpcIn;
        public static long rpcOut;
        public static long reportsSent;
        public static long outboundByteCount;

        public override void OnLateInitializeMelon()
        {
            LoggerInstance.Msg("PhotonInfo is a diagnostic mod intended for monitoring networking events. It is only for developers. You will need to have some basic Photon networking knowledge to make use of its output.");
            UnityEngine.Object.FindObjectOfType<CinemachineBrain>().gameObject.SetActive(false);
        }  

        public override void OnFixedUpdate()
        {
            PhotonNetworkController.Instance.disableAFKKick = true;
            GorillaLevelScreen[] levelScreens = GorillaComputer.instance.levelScreens;
            for (int i = 0; i < levelScreens.Length; i++)
            {
                levelScreens[i].UpdateText($"RPCs EXECUTED: {rpcIn}\n" +
                                           $"RPCs SENT: {rpcOut}\n" +
                                           $"REPORTS SENT: {reportsSent}\n" +
                                           $"DATA SENT: {outboundByteCount.ToSize(DataUtil.SizeUnits.KB)}KB | {outboundByteCount.ToSize(DataUtil.SizeUnits.MB)}MB", setToGoodMaterial: true);
            }

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
}

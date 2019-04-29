using Harmony;
using System;
using System.Linq;
using UnityEngine;

#pragma warning disable RCS1018, RCS1213, IDE0051
namespace TwitchIntegration.Harmony
{
    [HarmonyPatch(typeof(CarScreenLogic), "Awake")]
    public class CarScreenLogic_Patch
    {
        static void Prefix(CarScreenLogic __instance)
        {
            __instance.gameObject.AddComponent<CarScreenLogic_Update>();
        }

        protected class CarScreenLogic_Update : MonoBehaviour
        {
            private CarScreenLogic logic;

            void Awake()
            {
                logic = GetComponent<CarScreenLogic>();
            }

            void Update()
            {
                foreach (CarScreenWidgetBase widget in logic.carScreenWidgets_)
                {
                    if (WidgetsFilter.Contains(widget.name))
                    {
                        widget.gameObject.SetActive(false);
                    }
                }
                foreach (GameObject screensaver in logic.screensavers_)
                {
                    screensaver.SetActive(false);
                }

                Console.Title = G.Sys.PlayerManager_.Current_.playerData_.CarCamera_.activeCameraMode_.GetType().Name;

                logic.errorText_.gameObject.SetActive(true);
                logic.errorText_.enabled = true;

                logic.errorText_.textMesh_.text = Chat.WordWrap(Chat.Messages, 22);
                //TODO: make font smaller
                logic.errorText_.textMesh_.fontSize = 24;
            }

            public readonly string[] WidgetsFilter = new string[] {
                "Stunt Points",
                "Stunt Combo",
                "Stunt Combo Big",
                "Compass",
                "Arrow",
                //"TagBubble",
                //"Countdown",
                //"FinalCountdown",
                "Placement",
                "Speed",
                "MPH",
                "CheatIcon",
                "BackgroundPlane",
                "JumpCircle",
                "MovementLines",
                "Speedometer",
                "TopHeatLines",
                "WheelIndicatorBL",
                "WheelIndicatorBR",
                "WheelIndicatorFL",
                "WheelIndicatorFR",
                //"FrontLight",
                "WingsIndicator",
                //"BackLights",
                //"Time",
                "StuntCollectibleMultiplier"
            };
        }
    }
}

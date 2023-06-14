#define UNITY_5_3_OR_NEWER

using GenshinImpactVR;
using UnityEngine;
using MelonLoader;
using Valve.VR;
using UnityEngineInternal;
using System;
using UniverseLib.Input;
using Il2CppInterop.Runtime.Injection;

[assembly: MelonInfo(typeof(Program), "GenshinImpactVR", "0.0.1", "svr2kos2")]
//[assembly: MelonGame("miHoYo", "Genshin Impact")]

namespace GenshinImpactVR {

    public class CamereUpdater : MonoBehaviour {
        public CamereUpdater(IntPtr ptr) : base(ptr) {
        }
        public CamereUpdater() : base(ClassInjector.DerivedConstructorPointer<CamereUpdater>()) {
            ClassInjector.DerivedConstructorBody(this);
        }
        public void Update() {
            GetComponent<Camera>().fieldOfView = 90f;
        }
    }


    public class Program : MelonMod {
        Camera cam;

        void Init() {
            
        }

        void Deinit() {
            
        }

        public override void OnInitializeMelon() {
            ClassInjector.RegisterTypeInIl2Cpp<CamereUpdater>();
            MelonLogger.Msg("registed camera updater.");
        }


        public override void OnUpdate() {
            if (Input.GetKeyDown(KeyCode.Home)) {
                cam = Utility.RenderTest();
                cam.gameObject.AddComponent<CamereUpdater>();
            }

            if (cam == null)
                return;

            cam.projectionMatrix = new Matrix4x4(
                new Vector4(1.33319f, 0, 0, 0),
                new Vector4(0, 0.8391f, 0, 0),
                new Vector4(0, 0, -1.0006f, -1f),
                new Vector4(0, 0, -0.60018f, 0)
            );


            if (Input.GetKey(KeyCode.UpArrow)) {

            }

        }

    }

    

}
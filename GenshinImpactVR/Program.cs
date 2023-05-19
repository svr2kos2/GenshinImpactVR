#define UNITY_5_3_OR_NEWER

using GenshinImpactVR;
using UnityEngine;
using MelonLoader;
using Valve.VR;
using UnhollowerRuntimeLib;
using UnityEngineInternal;
using System;
using UniverseLib.Input;

[assembly: MelonInfo(typeof(Program), "GenshinImpactVR", "0.0.1", "svr2kos2")]
//[assembly: MelonGame("miHoYo", "Genshin Impact")]

namespace GenshinImpactVR {


    public class Program : MelonMod {
        CVRSystem? hmd;
        EVRInitError hmd_error = EVRInitError.Unknown;

        GameObject? player;

        RenderTexture? lrt;
        RenderTexture? rrt;

        Camera? leftCam;
        Camera? rightCam;

        void Init() {
            hmd = OpenVR.Init(ref hmd_error);
            if (hmd_error != EVRInitError.None) {
                MelonLogger.Error(hmd_error);
                return;
            }

            MelonLogger.Msg("Camera " + Camera.main.name);

            player = new GameObject();
            player.name = "VRPlayer";
            player.transform.parent = Camera.main.transform;
            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.identity;

            MelonLogger.Msg("Player " + player.name);

            uint h = 0, w = 0;
            hmd.GetRecommendedRenderTargetSize(ref w, ref h);

            lrt = new RenderTexture((int)w, (int)h, 0);
            rrt = new RenderTexture((int)w, (int)h, 0);

            var leftEye = new GameObject();
            var rightEye = new GameObject();

            leftEye.name = "LeftEye";
            leftCam = leftEye.AddComponent<Camera>();
            leftEye.transform.parent = player.transform;
            leftEye.transform.localPosition = Vector3.zero;
            leftEye.transform.rotation = Quaternion.identity;

            MelonLogger.Msg("LeftEye " + leftEye.name + " " + leftCam);

            rightEye.name = "RightEye";
            rightCam = rightEye.AddComponent<Camera>();
            rightEye.transform.parent = player.transform;
            rightEye.transform.localPosition = Vector3.zero;
            rightEye.transform.rotation = Quaternion.identity;

            MelonLogger.Msg("RightEye " + rightEye.name + " " + rightCam);
            
            leftCam.targetTexture = lrt;
            rightCam.targetTexture = rrt;

            var lPost = leftCam.gameObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
            lPost._hdr2sdrMat = Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().hdr2sdrMat;
            lPost.finalTarget = lrt;
            lPost.m_Resources = Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().m_Resources;
            lPost.isExtraCamera = true;
            lPost.m_ScreenHeight = (int)h;
            lPost.m_ScreenWidth = (int)w;

            var rPost = rightCam.gameObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
            rPost._hdr2sdrMat = Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().hdr2sdrMat;
            rPost.finalTarget = rrt;
            rPost.m_Resources = Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().m_Resources;
            rPost.isExtraCamera = true;
            rPost.m_ScreenHeight = (int)h;
            rPost.m_ScreenWidth = (int)w;
            

        }

        void Deinit() {
            OpenVRInterop.ShutdownInternal();
            
        }

        public override void OnLateUpdate() {
            if (hmd_error != EVRInitError.None) {
                return;
            }
            leftCam.projectionMatrix = GetHMDMatrixProjectionEye(EVREye.Eye_Left);
            rightCam.projectionMatrix = GetHMDMatrixProjectionEye(EVREye.Eye_Right);
            base.OnLateUpdate();
        }

        public override void OnUpdate() {
            if (InputManager.GetKeyDown(KeyCode.Home)) {
                MelonLogger.Msg("InitVR");
                Init();
            }
            if (InputManager.GetKeyDown(KeyCode.End)) {
                Deinit();
            }
            if (hmd_error != EVRInitError.None) {
                return;
            }
            var compositor = OpenVR.Compositor;
            if (compositor != null) {

                if (compositor.CanRenderScene()) {

                    var temp = RenderTexture.GetTemporary(lrt.descriptor);
                    Graphics.Blit(lrt, temp, new Vector2(1, -1), new Vector2(0, 1));
                    Graphics.Blit(temp, lrt);
                    Graphics.Blit(rrt, temp, new Vector2(1, -1), new Vector2(0, 1));
                    Graphics.Blit(temp, rrt);
                    RenderTexture.ReleaseTemporary(temp);

                    VRTextureBounds_t bounds = new VRTextureBounds_t();
                    bounds.uMin = 0f;
                    bounds.uMax = 1f;
                    bounds.vMin = 0f;
                    bounds.vMax = 1f;
                    Texture_t ltex = new Texture_t();
                    ltex.eColorSpace = EColorSpace.Gamma;
                    ltex.eType = ETextureType.DirectX;
                    ltex.handle = lrt.GetNativeDepthBufferPtr();
                    Texture_t rtex = new Texture_t();
                    rtex.eColorSpace = EColorSpace.Gamma;
                    rtex.eType = ETextureType.DirectX;
                    rtex.handle = rrt.GetNativeDepthBufferPtr();

                    //leftCam.Render();
                    var err = compositor.Submit(EVREye.Eye_Left, ref ltex, ref bounds, EVRSubmitFlags.Submit_Default);

                    //rightCam.Render();
                    err = compositor.Submit(EVREye.Eye_Right, ref rtex, ref bounds, EVRSubmitFlags.Submit_Default);

                }

                var renderPos = new TrackedDevicePose_t[64];
                var gamePos = new TrackedDevicePose_t[64];
                compositor.WaitGetPoses(renderPos, gamePos);

                player.transform.localPosition = renderPos[0].mDeviceToAbsoluteTracking.GetPosition();
                player.transform.localRotation = renderPos[0].mDeviceToAbsoluteTracking.GetRotation();

                var EyePos = hmd.GetEyeToHeadTransform(EVREye.Eye_Left);
                leftCam.transform.localPosition = EyePos.GetPosition();
                leftCam.transform.localRotation = EyePos.GetRotation();
                leftCam.projectionMatrix = GetHMDMatrixProjectionEye(EVREye.Eye_Left);

                EyePos = hmd.GetEyeToHeadTransform(EVREye.Eye_Right);
                rightCam.transform.localPosition = EyePos.GetPosition();
                rightCam.transform.localRotation = EyePos.GetRotation();
                rightCam.projectionMatrix = GetHMDMatrixProjectionEye(EVREye.Eye_Right);

            }
        }
        Matrix4x4 GetHMDMatrixProjectionEye(EVREye nEye) {
            if (hmd == null)
                return Matrix4x4.zero;

            var mat = hmd.GetProjectionMatrix(nEye, 0.3f, 1000f);

            return new Matrix4x4(
                new Vector4(mat.m0, mat.m4, mat.m8, mat.m12),
                new Vector4(mat.m1, mat.m5, mat.m9, mat.m13),
                new Vector4(mat.m2, mat.m6, mat.m10, mat.m14),
                new Vector4(mat.m3, mat.m7, mat.m11, mat.m15)
            );
        }

    }
}
using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GenshinImpactVR {
    public class Utility {
        public static Camera RenderTest() {
            string[] destroyList = { "VRPlayer", "RawImage", "RawImageBackground" };
            foreach (string destroy in destroyList) {
                var obj = GameObject.Find(destroy);
                if (obj != null) {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }
            Camera.main.GetComponent<Cinemachine.CinemachineExternalCamera>().enabled = false;
            var canvas = GameObject.Find("Pages").transform.parent.gameObject;
            var player = UnityEngine.Object.Instantiate(Camera.main.gameObject);
            //player.transform.SetParent(Camera.main.transform);
            player.name = "VRPlayer";
            player.transform.position = Camera.main.transform.position;
            player.transform.rotation = Camera.main.transform.rotation;
            var cam = player.GetComponent<Camera>();

            var rawImage = new GameObject("RawImage");
            rawImage.transform.SetParent(canvas.transform);
            rawImage.AddComponent<UnityEngine.UI.RawImage>();
            var rawImageRectTransform = rawImage.GetComponent<RectTransform>();
            rawImageRectTransform.localScale = Vector3.one;
            rawImageRectTransform.pivot = Vector2.zero;
            rawImageRectTransform.anchorMin = Vector2.zero;
            rawImageRectTransform.anchorMax = Vector2.zero;
            rawImageRectTransform.sizeDelta = new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2);

            var rt = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24, RenderTextureFormat.RGB111110Float);
            var camPost = cam.gameObject.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
            camPost.finalTarget = rt;
            camPost.isExtraCamera = true;
            rawImage.GetComponent<UnityEngine.UI.RawImage>().texture = rt;
            GameObject.Destroy(cam.GetComponent<Cinemachine.CinemachineExternalCamera>());
            GameObject.Destroy(cam.GetComponent<Cinemachine.CinemachineBrain>());
            cam.nearClipPlane = 0.000001f;
            return cam;
        }
    }
}

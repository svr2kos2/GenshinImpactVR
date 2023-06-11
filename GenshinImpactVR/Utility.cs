using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GenshinImpactVR {
    internal class Utility {
        public void RenderTest() {
            string[] destroyList = { "VRPlayer", "RawImage", "RawImageBackground" };
            foreach (string destroy in destroyList) {
                var obj = GameObject.Find(destroy);
                if (obj != null) {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }
            var canvas = GameObject.Find("Pages").transform.parent.gameObject;
            //var background = new GameObject("RawImageBackground");
            //background.transform.SetParent(canvas.transform);
            //var backGroundImage = background.AddComponent<UnityEngine.UI.RawImage>();
            //backGroundImage.color = new Color(1, 1, 1, 1);
            //var backgroundRT = background.GetComponent<RectTransform>();
            //backgroundRT.localScale = Vector3.one;
            //backgroundRT.pivot = Vector2.zero;
            //backgroundRT.anchorMin = Vector2.zero;
            //backgroundRT.anchorMax = Vector2.zero;
            //backgroundRT.sizeDelta = new Vector2(1920/2, 1200/2);

            var rawImage = new GameObject("RawImage");
            rawImage.transform.SetParent(canvas.transform);
            rawImage.AddComponent<UnityEngine.UI.RawImage>();
            var rawImageRectTransform = rawImage.GetComponent<RectTransform>();
            rawImageRectTransform.localScale = Vector3.one;
            rawImageRectTransform.pivot = Vector2.zero;
            rawImageRectTransform.anchorMin = Vector2.zero;
            rawImageRectTransform.anchorMax = Vector2.zero;
            rawImageRectTransform.sizeDelta = new Vector2(1920 / 2, 1200 / 2);

            var player = UnityEngine.Object.Instantiate(Camera.main.gameObject);
            player.transform.SetParent(Camera.main.transform);
            player.name = "VRPlayer";
            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.identity;

            //var player = new GameObject("VRPlayer");
            //player.transform.parent = Camera.main.transform;
            //player.transform.localPosition = Vector3.zero;
            //player.transform.localRotation = Quaternion.identity;
            var cam = player.GetComponent<Camera>();
            var rt = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24, RenderTextureFormat.RGB111110Float);


            var camPost = cam.gameObject.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
            camPost.finalTarget = rt;
            camPost.isExtraCamera = true;
            //camPost.m_ScreenHeight = cam.pixelWidth;
            //camPost.m_ScreenWidth = cam.pixelHeight;

            rawImage.GetComponent<UnityEngine.UI.RawImage>().texture = rt;
        }
    }
}

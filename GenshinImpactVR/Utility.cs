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
            
            var player = UnityEngine.Object.Instantiate(Camera.main.gameObject);
            //player.transform.SetParent(Camera.main.transform);
            player.name = "VRPlayer";
            player.transform.position = Camera.main.transform.position;
            player.transform.rotation = Camera.main.transform.rotation;
            var cam = player.GetComponent<Camera>();

            

            var rt = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24, RenderTextureFormat.RGB111110Float);
            var camPost = cam.gameObject.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
            camPost.finalTarget = rt;
            camPost.isExtraCamera = true;
            CreateRawImage().GetComponent<UnityEngine.UI.RawImage>().texture = rt;
            GameObject.Destroy(cam.GetComponent<Cinemachine.CinemachineExternalCamera>());
            GameObject.Destroy(cam.GetComponent<Cinemachine.CinemachineBrain>());
            cam.fieldOfView = 90f;
            return cam;
        }

        public static UnityEngine.UI.RawImage CreateRawImage() {
            var canvas = GameObject.Find("Pages").transform.parent.gameObject;
            var rawImage = new GameObject("RawImage");
            rawImage.transform.SetParent(canvas.transform);
            var res = rawImage.AddComponent<UnityEngine.UI.RawImage>();
            var rawImageRectTransform = rawImage.GetComponent<RectTransform>();
            rawImageRectTransform.localScale = Vector3.one;
            rawImageRectTransform.pivot = Vector2.zero;
            rawImageRectTransform.anchorMin = Vector2.zero;
            rawImageRectTransform.anchorMax = Vector2.zero;
            rawImageRectTransform.sizeDelta = new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
            return res;
        }

    }
}

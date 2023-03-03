/* 
*   BlazePalm
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Visualizers {

    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;
    using NatML.VideoKit.UI;
    using NatML.Vision;

    /// <summary>
    /// </summary>
    [RequireComponent(typeof(VideoKitCameraView))]
    public sealed class BlazePalmVisualizer : MonoBehaviour {

        #region --Inspector--
        public Image keypoint;
        public UILineRenderer bones;
        #endregion


        #region --Client API--
        /// <summary>
        /// </summary>
        /// <param name="hands"></param>
        public void Render (params BlazePalmPredictor.Hand[] hands) {
            // Clear current
            foreach (var t in currentHands)
                GameObject.Destroy(t.gameObject);
            currentHands.Clear();
            // Render
            var segments = new List<Vector2[]>();
            foreach (var hand in hands) {
                // Keypoints
                foreach (var p in hand.keypoints)
                    AddKeypoint((Vector2)p);
                // Bones
                segments.AddRange(new List<Vector3[]> {
                    new [] { hand.keypoints.wrist, hand.keypoints.thumbCMC, hand.keypoints.thumbMCP, hand.keypoints.thumbIP, hand.keypoints.thumbTip },
                    new [] { hand.keypoints.wrist, hand.keypoints.indexMCP, hand.keypoints.indexPIP, hand.keypoints.indexDIP, hand.keypoints.indexTip },
                    new [] { hand.keypoints.middleMCP, hand.keypoints.middlePIP, hand.keypoints.middleDIP, hand.keypoints.middleTip },
                    new [] { hand.keypoints.ringMCP, hand.keypoints.ringPIP, hand.keypoints.ringDIP, hand.keypoints.ringTip },
                    new [] { hand.keypoints.wrist, hand.keypoints.pinkyMCP, hand.keypoints.pinkyPIP, hand.keypoints.pinkyDIP, hand.keypoints.pinkyTip },
                    new [] { hand.keypoints.indexMCP, hand.keypoints.middleMCP, hand.keypoints.ringMCP, hand.keypoints.pinkyMCP },
                }.Select(points => points.Select(p => (Vector2)p).ToArray()));
            }
            bones.Points = null;
            bones.Segments = segments;
        }
        #endregion


        #region --Operations--
        private RawImage rawImage;
        private AspectRatioFitter aspectFitter;
        private readonly List<GameObject> currentHands = new List<GameObject>();

        void Awake () {
            rawImage = GetComponent<RawImage>();
            aspectFitter = GetComponent<AspectRatioFitter>();
        }

        private void AddKeypoint (Vector2 point) {
            // Instantiate
            var prefab = Instantiate(keypoint, transform);
            prefab.gameObject.SetActive(true);
            // Position
            var prefabTransform = prefab.transform as RectTransform;
            var imageTransform = rawImage.transform as RectTransform;
            prefabTransform.anchorMin = 0.5f * Vector2.one;
            prefabTransform.anchorMax = 0.5f * Vector2.one;
            prefabTransform.pivot = 0.5f * Vector2.one;
            prefabTransform.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, point);
            // Add
            currentHands.Add(prefab.gameObject);
        }
        #endregion
    }
}
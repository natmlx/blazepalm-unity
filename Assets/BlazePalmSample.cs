/* 
*   BlazePalm
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using NatML.Vision;
    using NatML.Visualizers;
    using VideoKit;

    public sealed class BlazePalmSample : MonoBehaviour {

        [Header(@"Camera")]
        public VideoKitCameraManager cameraManager;

        [Header(@"UI")]
        public BlazePalmVisualizer visualizer;

        private BlazePalmPipeline pipeline;

        private async void Start () {
            // Create the BlazePalm predictor
            pipeline = await BlazePalmPipeline.Create();
            // Listen for camera frames
            cameraManager.OnCameraFrame.AddListener(OnCameraFrame);
        }

        private void OnCameraFrame (CameraFrame frame) {
            // Predict
            var hands = pipeline.Predict(frame);
            // Visualize
            visualizer.Render(hands);
        }

        private void OnDisable () {
            // Stop listening for camera frames
            cameraManager.OnCameraFrame.RemoveListener(OnCameraFrame);
            // Dispose the BlazePalm pipeline
            pipeline?.Dispose();
        }
    }
}
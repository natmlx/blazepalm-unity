/* 
*   BlazePalm
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using NatML.Devices;
    using NatML.Devices.Outputs;
    using NatML.Vision;
    using NatML.Visualizers;

    [MLModelDataEmbed("@natml/blazepalm-detector"), MLModelDataEmbed("@natml/blazepalm-landmark")]
    public sealed class BlazePalmSample : MonoBehaviour {
        
        [Header(@"UI")]
        public BlazePalmVisualizer visualizer;

        private CameraDevice cameraDevice;
        private TextureOutput previewTextureOutput;

        private BlazePalmPipeline pipeline;

        async void Start () {
            // Request camera permissions
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();
            if (permissionStatus != PermissionStatus.Authorized) {
                Debug.Log(@"User did not grant camera permissions");
                return;
            }
            // Get a camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            previewTextureOutput = new TextureOutput();
            cameraDevice.StartRunning(previewTextureOutput);
            // Display the preview
            var previewTexture = await previewTextureOutput;
            visualizer.image = previewTexture;
            // Create the BlazePalm predictor
            var detectorModelData = await MLModelData.FromHub("@natml/blazepalm-detector");
            var predictorModelData = await MLModelData.FromHub("@natml/blazepalm-landmark");
            pipeline = new BlazePalmPipeline(detectorModelData, predictorModelData);
        }

        void Update () {
            // Check that the predictor has been created
            if (pipeline == null)
                return;
            // Predict
            var hands = pipeline.Predict(previewTextureOutput.texture);
            visualizer.Render(hands);
        }

        void OnDisable () => pipeline?.Dispose();
    }
}
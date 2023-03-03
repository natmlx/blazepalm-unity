/* 
*   BlazePalm
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// </summary>
    public sealed class BlazePalmPipeline : IMLPredictor<BlazePalmPredictor.Hand[]> {

        #region --Client API--
        /// <summary>
        /// Detect hands in an image.
        /// </summary>
        /// <param name="inputs">Input image. This MUST be an `MLImageFeature`.</param>
        /// <returns>Detected hands.</returns>
        public BlazePalmPredictor.Hand[] Predict (params MLFeature[] inputs) {
            // Check type
            if (!(inputs[0] is MLImageFeature imageFeature))
                throw new ArgumentException(@"BlazePalm pipeline expects an image feature", nameof(inputs));
            // Detect ROIs
            var detections = detector.Predict(imageFeature);
            // Predict landmarks
            var capacity = Mathf.Min(detections.Length, maxDetections);
            var result = new List<BlazePalmPredictor.Hand>(capacity);
            for (var i = 0; i < capacity; ++i) {
                // Extract ROI
                var detection = detections[i];
                var roi = detection.regionOfInterest;
                var roiFeature = new MLImageFeature(roiBuffer, roi.width, roi.height);
                imageFeature.CopyTo(roiFeature, roi, -detection.rotation, Color.black);
                // Predict landmarks
                var landmarks = predictor.Predict(roiFeature);
                // Create pose
                var keypoints = new BlazePalmPredictor.Keypoints(landmarks.keypoints.data, landmarks.keypoints.imageType, detection.regionOfInterestToImageMatrix);
                var hand = new BlazePalmPredictor.Hand(landmarks.score, landmarks.handedness, keypoints, landmarks.keypoints3D);
                result.Add(hand);
            }
            // Return
            return result.ToArray();
        }

        /// <summary>
        /// Dispose the pipeline and release resources.
        /// </summary>
        public void Dispose () {
            detector.Dispose();
            predictor.Dispose();
        }

        /// <summary>
        /// Create the BlazePalm pipeline.
        /// </summary>
        /// <param name="minScore">Minimum hand candidate score.</param>
        /// <param name="maxDetections">Maximum number of detections to return.</param>
        /// <param name="configuration">Edge model configuration.</param>
        /// <param name="accessKey">NatML access key.</param>
        public static async Task<BlazePalmPipeline> Create (
            float minScore = 0.5f,
            int maxDetections = Int32.MaxValue,
            MLEdgeModel.Configuration configuration = null,
            string accessKey = null
        ) {
            // Create detector and predictor
            const float MaxIoU = 0.5f;
            var detectorTask = BlazePalmDetector.Create(minScore, MaxIoU, configuration, accessKey);
            var predictorTask = BlazePalmPredictor.Create(configuration, accessKey);
            // Wait until created
            await Task.WhenAll(detectorTask, predictorTask);
            // Create pipeline
            var pipeline = new BlazePalmPipeline(detectorTask.Result, predictorTask.Result, maxDetections);
            return pipeline;
        }
        #endregion


        #region --Operations--
        private readonly BlazePalmDetector detector;
        private readonly BlazePalmPredictor predictor;
        private readonly int maxDetections;
        private readonly byte[] roiBuffer; // this is a memory optimization to prevent allocations when extracting ROIs

        private BlazePalmPipeline (BlazePalmDetector detector, BlazePalmPredictor predictor, int maxDetections = Int32.MaxValue) {
            this.detector = detector;
            this.predictor = predictor;
            this.maxDetections = maxDetections;
            this.roiBuffer = new byte[640 * 480 * 4]; // just has to be big enough for the largest possible ROI
        }
        #endregion
    }
}
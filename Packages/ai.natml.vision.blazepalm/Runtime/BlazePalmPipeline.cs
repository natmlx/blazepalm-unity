/* 
*   BlazePalm
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// </summary>
    public sealed class BlazePalmPipeline : IMLPredictor<BlazePalmPredictor.Hand[]> {

        #region --Client API--
        /// <summary>
        /// Create the BlazePalm pipeline.
        /// </summary>
        /// <param name="detector">BlazePalm detector model data.</param>
        /// <param name="predictor">BlazePalm landmark predictor model data.</param>
        /// <param name="maxDetections">Maximum number of detections to return.</param>
        public BlazePalmPipeline (MLModelData detector, MLModelData predictor, int maxDetections = Int32.MaxValue) {
            this.detectorData = detector;
            this.predictorData = predictor;
            this.detectorModel = new MLEdgeModel(detector);
            this.predictorModel = new MLEdgeModel(predictor);
            this.detector = new BlazePalmDetector(detectorModel);
            this.predictor = new BlazePalmPredictor(predictorModel);
            this.maxDetections = maxDetections;
        }

        /// <summary>
        /// Detect hands in an image.
        /// </summary>
        /// <param name="inputs">Input image. This MUST be an `MLImageFeature`.</param>
        /// <returns>Detected hands.</returns>
        public BlazePalmPredictor.Hand[] Predict (params MLFeature[] inputs) {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"BlazePalm pipeline expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0] as MLImageFeature;
            if (input == null)
                throw new ArgumentException(@"BlazePalm pipeline expects an image feature", nameof(inputs));
            // Detect poses
            (input.mean, input.std) = detectorData.normalization;
            input.aspectMode = detectorData.aspectMode;
            var detections = detector.Predict(input);
            // Predict landmarks
            var capacity = Mathf.Min(detections.Length, maxDetections);
            var result = new List<BlazePalmPredictor.Hand>(capacity);
            for (var i = 0; i < capacity; ++i) {
                // Extract ROI
                var detection = detections[i];
                var roi = input.RegionOfInterest(detection.regionOfInterest, -detection.rotation, Color.black);
                // Predict landmarks
                (roi.mean, roi.std) = predictorData.normalization;
                roi.aspectMode = predictorData.aspectMode;
                var landmarks = predictor.Predict(roi);
                // Create pose
                var inputType = predictorModel.inputs[0] as MLImageType;
                var keypoints = new BlazePalmPredictor.Keypoints(landmarks.keypoints.data, inputType, detection.regionOfInterestToImageMatrix);
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
            detectorModel.Dispose();
            predictorModel.Dispose();
        }
        #endregion


        #region --Operations--
        private readonly MLModelData detectorData;
        private readonly MLModelData predictorData;
        private readonly MLEdgeModel detectorModel;
        private readonly MLEdgeModel predictorModel;
        private readonly BlazePalmDetector detector;
        private readonly BlazePalmPredictor predictor;
        private readonly int maxDetections;
        #endregion
    }
}
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
    public sealed partial class BlazePalmPredictor : IMLPredictor<BlazePalmPredictor.Hand> {

        #region --Client API--
        /// <summary>
        /// Create the BlazePalm predictor.
        /// </summary>
        /// <param name="model">BlazePalm ML model.</param>
        public BlazePalmPredictor (MLModel model) => this.model = model as MLEdgeModel;

        /// <summary>
        /// Detect hand landmarks in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Detected hand landmarks.</returns>
        public unsafe Hand Predict (params MLFeature[] inputs) { // INCOMPLETE
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"BlazePalm predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            if (!MLImageType.FromType(input.type))
                throw new ArgumentException(@"BlazePalm predictor expects an an array or image feature", nameof(inputs));  
            // Predict
            var inputType = model.inputs[0] as MLImageType;
            using var inputFeature = (input as IMLEdgeFeature).Create(inputType);
            using var outputFeatures = model.Predict(inputFeature);
            // Marshal
            var keypointsFeature = new MLArrayFeature<float>(outputFeatures[0]);    // or 3
            var scoreFeature = new MLArrayFeature<float>(outputFeatures[1]);
            var handednessFeature = new MLArrayFeature<float>(outputFeatures[2]);
            var keypoints3DFeature = new MLArrayFeature<float>(outputFeatures[3]);  // or 0
            var score = scoreFeature[0];
            var handedness = handednessFeature[0] >= 0.5f ? Handedness.Right : Handedness.Left;
            var keypoints = new Keypoints(keypointsFeature.ToArray(), inputType, Matrix4x4.identity);
            var keypoints3D = new Keypoints3D();
            var result = new Hand(score, handedness, keypoints, keypoints3D);
            return result;
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;

        void IDisposable.Dispose () { } // Not used
        #endregion
    }
}
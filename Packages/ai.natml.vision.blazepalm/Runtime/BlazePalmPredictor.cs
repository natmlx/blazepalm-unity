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
    public sealed partial class BlazePalmPredictor : IMLPredictor<BlazePalmPredictor.Hand> {

        #region --Client API--
        /// <summary>
        /// Detect hand landmarks in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Detected hand landmarks.</returns>
        public unsafe Hand Predict (params MLFeature[] inputs) {
            // Preprocess
            var input = inputs[0];
            if (input is MLImageFeature imageFeature) {
                (imageFeature.mean, imageFeature.std) = model.normalization;
                imageFeature.aspectMode = model.aspectMode;
            }
            // Predict
            var inputType = model.inputs[0] as MLImageType;
            using var inputFeature = (input as IMLEdgeFeature).Create(inputType);
            using var outputFeatures = model.Predict(inputFeature);
            // Marshal
            var keypointsFeature = new MLArrayFeature<float>(outputFeatures[0]);
            var scoreFeature = new MLArrayFeature<float>(outputFeatures[1]);
            var handednessFeature = new MLArrayFeature<float>(outputFeatures[2]);
            var keypoints3DFeature = new MLArrayFeature<float>(outputFeatures[3]);
            var score = scoreFeature[0];
            var handedness = handednessFeature[0] >= 0.5f ? Handedness.Right : Handedness.Left;
            var keypoints = new Keypoints(keypointsFeature.ToArray(), inputType, Matrix4x4.identity);
            var keypoints3D = new Keypoints3D();
            var result = new Hand(score, handedness, keypoints, keypoints3D);
            return result;
        }

        /// <summary>
        /// Dispose the predictor and release resources
        /// </summary>
        public void Dispose () => model.Dispose();

        /// <summary>
        /// Create the BlazePalm predictor.
        /// </summary>
        /// <param name="configuration">Edge model configuration.</param>
        /// <param name="accessKey">NatML access key.</param>
        public static async Task<BlazePalmPredictor> Create (
            MLEdgeModel.Configuration configuration = null,
            string accessKey = null
        ) {
            var model = await MLEdgeModel.Create("@natml/blazepalm-landmark", configuration, accessKey);
            var predictor = new BlazePalmPredictor(model);
            return predictor;
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;

        private BlazePalmPredictor (MLEdgeModel model) => this.model = model;
        #endregion
    }
}
/* 
*   BlazePalm
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed partial class BlazePalmPredictor {

        /// <summary>
        /// Handedness.
        /// </summary>
        public enum Handedness {
            Left = 0,
            Right = 1
        }
        
        /// <summary>
        /// Detected hand landmarks.
        /// </summary>
        public readonly struct Hand {

            #region --Client API--
            /// <summary>
            /// Confidence score.
            /// </summary>
            public readonly float score;

            /// <summary>
            /// Hand handedness.
            /// </summary>
            public readonly Handedness handedness;

            /// <summary>
            /// Normalized image keypoints.
            /// </summary>
            public readonly Keypoints keypoints;

            /// <summary>
            /// Estimated world-space 3D keypoints.
            /// </summary>
            public readonly Keypoints3D keypoints3D;
            #endregion


            #region --Operations--

            internal Hand (float score, Handedness handedness, Keypoints keypoints, Keypoints3D keypoints3D) {
                this.score = score;
                this.handedness = handedness;
                this.keypoints = keypoints;
                this.keypoints3D = keypoints3D;
            }
            #endregion
        }
    }
}
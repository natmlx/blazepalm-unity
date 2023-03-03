/* 
*   BlazePalm
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using NatML.Types;

    public sealed partial class BlazePalmPredictor {

        /// <summary>
        /// Normalized hand keypoints.
        /// The `xy` coordinates of each keypoint correspond to the  normalized position of the 
        /// keypoint in the region of interest. The `z` coordinate corresponds to depth.
        /// </summary>
        public readonly struct Keypoints : IEnumerable<Vector3> {
            
            #region --Client API--
            /// <summary>
            /// Number of landmarks in the hand.
            /// </summary>
            public readonly int Count => data.Length / 3;

            /// <summary>
            /// </summary>
            public readonly Vector3 wrist => this[0];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 thumbCMC => this[1];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 thumbMCP => this[2];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 thumbIP => this[3];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 thumbTip => this[4];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 indexMCP => this[5];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 indexPIP => this[6];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 indexDIP => this[7];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 indexTip => this[8];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 middleMCP => this[9];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 middlePIP => this[10];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 middleDIP => this[11];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 middleTip => this[12];
            
            /// <summary>
            /// </summary>
            public readonly Vector3 ringMCP => this[13];

            /// <summary>
            /// </summary>
            public readonly Vector3 ringPIP => this[14];

            /// <summary>
            /// </summary>
            public readonly Vector3 ringDIP => this[15];

            /// <summary>
            /// </summary>
            public readonly Vector3 ringTip => this[16];

            /// <summary>
            /// </summary>
            public readonly Vector3 pinkyMCP => this[17];

            /// <summary>
            /// </summary>
            public readonly Vector3 pinkyPIP => this[18];

            /// <summary>
            /// </summary>
            public readonly Vector3 pinkyDIP => this[19];

            /// <summary>
            /// </summary>
            public readonly Vector3 pinkyTip => this[20];

            /// <summary>
            /// Get the keypoints for a given index.
            /// </summary>
            public Vector3 this [int idx] {
                get {
                    var rawKeypoint = new Vector3(data[idx * 3 + 0], data[idx * 3 + 1], 1f);
                    var keypoint = Vector3.Scale(rawKeypoint, scale);
                    keypoint.y = 1f - keypoint.y;
                    var depth = data[idx * 3 + 2];
                    var transformedKeypoint = transformation.MultiplyPoint3x4(keypoint);
                    var transformedZero = transformation.MultiplyPoint3x4(Vector3.zero);
                    var transformedRight = transformation.MultiplyPoint3x4(Vector3.right);
                    var zScale = (transformedRight - transformedZero).magnitude;
                    var result = new Vector3(transformedKeypoint.x, transformedKeypoint.y, zScale * depth);
                    return result;
                }
            }
            #endregion


            #region --Operations--
            internal readonly float[] data;
            internal readonly MLImageType imageType;
            private readonly Matrix4x4 transformation;
            private readonly Vector3 scale => new Vector3(1f / imageType.width, 1f / imageType.height, 1f);

            internal Keypoints (float[] data, MLImageType imageType, Matrix4x4 transformation) {
                this.data = data;
                this.imageType = imageType;
                this.transformation = transformation;
            }

            readonly IEnumerator<Vector3> IEnumerable<Vector3>.GetEnumerator () {
                for (var i = 0; i < Count; ++i)
                    yield return this[i];
            }

            readonly IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<Vector3>).GetEnumerator();
            #endregion
        }
    }
}
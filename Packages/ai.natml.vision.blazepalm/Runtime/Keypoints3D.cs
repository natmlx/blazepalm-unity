/* 
*   BlazePalm
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed partial class BlazePalmPredictor {

        public readonly struct Keypoints3D : IEnumerable<Vector3> {

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
            public Vector3 this [int idx] => new Vector3(data[3 * idx + 0], -data[3 * idx + 1], data[3 * idx + 2]);
            #endregion
            
            #region --Operations--
            private readonly float[] data;

            internal Keypoints3D (float[] data) => this.data = data;

            readonly IEnumerator<Vector3> IEnumerable<Vector3>.GetEnumerator () {
                for (var i = 0; i < Count; ++i)
                    yield return this[i];
            }

            readonly IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<Vector3>).GetEnumerator();
            #endregion
        }
    }
}
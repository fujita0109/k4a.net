using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using K4AdotNet.BodyTracking;
using K4AdotNet.Samples.Unity;

namespace SG
{
    //C# では値を保持するクラスを作り、そのインスタンスを渡すのが良い
    //複数のクラスで共有する値なら、その値をクラス定義してインスタンスを共有するといい
    public class SGBone : MonoBehaviour
    {
        public class SGJoint
        {
            public JointType Type { set; get; }
            public Transform Transform { set; get; } = null;

            public string Name { set; get; } = string.Empty;

            public SGJoint(JointType jointType, Transform transform, string name)
            {
                Type = jointType;
                Transform = transform;
                Name = name;
            }
        }

        public SGJoint Joint { set; get; } = null;

        public SGBone Parent { set; get; } = null;
        public List<SGBone> Children { set; get; } = new List<SGBone>();

        public void SetJoint(JointType jointType, Transform transform, string name)
        {
            Joint = new SGJoint(jointType, transform, name);
        }

        public void SetParent(SGBone sgBone)
        {
            Parent = sgBone;
        }

        public void SetCildren(SGBone sgBone)
        {
            Children.Add(sgBone);
        }
    }
}

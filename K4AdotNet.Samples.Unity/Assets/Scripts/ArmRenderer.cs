#if false
using K4AdotNet.BodyTracking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class ArmRenderer : MonoBehaviour
    {
        public GameObject rightArm;
        public GameObject LeftArm;

        private void Awake()
        {
            _root = new GameObject();
            _root.name = "arm:root";
            _root.transform.parent = transform;
            _root.transform.localScale = Vector3.one;
            _root.transform.localPosition = Vector3.zero;
            _root.SetActive(false);


            CreateArm();
        }

        #region Render objects

        private GameObject _root;
        private IReadOnlyDictionary<JointType, Transform> _joints;
        private IReadOnlyCollection<Bone> _bones;
        private Transform _head;

        private class Bone
        {
            public static Bone FromChildJoint(JointType childJoint)
            {
                return new Bone(childJoint.GetParent(), childJoint);
            }

            public Bone(JointType parentJoint, JointType childJoint)
            {
                ParentJoint = parentJoint;
                ChildJoint = childJoint;

                var pos = new GameObject();
                pos.name = $"{parentJoint}->{childJoint}:pos";

                var bone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bone.name = $"{parentJoint}->{childJoint}:bone";
                bone.transform.parent = pos.transform;
                bone.transform.localScale = new Vector3(0.033f, 0.5f, 0.033f);
                bone.transform.localPosition = 0.5f * Vector3.up;

                Transform = pos.transform;
            }

            public JointType ParentJoint { get; }
            public JointType ChildJoint { get; }
            public Transform Transform { get; }
        }

        private void CreateArm()
        {

        }

        #endregion

        private void OnEnable()
        {
            var armProvider = FindObjectOfType<ArmProvider>();

            if (armProvider != null)
            {
                armProvider.ArmUpdated += ArmProvider_ArmUpdated;
            }
        }

        private void OnDisable()
        {
            var armProvider = FindObjectOfType<ArmProvider>();

            if (armProvider != null)
            {
                armProvider.ArmUpdated -= ArmProvider_ArmUpdated;
            }
        }

        private void ArmProvider_ArmUpdated(object sender, ArmEventArgs e)
        {
            if (e.Skeleton == null)
            {
                //引数がEnptyだったらここに来る
            }
            else
            {
                ArmSkeleton(e.Skeleton.Value);
            }
        }

        private void ArmSkeleton(Skeleton skeleton)
        {

            _root.SetActive(true);
        }

        private static Vector3 ConvertKinectPos(Float3 pos)
        {
             // Kinect Y軸が下を向いているため、Y座標を反転
             //ミリメートルをメートルに変換するためのスケール
             // https://docs.microsoft.com/en-us/azure/Kinect-dk/coordinate-systems
             //その他の変換（シーン内のスケルトンの配置、ミラーリング）
             //アセンダントGameObjectのプロパティによって処理
            return 0.001f * new Vector3(pos.X, -pos.Y, pos.Z);
        }

        private void HideSkeleton()
        {
            //親がfalseになったら子供も全てfalseになる
            _root.SetActive(false);
        }
    }
}
#endif
#if true

using K4AdotNet.BodyTracking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using K4AdotNet.Samples.Unity;
using K4AdotNet;

namespace SG
{
    public class SGSkeletonRenderer : MonoBehaviour
    {
        private void SetRootChild(ref GameObject myRoot,ref GameObject parent, string name)
        {
            myRoot = new GameObject();
            //親の設定
            myRoot.transform.SetParent(parent.transform);
            myRoot.name = name;
            myRoot.transform.localScale = Vector3.one;
            myRoot.transform.localPosition = Vector3.zero;
            myRoot.SetActive(false);
        }

        private void Awake()
        {
            _root = new GameObject();
            _root.name = "skeleton:sgRoot";
            _root.transform.parent = transform;
            _root.transform.localScale = Vector3.one;
            _root.transform.localPosition = Vector3.zero;
            _root.SetActive(false);

            CreateRoot();
            CreateJoints();
            CreateHipFollowing();
            //CreateRibsFollowing();
        }

        #region Render objects

        //1層
        private GameObject _root;
        //2層
        private GameObject _hipRoot;
        private GameObject _ribsRoot;

        //hip～
        private GameObject _leftThigh;
        private GameObject _rightThigh;

        private GameObject _leftKnee;
        private GameObject _rightKnee;

        private GameObject _leftAnkle;
        private GameObject _rightAnkle;

        private GameObject _leftToe;
        private GameObject _rightToe;

        //rigs～
        private GameObject _leftShoulderRoot;
        private GameObject _rightShoulderRoot;

        private GameObject _leftUpperArmRoot;
        private GameObject _rightUpperArmRoot;

        private GameObject _leftForearmRoot;
        private GameObject _rightForearmRoot;

        private GameObject _leftWristRoot;
        private GameObject _rightWristRoot;

        private GameObject _neckRoot;
        private GameObject _headRoot;

        private IReadOnlyDictionary<JointType, Transform> _joints;
        private IReadOnlyCollection<Bone> _bones;
        private Transform _head;

        //独自の階層構造を作るのに参照が必要なのでprivateからpublicに変更
        public class Bone
        {
            public static Bone FromChildJoint(JointType childJoint)
            {
                return new Bone(childJoint.GetParent(), childJoint);
            }


            //親と子のpositionの中間地点を計算することでシリンダーの位置がわかる
            public Bone(JointType parentJoint, JointType childJoint)
            {
                ParentJoint = parentJoint;
                ChildJoint = childJoint;

                var pos = new GameObject();

                pos.name = $"{parentJoint}->{childJoint}:pos";

                //シリンダーを作成
                var bone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                bone.name = $"{parentJoint}->{childJoint}:bone";
                bone.transform.parent = pos.transform;
                bone.transform.localScale = new Vector3(0.033f, 0.5f, 0.033f);

                //new Vector3(0,0.5,0);
                //new Vector3(0,0,0)にすると関節にシリンダーの真ん中が来る
                bone.transform.localPosition = 0.5f * Vector3.up;

                Transform = pos.transform;
            }

            public JointType ParentJoint { get; }
            public JointType ChildJoint { get; }
            public Transform Transform { get; }
        }


        private void CreateJoints()
        {
            //ジョイント(関節)は球としてレンダリング
            //シーケンス = 操作対象のデータ
            //ToDictionaryメソッドは即時評価 シーケンスからDictionary<Tkey,Tvalue>を作成
            //一個一個のenumに処理が書ける
            _joints = JointTypes.All.ToDictionary(
                    jt => jt, jt =>
                    {
                        //球のゲームオブジェクトを作成
                        var joint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                        //JointTypeのenum名前
                        joint.name = jt.ToString();

                        //トランスフォームの親に rootのトランスフォーム (0,0,0)
                        joint.transform.parent = _root.transform;

                        //joint.transform.localScale = 0.075f * Vector3.one;
                        joint.transform.localScale = (0.075f * Vector3.one) * 0.5f;

                        return joint.transform;
                    });

            //デフォルトの色として緑を設定
            SetJointColor(Color.green, typeof(JointType).GetEnumValues().Cast<JointType>().ToArray());

            //一部の関節のサイズを少し小さく設定
            SetJointScale(0.05f, JointType.Neck, JointType.Head, JointType.ClavicleLeft, JointType.ClavicleRight, JointType.EarLeft, JointType.EarRight);

            //顔の関節のサイズと特定の色を大幅に縮小して設定
            SetJointScale(0.03f, JointType.EyeLeft, JointType.EyeRight, JointType.Nose);
            SetJointColor(Color.cyan, JointType.EyeLeft, JointType.EyeRight);
            SetJointColor(Color.magenta, JointType.Nose);
            SetJointColor(Color.yellow, JointType.EarLeft, JointType.EarRight);
        }

        //大きさ
        private void SetJointScale(float scale, params JointType[] jointTypes)
        {
            foreach (var jt in jointTypes)
                _joints[jt].localScale = scale * Vector3.one;
        }

        //色
        private void SetJointColor(Color color, params JointType[] jointTypes)
        {
            foreach (var jt in jointTypes)
                _joints[jt].GetComponent<Renderer>().material.color = color;
        }


        private void CreateHipFollowing()
        {
            SGBone hip = new SGBone();

        }

        private static void CreateBones(ICollection<SGBone> list, params JointType[] childJoints)
        {
            foreach (var joint in childJoints)
            {
                //list.Add(Bone.FromChildJoint(joint));
            }
        }

        //=========================================
        //=========================================
        //Bone作成
        /*
        private void CreateBones()
        {
            var bones = new List<Bone>();

            // Spine
            CreateBones(bones, JointType.SpineNavel, JointType.SpineChest, JointType.Neck, JointType.Head);
            // Right arm
            CreateBones(bones, JointType.ClavicleRight, JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight);
            // Left arm
            CreateBones(bones, JointType.ClavicleLeft, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft);
            // Right leg
            CreateBones(bones, JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight);
            // Left leg
            CreateBones(bones, JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft, JointType.FootLeft);

            _bones = bones;
            foreach (var b in _bones)
            {
                b.Transform.parent = _root.transform;
            }
        }

        private static void CreateBones(ICollection<Bone> list, params JointType[] childJoints)
        {
            foreach (var joint in childJoints)
            {
                list.Add(Bone.FromChildJoint(joint));
            }
        }

        private void CreateHead()
        {
            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.8f);
            head.transform.parent = _root.transform;

            _head = head.transform;
        }

         */
        //========================================
        //========================================

        #endregion
        private void OnEnable()
        {
            var skeletonProvider = FindObjectOfType<SkeletonProvider>();
            if (skeletonProvider != null)
            {
                skeletonProvider.SkeletonUpdated += SkeletonProvider_SkeletonUpdated;
            }
        }

        private void OnDisable()
        {
            var skeletonProvider = FindObjectOfType<SkeletonProvider>();
            if (skeletonProvider != null)
            {
                skeletonProvider.SkeletonUpdated -= SkeletonProvider_SkeletonUpdated;
            }
        }

        //(Skeltonクラス) 引数で体の情報が渡って来た)
        private void SkeletonProvider_SkeletonUpdated(object sender, SkeletonEventArgs e)
        {
            if (e.Skeleton == null)
            {
                //HideSkeleton();
            }
            else
            {
                RenderSkeleton(e.Skeleton.Value);
            }
        }

        private void RenderSkeleton(Skeleton skeleton)
        {
            foreach (var item in _joints)
            {
                item.Value.localPosition = ConvertKinectPos(skeleton[item.Key].PositionMm);
            }

            foreach (var bone in _bones)
            {
                PositionBone(bone, skeleton);
            }

            PositionHead(skeleton);

            _root.SetActive(true);
        }

        //改造する
        private static void PositionBone(Bone bone, Skeleton skeleton)
        {
            var parentPos = ConvertKinectPos(skeleton[bone.ParentJoint].PositionMm);
            var direction = ConvertKinectPos(skeleton[bone.ChildJoint].PositionMm) - parentPos;


            bone.Transform.localPosition = parentPos;
            bone.Transform.localScale = new Vector3(1, direction.magnitude, 1);
            bone.Transform.localRotation = UnityEngine.Quaternion.FromToRotation(Vector3.up, direction);
        }

        private void PositionHead(Skeleton skeleton)
        {
            var headPos = ConvertKinectPos(skeleton[JointType.Head].PositionMm);
            var earPosR = ConvertKinectPos(skeleton[JointType.EarRight].PositionMm);
            var earPosL = ConvertKinectPos(skeleton[JointType.EarLeft].PositionMm);
            var headCenter = 0.5f * (earPosR + earPosL);
            var d = (earPosR - earPosL).magnitude;

            _head.localPosition = headCenter;
            _head.localRotation = UnityEngine.Quaternion.FromToRotation(Vector3.up, headCenter - headPos);
            _head.localScale = new Vector3(d, 2 * (headCenter - headPos).magnitude, d);
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

        private void CreateRoot()
        {
            //hip以下のroot
            SetRootChild(ref _hipRoot, ref _root, "_hip:root");
            //left
            SetRootChild(ref _leftThigh, ref _hipRoot, "leftThigh:root");
            SetRootChild(ref _leftKnee, ref _leftThigh, "leftKnee:root");
            SetRootChild(ref _leftAnkle, ref _leftKnee, "leftAnkle:root");
            SetRootChild(ref _leftToe, ref _leftAnkle, "leftToe:root");
            //right
            SetRootChild(ref _rightThigh, ref _hipRoot, "rightThigh:root");
            SetRootChild(ref _rightKnee, ref _rightThigh, "rightThigh:root");
            SetRootChild(ref _rightAnkle, ref _rightKnee, "rightThigh:root");
            SetRootChild(ref _rightToe, ref _rightAnkle, "rightThigh:root");

            //ribs以下のroot
            SetRootChild(ref _ribsRoot, ref _root, "_ribs:root");
            //left
            SetRootChild(ref _leftShoulderRoot, ref _ribsRoot, "leftShoulder:root");
            SetRootChild(ref _leftUpperArmRoot, ref _leftShoulderRoot, "leftUpperArm:root");
            SetRootChild(ref _leftForearmRoot, ref _leftUpperArmRoot, "leftForearm:root");
            SetRootChild(ref _leftWristRoot, ref _leftForearmRoot, "leftWrist:root");
            //right
            SetRootChild(ref _rightShoulderRoot, ref _ribsRoot, "rightShoulder:root");
            SetRootChild(ref _rightUpperArmRoot, ref _rightShoulderRoot, "rightUpperArm:root");
            SetRootChild(ref _rightForearmRoot, ref _rightUpperArmRoot, "rightForearm:root");
            SetRootChild(ref _rightWristRoot, ref _rightForearmRoot, "rightWrist:root");
            //neck
            SetRootChild(ref _neckRoot, ref _ribsRoot, "neckRoot:root");
            SetRootChild(ref _headRoot, ref _neckRoot, "headRoot:root");
        }
    }
}
#endif
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
        }

        #region Render objects

        //1層
        static private GameObject _root;
        //2層
        static private GameObject _hipRoot;
        static private GameObject _ribsRoot;
        static private GameObject _spineChestRoot;

        //hip～
        static private GameObject _leftThighRoot;
        static private GameObject _rightThighRoot;

        static private GameObject _leftKneeRoot;
        static private GameObject _rightKneeRoot;

        static private GameObject _leftAnkleRoot;
        static private GameObject _rightAnkleRoot;

        static private GameObject _leftToeRoot;
        static private GameObject _rightToeRoot;

        //rigs～
        static private GameObject _leftShoulderRoot;
        static private GameObject _rightShoulderRoot;

        static private GameObject _leftUpperArmRoot;
        static private GameObject _rightUpperArmRoot;

        static private GameObject _leftForearmRoot;
        static private GameObject _rightForearmRoot;

        static private GameObject _leftWristRoot;
        static private GameObject _rightWristRoot;

        static private GameObject _neckRoot;
        static private GameObject _headRoot;

        //hand
        static private GameObject _leftHandRoot;
        static private GameObject _leftTipRoot;
        static private GameObject _leftThumbRoot;

        static private GameObject _rightHandRoot;
        static private GameObject _rightTipRoot;
        static private GameObject _rightThumbRoot;

        //顔
        static private GameObject _noseRoot;
        static private GameObject _leftEyeRoot;
        static private GameObject _rightEyeRoot;
        static private GameObject _leftEarRoot;
        static private GameObject _rightEarRoot;

        //もし階層構造が
        private Dictionary<JointType, GameObject> _mapRootJoint = new Dictionary<JointType,GameObject>(){
            {JointType.Pelvis,_hipRoot },
            {JointType.SpineChest,_ribsRoot },
            {JointType.SpineNavel,_spineChestRoot },

            {JointType.Neck,_neckRoot },
            {JointType.ClavicleLeft,_leftShoulderRoot },
            {JointType.ShoulderLeft,_leftUpperArmRoot },
            {JointType.ElbowLeft,_leftForearmRoot },
            {JointType.WristLeft,_leftWristRoot },
            {JointType.HandLeft ,_leftHandRoot},
            {JointType.HandTipLeft,_leftTipRoot},
            {JointType.ThumbLeft,_leftThumbRoot},

            {JointType.ClavicleRight,_rightShoulderRoot },
            {JointType.ShoulderRight,_rightUpperArmRoot },
            {JointType.ElbowRight,_rightForearmRoot },
            {JointType.WristRight,_rightWristRoot },
            {JointType.HandRight ,_rightHandRoot},
            {JointType.HandTipRight,_rightTipRoot},
            {JointType.ThumbRight,_rightThumbRoot },

            {JointType.HipLeft,_leftThighRoot },
            {JointType.KneeLeft,_leftKneeRoot },
            {JointType.AnkleLeft,_leftAnkleRoot },
            {JointType.FootLeft,_leftToeRoot },

            {JointType.HipRight,_rightThighRoot },
            {JointType.KneeRight,_rightKneeRoot },
            {JointType.AnkleRight,_rightAnkleRoot },
            {JointType.FootRight,_rightToeRoot },

            {JointType.Head,_headRoot },

            //顔部分
            {JointType.Nose,_noseRoot},
            {JointType.EyeLeft,_leftEyeRoot },
            {JointType.EarLeft,_leftEarRoot },
            {JointType.EyeRight,_rightEyeRoot },
            {JointType.EarRight,_rightEarRoot },
        };

        //キーと値のペアの読み取り専用ジェネリック コレクションを表す
        //CreateJointでラムダで値を入れている
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
            //一個一個(JointType)のenumに処理が書ける
            _joints = JointTypes.All.ToDictionary(
                    jt => jt, jt =>
                    {
                        //球のゲームオブジェクトを作成
                        var joint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                        //JointTypeのenum名前
                        joint.name = jt.ToString();

                        //トランスフォームの親に rootのトランスフォーム (0,0,0)
                        //joint.transform.parent = _root.transform;

                        //親の指定
                        //mapの値がnull
                        var jointRoot = _mapRootJoint[jt];

                        //joint.transform.SetParent(jointRoot.transform);
                        joint.transform.parent = jointRoot.transform;

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
            SetRootChild(ref _hipRoot, ref _root, "hip:root");
            //left
            SetRootChild(ref _leftThighRoot, ref _hipRoot, "leftThigh:root");
            SetRootChild(ref _leftKneeRoot, ref _leftThighRoot, "leftKnee:root");
            SetRootChild(ref _leftAnkleRoot, ref _leftKneeRoot, "leftAnkle:root");
            SetRootChild(ref _leftToeRoot, ref _leftAnkleRoot, "leftToe:root");
            //right
            SetRootChild(ref _rightThighRoot, ref _hipRoot, "rightThigh:root");
            SetRootChild(ref _rightKneeRoot, ref _rightThighRoot, "rightThigh:root");
            SetRootChild(ref _rightAnkleRoot, ref _rightKneeRoot, "rightThigh:root");
            SetRootChild(ref _rightToeRoot, ref _rightAnkleRoot, "rightThigh:root");

            //ribs以下のroot
            SetRootChild(ref _ribsRoot, ref _root, "ribs:root");
            //left
            SetRootChild(ref _leftShoulderRoot, ref _ribsRoot, "leftShoulder:root");
            SetRootChild(ref _leftUpperArmRoot, ref _leftShoulderRoot, "leftUpperArm:root");
            SetRootChild(ref _leftForearmRoot, ref _leftUpperArmRoot, "leftForearm:root");
            SetRootChild(ref _leftWristRoot, ref _leftForearmRoot, "leftWrist:root");
            SetRootChild(ref _leftHandRoot, ref _leftWristRoot, "leftHandRoot:root");
            SetRootChild(ref _leftTipRoot, ref _leftWristRoot, "leftTipRoot:root");
            SetRootChild(ref _leftThumbRoot, ref _leftWristRoot, "leftThumbRoot:root");

            //right
            SetRootChild(ref _rightShoulderRoot, ref _ribsRoot, "rightShoulder:root");
            SetRootChild(ref _rightUpperArmRoot, ref _rightShoulderRoot, "rightUpperArm:root");
            SetRootChild(ref _rightForearmRoot, ref _rightUpperArmRoot, "rightForearm:root");
            SetRootChild(ref _rightWristRoot, ref _rightForearmRoot, "rightWrist:root");
            SetRootChild(ref _rightHandRoot, ref _rightWristRoot, "rightHandRoot:root");
            SetRootChild(ref _rightTipRoot, ref _rightWristRoot, "rightTipRoot:root");
            SetRootChild(ref _rightThumbRoot, ref _rightWristRoot, "rightThumbRoot:root");

            //neck
            SetRootChild(ref _neckRoot, ref _ribsRoot, "neckRoot:root");
            SetRootChild(ref _headRoot, ref _neckRoot, "headRoot:root");

            //顔
            SetRootChild(ref _noseRoot, ref _headRoot, "noseRoot:root");
            SetRootChild(ref _leftEyeRoot, ref _headRoot, "leftEyeRoot:root");
            SetRootChild(ref _leftEarRoot, ref _headRoot, "leftEarRoot:root");
            SetRootChild(ref _rightEyeRoot, ref _headRoot, "rightEyeRoot:root");
            SetRootChild(ref _rightEarRoot, ref _headRoot, "rightEarRoot:root");

            SetRootChild(ref _spineChestRoot, ref _root, "spineChestRoot:root");
        }
    }
}
#endif
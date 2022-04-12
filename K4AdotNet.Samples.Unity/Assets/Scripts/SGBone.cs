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
        private GameObject _root;

        //ポインタにしたい
        private GameObject _parent;

        public SGBone(GameObject parent,string name)
        {
            _parent = parent;

            _root = new GameObject();
            _root.name = name;
            _root.transform.localScale = Vector3.one;
            _root.transform.localPosition = Vector3.zero;
            _root.SetActive(false);

            //親の設定
            _root.transform.SetParent(parent.transform);
        }
        //public List<SGBone> Children { set; get; } = new List<SGBone>();

        public GameObject GetParent()
        {
            return _parent;
        }

        public GameObject MyGameObject()
        {
            return this.gameObject;
        }
    }
}

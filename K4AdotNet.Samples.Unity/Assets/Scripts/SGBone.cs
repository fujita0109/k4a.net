using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using K4AdotNet.BodyTracking;
using K4AdotNet.Samples.Unity;

namespace SG
{
    //C# �ł͒l��ێ�����N���X�����A���̃C���X�^���X��n���̂��ǂ�
    //�����̃N���X�ŋ��L����l�Ȃ�A���̒l���N���X��`���ăC���X�^���X�����L����Ƃ���
    public class SGBone : MonoBehaviour
    {
        private GameObject _root;

        //�|�C���^�ɂ�����
        private GameObject _parent;

        public SGBone(GameObject parent,string name)
        {
            _parent = parent;

            _root = new GameObject();
            _root.name = name;
            _root.transform.localScale = Vector3.one;
            _root.transform.localPosition = Vector3.zero;
            _root.SetActive(false);

            //�e�̐ݒ�
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

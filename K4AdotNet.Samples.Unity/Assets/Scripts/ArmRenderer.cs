using UnityEngine;
using UnityEngine.UI;

namespace K4AdotNet.Samples.Unity
{
    public class ArmRenderer : MonoBehaviour
    {
        [SerializeField]
        private Image armImage = null;

        [SerializeField]
        private bool isLeft = false;

        private float armStartX;
        private float armStartY;
        private float armMaxY;
        private float armY;

        private float time = 0.0f;

        private void OnEnable()
        {
            var skeletonProvider = FindObjectOfType<SkeletonProvider>();

            if (skeletonProvider != null)
            {
                //イベントの登録
                skeletonProvider.SkeletonUpdated += SkeletonProvider_SkeletonUpdated;
            }
        }

        private void OnDisable()
        {
            var skeletonProvider = FindObjectOfType<SkeletonProvider>();

            if (skeletonProvider != null)
            {
                //イベントの削除
                skeletonProvider.SkeletonUpdated -= SkeletonProvider_SkeletonUpdated;
            }
        }

        private void Start()
        {
            //初期値(0.1) Max(0.4)
            armStartY = Screen.height * 0.1f;
            armMaxY = Screen.height * 0.4f;

            armStartX = (Screen.width * 0.5f) * 0.4f;

            if (isLeft)
            {
                armStartX = -armStartX;
            }

            //anchoredPosition = アンカーを中心とする座標系の位置
            //一番下の中心
            armImage.rectTransform.anchoredPosition = new Vector2(armStartX, armStartY);
        }

        private void Update()
        {
            time += Time.deltaTime;

            if (time > 2)
            {
                //例　Kinectの割合の変わり
                var y = Random.Range(0.0f, 1.0f);

                armY = armStartY + ((armMaxY - armStartY) * y);

                armImage.rectTransform.anchoredPosition = new Vector2(armStartX, armY);
                
                time = 0.0f;
            }
        }


        //Kinectの情報を取ってくる
        private void SkeletonProvider_SkeletonUpdated(object sender, SkeletonEventArgs e)
        {

            //引数のeにスケルトンの情報

            if (e.Skeleton != null)
            {
                //this.gameObject..SetActive(true);
            }
            else
            {
                //_skin?.SetActive(false);
            }
        }
    }
}
﻿#if false
using K4AdotNet.BodyTracking;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class ArmProvider : MonoBehaviour, IInitializable
    {
        private Tracker _tracker;

        public bool IsInitializationComplete { get; private set; }
        public bool IsAvailable { get; private set; }

        public event EventHandler<ArmEventArgs> ArmUpdated;

        private IEnumerator Start()
        {
            //2秒待ってから実行
            yield return new WaitForSeconds(2);

            //() =>と、空の括弧を書けば引数無しという扱い
            //C#のTaskクラス 非同期処理操作
            //同期処理をまとめてタスクを作り、別スレッドで実行する
            var task = Task.Run(() =>
            {
                var initialized = Sdk.TryInitializeBodyTrackingRuntime(TrackerProcessingMode.GpuCuda, out var message);

                return Tuple.Create(initialized, message);
            });


            //WaitUntil = 条件がtrueになったら進む
            //c#
            yield return new WaitUntil(() => task.IsCompleted);

            var isAvailable = false;

            try
            {
                //<bool,string>のタプル型
                var result = task.Result;

                isAvailable = result.Item1;
                if (!isAvailable)
                {
                    Debug.Log($"Cannot initialize body tracking: {result.Item2}");
                }
            }
            catch (Exception ex)
            {
                //nameof 式を使うと、変数、型、またはメンバーの名前が文字列定数として生成
                Debug.LogWarning($"Exception on {nameof(Sdk.TryInitializeBodyTrackingRuntime)}\r\n{ex}");
            }

            if (isAvailable)
            {
                var captureManager = FindObjectOfType<CaptureManager>();

                yield return new WaitUntil(() => captureManager?.IsInitializationComplete != false);

                if (captureManager?.IsAvailable == true)
                {
                    var calibration = captureManager.Calibration;
                    var config = TrackerConfiguration.Default;

                    config.ProcessingMode = TrackerProcessingMode.GpuCuda;
                    //速度のためにライトバージョンのDNNモデルを使用します
                    //（デフォルトのDNNモデルを使用するには次の行にコメントしてください）
                    config.ModelPath = Sdk.BODY_TRACKING_DNN_MODEL_LITE_FILE_NAME;

                    _tracker = new Tracker(in calibration, config);

                    captureManager.CaptureReady += CaptureManager_CaptureReady;
                }
                else
                {
                    isAvailable = false;
                }
            }

            IsAvailable = isAvailable;
            IsInitializationComplete = true;
        }

        private void OnDestroy()
        {
            IsAvailable = false;

            var captureManager = FindObjectOfType<CaptureManager>();

            if (captureManager != null) captureManager.CaptureReady -= CaptureManager_CaptureReady;
            _tracker?.Dispose();
        }

        private void CaptureManager_CaptureReady(object sender, CaptureEventArgs e)
        {
            if (IsAvailable)
            {
                using var capture = e.Capture;
                using var depthImage = capture.DepthImage;
                using var irImage = capture.IRImage;

                if (!(depthImage is null) && !(irImage is null))
                    _tracker.TryEnqueueCapture(capture);
            }
        }

        private void Update()
        {
            if (IsAvailable)
            {
                if (_tracker.TryPopResult(out var bodyFrame))
                {
                    using (bodyFrame)
                    {
                        if (bodyFrame.BodyCount > 0)
                        {
                            //???おそらくキネクトの情報を取ってきている
                            bodyFrame.GetBodySkeleton(0, out var skeleton);

                            //Invoke = 登録されている全てのコールバック
                            ArmUpdated?.Invoke(this, new ArmEventArgs(skeleton));
                        }
                        else
                        {
                            //Emptyとは???おそらくnull
                            ArmUpdated?.Invoke(this, ArmEventArgs.Empty);
                        }
                    }
                }
            }
        }
    }
}
#endif
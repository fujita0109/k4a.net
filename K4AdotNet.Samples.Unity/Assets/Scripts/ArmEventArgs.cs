#if false
using K4AdotNet.BodyTracking;
using System;

namespace K4AdotNet.Samples.Unity
{
    public class ArmEventArgs : EventArgs
    {
        //???
        //(キャプチャークラスには無いもの) Emptyとは
        public static readonly new ArmEventArgs Empty = new ArmEventArgs(null);

    public ArmEventArgs(Skeleton? value)
        {
            Skeleton = value;
        }

        public Skeleton? Skeleton { get; }
    }
}
#endif

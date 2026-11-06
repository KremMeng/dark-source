using UnityEngine.Timeline;

[TrackColor(0.4f, 0.8f, 1f)]
[TrackClipType(typeof(SwordTrailClip))]
[TrackBindingType(typeof(PlayerSwordTrail))]//绑定刀光播放器，运行时通过它获取刀光生成位置

public class SwordTrailTrack : TrackAsset{
    
}

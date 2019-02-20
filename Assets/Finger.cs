
using UnityEngine;

namespace TouchSys.Finger
{
    public class Finger
    {
        public int Index;

        public bool Touching;

        public bool LastTouching;

        public bool Tapped;

        public bool Swiped;

        public Vector2 StartScreenPosition;

        public Vector2 LastScreenPosition;

        public Vector2 ScreenPosition;
        
        public bool IsActive => TouchSystem.Fingers.Contains(this);
        public bool Down => Touching == true && LastTouching == false;
        public bool Up => Touching == false && LastTouching == true;
        public Vector2 ScreenDelta => ScreenPosition - LastScreenPosition;
        public Vector2 SwipeScreenDelta => ScreenPosition - StartScreenPosition;
    }
}
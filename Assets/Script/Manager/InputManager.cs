using System.Collections.Generic;
using UnityEngine;

namespace Script.Manager
{
    public enum InputCode
    {
        Crouch,
        Jump,
        Fast,   //use for running, nitro etc
        Slow,   //use for handbrake, skid and walk

        Horizontal,
        Vertical,

        Action,
        Fire,
        Aim,
        Scroll,
        MouseX,
        MouseY,

        CameraChange,
        AlternateCamera,
        LookBehind,
    }

    public enum InputType
    {
        Down,
        Instant,
        Up,
    }

    public class InputManager : Singleton<InputManager>
    {
        private delegate bool InputButtonDel(string s);
        private delegate bool InputKeyDel(KeyCode k);
        private readonly InputButtonDel[] inputButtonDels = { Input.GetButtonDown, Input.GetButton, Input.GetButtonUp };
        private readonly InputKeyDel[] inputKeyDels = { Input.GetKeyDown, Input.GetKey, Input.GetKeyUp };

        private class InputHandlerBase
        {
            protected InputCode[] codes;
            protected bool[] inputArray;

            public InputHandlerBase(InputCode[] codes)
            {
                this.codes = codes;
                inputArray = new bool[codes.Length];
            }

            public virtual void UpdateInput()
            {
                SetInput();
            }

            public void ResetInput()
            {
                for (var i = 0; i < inputArray.Length; i++)
                    inputArray[i] = false;
            }

            protected void SetInput()
            {
                for (var i = 0; i < codes.Length; i++)
                    inputArray[i] = Instance.GetButton(codes[i], InputType.Instant);
            }

            public bool GetInput()
            {
                for (var i = 0; i < inputArray.Length; i++)
                    if (!inputArray[i])
                        return false;
                return true;
            }
        }

        private class InputHandlerTimed : InputHandlerBase
        {
            private int currentFrame = 0;
            private int frameSkip;

            public InputHandlerTimed(int delay, InputCode[] codes) : base(codes)
            {
                frameSkip = delay;
            }

            public override void UpdateInput()
            {
                currentFrame = Mathf.Clamp(++currentFrame, 0, frameSkip);
                if (currentFrame == frameSkip)
                    ResetInput();
                base.UpdateInput();
            }
        }

        //TODO: Incomplete, usage : cheat codes and shit
        private class InputHandlerOrdered : InputHandlerBase
        {
            int progressIndex;
            KeyCode[] ignoreCodes;

            public InputHandlerOrdered(KeyCode[] ignoreCodes, InputCode[] codes) : base(codes)
            {
            }

            public override void UpdateInput()
            {
                if ((progressIndex == 0 || inputArray[progressIndex - 1]))
                {
                    inputArray[progressIndex] = true;
                    progressIndex = Mathf.Clamp(++progressIndex, 0, codes.Length);
                }
            }
        }

        private Dictionary<int, InputHandlerTimed> timedHandlers = new Dictionary<int, InputHandlerTimed>();
        int id = -1;

        private void Update()
        {
            foreach (var handler in timedHandlers)
                handler.Value.UpdateInput();
        }

        public int AddTimed(int delay, InputCode[] inputCodes)
        {
            id++;
            timedHandlers.Add(id, new InputHandlerTimed(delay, inputCodes));
            return id;
        }

        public void RemoveTimed(int id)
        {
            timedHandlers.Remove(id);
        }

        public bool GetInput(int id)
        {
            return timedHandlers[id].GetInput();
        }

        public float GetAxis(InputCode input)
        {
            return Input.GetAxis(input.ToString());
        }

        public bool GetButton(InputCode input, InputType type)
        {
            return inputButtonDels[(int)type](input.ToString());
        }

        private bool GetButton(KeyCode input, InputType type) 
        {
            return inputKeyDels[(int)type](input);
        }
    }
}
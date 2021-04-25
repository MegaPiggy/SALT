using UnityEngine;

namespace SAL
{
    public interface InputDetector : UISDetector
    {
        void InputBegan(UserInputService.InputObject inputObject, bool wasProcessed);

        void InputChanged(UserInputService.InputObject inputObject, bool wasProcessed);

        void InputEnded(UserInputService.InputObject inputObject, bool wasProcessed);
    }
}

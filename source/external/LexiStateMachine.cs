using System;
using System.Collections.Generic;
using System.Text;

namespace Lexicon_StateMachine
{
    internal class LexiStateMachine
    {
        protected Stack<BaseState> stateStack = new Stack<BaseState>();

        public bool Setup<InitalStateClass>() where InitalStateClass : BaseState
        {
            BaseState? initalState = Activator.CreateInstance(typeof(InitalStateClass), [this]) as InitalStateClass;
            if (initalState == null)
            {
                return false;
            }

            initalState.SetupState();                                                                                

            stateStack.Push(initalState);

            return true;
        }

        public bool RunState()
        {
            if (stateStack.Count == 0)
            {
                return false;
            }

            stateStack.Peek().RunState();

            return true;
        }

        public void PopState()
        {
            stateStack.Pop().CleanupState();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lexicon_StateMachine
{
    internal abstract class BaseState
    {
        protected bool ReadUserInput(out string outLowerCaseInput, string aPrefixCursor = "> ")
        {
            Console.Write(aPrefixCursor);

            outLowerCaseInput = "";

            {
                string? userInput = Console.ReadLine();

                if (userInput != null)
                {
                    outLowerCaseInput = userInput.ToLower();
                }
            }

            // Special case as 'back' will always take you back to the main menu
            if (GetLowercaseBackKeywords().Contains(outLowerCaseInput))
            {
                return true;
            }

            return false;
        }

        protected virtual List<string> GetLowercaseBackKeywords() { return new List<string>(); }

        protected LexiStateMachine StateMachine;
        public BaseState(LexiStateMachine aOwningStateMachine)
        {
            StateMachine = aOwningStateMachine;
        }

        public abstract void SetupState();
        public abstract void RunState();
        public abstract void CleanupState();
    }
}

using Lexicon_Miniproject_SQLAssetTracking;
using Lexicon_StateMachine;
namespace Lexicon_Miniproject_SQLAssetTracking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool programRunning = true;
            LexiStateMachine stateMachine = new LexiStateMachine();
            stateMachine.Setup<AssetTrackingState>();
            PriceConverter.LoadConversions();
            do
            {
                programRunning = stateMachine.RunState();
            } while (programRunning);
        }
    }
}

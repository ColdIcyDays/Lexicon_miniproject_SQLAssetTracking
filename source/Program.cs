using Lexicon_Miniproject_SQLAssetTracking;
using Lexicon_StateMachine;
namespace Lexicon_Miniproject_SQLAssetTracking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            
            PriceConverter.LoadConversions();
            PriceConverter.LoadCodeToSymbols();
            
            bool programRunning = true;
            LexiStateMachine stateMachine = new LexiStateMachine();
            stateMachine.Setup<AssetTrackingState>();
            do
            {
                programRunning = stateMachine.RunState();
            } while (programRunning);
        }
    }
}

using AurOS;
using MelonLoader;


namespace NumbersGoUp
{
    internal class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Api.RegisterProgram(new Game2048());
            LoggerInstance.Msg("Initialized Successfully!");
        }
    }
}

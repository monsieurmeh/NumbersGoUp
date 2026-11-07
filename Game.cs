using AurOS;
using UnityEngine;
using Canvas = AurOS.Canvas;


namespace NumbersGoUp
{
    internal abstract class Game : ICanvasProgram
    {
        protected static readonly System.Random Random = new System.Random();

        public abstract string Name { get; }
        public Texture? Screen => null;
        protected abstract Prompt[] Prompts { get; }


        public void Start(Controls controls)
        {
            Api.SetPrompts(Prompts);
            NewGame();
        }


        public void Update(Controls controls)
        {
            if (controls.ReloadPressed)
            {
                NewGame();
                return;
            }
            Play(controls, Time.unscaledDeltaTime);
        }


        public void Stop()
        {
        }


        public abstract void Draw(Canvas canvas);
        protected abstract void NewGame();
        protected abstract void Play(Controls controls, float deltaTime);


        protected static void Title(Canvas canvas, string title, string status)
        {
            canvas.Text(title, 8, 4, 12, Palette.Green);
            canvas.Text(status, Canvas.Width - 8, 4, 12, Palette.Dim, TextAlign.Right);
        }
    }
}

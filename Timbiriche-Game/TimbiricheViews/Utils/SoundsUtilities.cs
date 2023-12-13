using System.Media;


namespace TimbiricheViews.Utils
{
    public static class SoundsUtilities
    {
        private static readonly SoundPlayer btnLine = new SoundPlayer(Utilities.BuildAbsolutePath(@"..\..\Resources\Sounds\clicLine.wav"));
        private static readonly SoundPlayer squareComplete = new SoundPlayer(Utilities.BuildAbsolutePath(@"..\..\Resources\Sounds\squareComplete.wav"));

        public static  void PlayButtonClicLineSound()
        {
            btnLine.Play();
        }

        public static void PlaySquareCompleteSound()
        {
            squareComplete.Play();
        }
    }
}

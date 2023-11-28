using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheViews.Utils
{
    public class SoundsUtilities
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

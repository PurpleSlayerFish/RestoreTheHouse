using Runtime.Game.Ui.Windows.TouchPad;
using SimpleUi;

namespace Runtime.Game.Ui.Windows.StartToPlay 
{
    public class StartToPlayWindow : WindowBase 
    {
        public override string Name => "StartToPlay";
        protected override void AddControllers()
        {
            AddController<StartToPlayController>();    
            AddController<TouchpadViewController>();
        }
    }
}
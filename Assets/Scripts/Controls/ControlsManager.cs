namespace Scripts.Controls
{
    public static class ControlsManager
    {
        public static Controls Controls { get; private set; }


        static ControlsManager()
        {
            Controls = new();
            Controls.Enable();
        }
    }
}
namespace MetaScripts
{
    public interface IDeactivable
    {
        int GetAppearanceFrequency();
        void Deactivate();
        void Activate();
    }
}
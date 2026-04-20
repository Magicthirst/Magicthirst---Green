namespace Common
{
    public interface IGameNavigation
    {
        void FailLevel();
        void GoMainMenu();
        void GoGame();
        void GoJoinSession();
        void GoSignIn();
        void QuitGame();
    }
}
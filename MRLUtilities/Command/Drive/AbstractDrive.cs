
namespace MRL.Command.Drive
{
    public abstract class AbstractDrive
    {
        protected string _type;
        public abstract string Type { get; }
        public abstract string GetCommand(string cmd);
        public abstract string GetStopCommand();
    }
}

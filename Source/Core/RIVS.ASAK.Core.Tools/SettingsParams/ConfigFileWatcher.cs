using System.IO;

namespace RIVS.ASAK.Core.Tools.SettingsParams
{
    /// <summary>
    /// Ожидальщик изменений файла RIVSASAK.config
    /// </summary>
    public class ConfigFileWatcher
    {
        private FileSystemDelayWatcher _fileWatcher = new FileSystemDelayWatcher(RIVSASAKParams.RIVSASAKExecutionPath, ParamsFromFile.ConfigFileName, 2000);

        public ConfigFileWatcher()
        {
            _fileWatcher.ChangedDelay += OnFileChanged;
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _fileWatcher.EnableRaisingEvents = true;
        }

        void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if(Changed != null)
            {
                Changed.Invoke(sender, e);
            }
        }

        public event FileSystemEventHandler Changed;
    }
}

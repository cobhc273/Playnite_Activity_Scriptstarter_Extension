using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ActivityScripts
{
    public class ActivityScripts : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private const string FULLSCREEN = "Fullscreen";
        private const string DESKTOP = "Desktop";
        //private const string ONAPPSTART = "OnApplicationStarted";
        //private const string ONAPPSTOP = "OnApplicationStopped";

        private string scriptPath;

        private ActivityScriptsSettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("6d7ad2c2-3859-4205-8c36-4f5765afdb37");

        public ActivityScripts(IPlayniteAPI api) : base(api)
        {
            settings = new ActivityScriptsSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };


            string basePath = Path.Combine(api.Paths.ApplicationPath, "Extensions","ActivityScripts", "Scripts");


            if (api.ApplicationInfo.Mode == ApplicationMode.Fullscreen)
            {
                scriptPath = Path.Combine(basePath, FULLSCREEN);
            }
            else if (PlayniteApi.ApplicationInfo.Mode == ApplicationMode.Desktop)
            {
                scriptPath = Path.Combine(basePath, DESKTOP);
            }


            AddSettingsSupport(new AddSettingsSupportArgs
            {
                SourceName = "ActivityScripts",
                SettingsRoot = "Settings"
            });
        }

        public override void OnGameInstalled(OnGameInstalledEventArgs args)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(OnGameStartedEventArgs args)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Add code to be executed when Playnite is initialized.
            if (PlayniteApi.ApplicationInfo.Mode == ApplicationMode.Fullscreen)
            {
                ExecuteCommandlist(settings.Settings.ScriptlistStarted_Fullscreen);
            }
            else if (PlayniteApi.ApplicationInfo.Mode == ApplicationMode.Desktop)
            {
                ExecuteCommandlist(settings.Settings.ScriptlistStarted_Desktop);
            }
;           
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Add code to be executed when Playnite is shutting down.
            if (PlayniteApi.ApplicationInfo.Mode == ApplicationMode.Fullscreen)
            {
                ExecuteCommandlist(settings.Settings.ScriptlistStopped_Fullscreen);
            }
            else if (PlayniteApi.ApplicationInfo.Mode == ApplicationMode.Desktop)
            {
                ExecuteCommandlist(settings.Settings.ScriptlistStopped_Desktop);
            }

        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new ActivityScriptsSettingsView();
        }

        private void ExecuteScriptsInDirectory(string dir)
        {
            //Testing procedure for executing files from directory. Not used anymore
            if (Directory.Exists(dir))
            {

                string[] files = Directory.GetFiles(dir);
                foreach (string file in files)
                {
                    bool executable = false;
                    string exFile = Path.Combine(dir, file);

                    switch (Path.GetExtension(file))
                    {
                        case ".exe":
                        {
                            executable = true;
                            break;
                        }
                        case ".bat":
                        {
                            executable = true;
                            break;
                        } 
                    }    

                    if (executable)
                    {
                        Process.Start(exFile);
                    }
                }
            }
            else
            {
                PlayniteApi.Dialogs.ShowErrorMessage("Path does not exist: " +  dir, "ERROR");
            }
        }


        private void ExecuteCommandlist(string commandlist)
        {
            if (!string.IsNullOrEmpty(commandlist.Trim()))
            {
                //Rule: one line -> one command
                string[] lines = commandlist.Split(new string[] { Environment.NewLine },
                                                   StringSplitOptions.None
                                                   );
                foreach (string command in lines)
                {
                    //Process.Start("cmd.exe", "/C " + command);

                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.CreateNoWindow = true;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "powershell.exe";
                    startInfo.UseShellExecute = false;
                    startInfo.Arguments = command;
                    process.StartInfo = startInfo;
                    process.Start();
                }
            }


        }
    }
}
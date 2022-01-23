using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityScripts
{
    public class ActivityScriptsSettings : ObservableObject
    {
        private string scriptlistStarted_Desktop = string.Empty;
        private string scriptlistStarted_Fullscreen = string.Empty;
        private string scriptlistStopped_Desktop = string.Empty;
        private string scriptlistStopped_Fullscreen = string.Empty;
        private bool optionThatWontBeSaved = false;

        public string ScriptlistStarted_Desktop { get => scriptlistStarted_Desktop; set => SetValue(ref scriptlistStarted_Desktop, value); }
        public string ScriptlistStarted_Fullscreen { get => scriptlistStarted_Fullscreen; set => SetValue(ref scriptlistStarted_Fullscreen, value); }
        public string ScriptlistStopped_Desktop { get => scriptlistStopped_Desktop; set => SetValue(ref scriptlistStopped_Desktop, value); }
        public string ScriptlistStopped_Fullscreen { get => scriptlistStopped_Fullscreen; set => SetValue(ref scriptlistStopped_Fullscreen, value); }
        // Playnite serializes settings object to a JSON object and saves it as text file.
        // If you want to exclude some property from being saved then use `JsonDontSerialize` ignore attribute.
        [DontSerialize]
        public bool OptionThatWontBeSaved { get => optionThatWontBeSaved; set => SetValue(ref optionThatWontBeSaved, value); }
    }

    public class ActivityScriptsSettingsViewModel : ObservableObject, ISettings
    {
        private readonly ActivityScripts plugin;
        private ActivityScriptsSettings editingClone { get; set; }

        private ActivityScriptsSettings settings;
        public ActivityScriptsSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public ActivityScriptsSettingsViewModel(ActivityScripts plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<ActivityScriptsSettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new ActivityScriptsSettings();
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}
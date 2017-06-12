using CSharp_Lua_Scripting_Engine;
using NLua;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IScriptable, ILuaConsole, INotifyPropertyChanged
    {
        private DefaultScriptingEngine<object> _engine;

        private ObservableCollection<Tuple<object, object>> _logs = new ObservableCollection<Tuple<object, object>>();

        private CancellationTokenSource _tokenSource;

        public MainWindow()
        {
            InitializeComponent();

            _engine = new DefaultScriptingEngine<object>(this)
            {
                // Source will need to be changed to match your source directory
                SourceDirectory = @"C:\Users\Temdog007\Documents\GitHub\CSharp-Lua-Scripting-Engine\WpfExample\Scripts",
                DestinationDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Scripts")
            };

            _tokenSource = new CancellationTokenSource();

            LoadData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Tuple<object, object>> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged("Logs");
            }
        }

        public ScriptableType ScriptableType => ScriptableType.File;

        public void Clear()
        {
            Logs.Clear();
        }

        public void DeleteAt(int index)
        {
            Logs.RemoveAt(index);
        }

        public string GetScriptSource(object scriptType)
        {
            return Path.Combine("Scripts", "Values.lua");
        }

        public void Write(string message)
        {
            Logs.Add(Tuple.Create<object, object>("Error", message));
        }

        protected override void OnClosed(EventArgs e)
        {
            _tokenSource.Cancel();
            _engine.Dispose();
            base.OnClosed(e);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Logs.Clear();
            
            _engine.ReloadAllScripts();
            _engine.Update();
            LoadData();
        }

        private void LoadData()
        {
            var script = _engine.GetScript(this, 0);
            foreach (var obj in script.Run("GetValues") ?? Enumerable.Empty<object>())
            {
                if (obj is LuaTable)
                {
                    LuaTable table = obj as LuaTable;
                    foreach (var pair in table.ToEnumerable())
                    {
                        Logs.Add(pair);
                    }
                }
            }
        }
    }
}
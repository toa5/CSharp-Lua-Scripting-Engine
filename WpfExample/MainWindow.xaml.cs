using CSharp_Lua_Scripting_Engine;
using NLua;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IScriptingEngineConsole, INotifyPropertyChanged
    {
        private DefaultScriptingEngine<Script> _engine;

        private ObservableCollection<Tuple<object, object>> _logs = new ObservableCollection<Tuple<object, object>>();

        private CancellationTokenSource _tokenSource;

        public MainWindow()
        {
            InitializeComponent();

            _engine = new DefaultScriptingEngine<Script>(this)
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

        public void Clear()
        {
            Logs.Clear();
        }

        public void DeleteAt(int index)
        {
            Logs.RemoveAt(index);
        }

        public void Write(string message)
        {
            Logs.Add(Tuple.Create<object, object>("Error", message));
        }

        public void Write(string message, params object[] args)
        {
            Logs.Add(Tuple.Create<object, object>("Error", string.Format(message, args)));
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
            LoadData();
        }

        private void LoadData()
        {
            var script = _engine.GetScript(this, "Scripts/Values.lua", ScriptSourceType.File);
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
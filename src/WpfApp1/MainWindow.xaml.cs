using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    using System.Diagnostics;
    using System.Windows.Threading;
    using OpenTracing.Contrib.SystemDiagnostics.ToOpenTracing;
    using OpenTracing.Util;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainWindow.ConfigureTraceListeners();
            MainWindow.ConnectTracing(this.Tree);

            App.Current.Dispatcher.InvokeAsync(
                async () => TestDataGenerator.GenerateTestData());
        }

        private static void ConnectTracing(TreeView tree)
        {
            GlobalTracer.Register(TreeViewTracer.Create(tree));
        }

        private static void ConfigureTraceListeners()
        {
            // Send CW to OT
            var rawConsoleOut = Console.Out;
            Console.SetOut(new OpenTracingTextWriter(rawConsoleOut));

            // Send Trace.Write to OT
            Trace.Listeners.Add(new OpenTracingTraceListener());
        }
    }
}

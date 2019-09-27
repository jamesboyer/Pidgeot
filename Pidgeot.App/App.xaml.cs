using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pidgeot.App
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      var window = new MainWindow();
      var interactionNotifier = new InteractionNotifier(window);
      var grpcServer = new Server
      {
        Services = { Interactions.InteractionReporter.BindService(interactionNotifier) },
        Ports = { new ServerPort("127.0.0.1", 50051, ServerCredentials.Insecure) }
      };
      grpcServer.Start();

      window.ShowDialog();

      grpcServer.ShutdownAsync().Wait();
    }
  }
}

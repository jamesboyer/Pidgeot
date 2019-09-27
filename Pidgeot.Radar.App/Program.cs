using Grpc.Core;
using Pidgeot.Interactions;
using System;
using System.Linq;

namespace Pidgeot.Radar.App
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello World! Hit enter to close the app at any time!");

      var channel = new Channel("127.0.0.1", 50051, ChannelCredentials.Insecure);

      var client = new InteractionReporter.InteractionReporterClient(channel);

      var request = new PidgeyInteractionsNofityRequest() { MinSeconds = 1 };
      var disposable = client
        .GetPidgeyInteractionsStream(request)
        .ResponseStream
        .ReadAllAsync()
        .ToObservable()
        .Subscribe(interaction => Console.WriteLine($"Pidgey spotted! You saw it for {interaction.SecondsHovered} seconds."));

      Console.ReadLine();
    }
  }
}

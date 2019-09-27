using Grpc.Core;
using Pidgeot.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pidgeot.App
{
  public class InteractionNotifier : InteractionReporter.InteractionReporterBase
  {
    private readonly MainWindow _window;

    public InteractionNotifier(MainWindow window)
    {
      _window = window ?? throw new ArgumentNullException( nameof( window ) );
    }

    public override Task GetPidgeyInteractionsStream(
      PidgeyInteractionsNofityRequest request,
      IServerStreamWriter<PidgeyInteraction> responseStream,
      ServerCallContext context)
    {
      return Observable.Create<PidgeyInteraction>(observer =>
      {
        return _window
          .Pidgey
          .Events()
          .MouseEnter
          .Subscribe(_ =>
          {
            var mouseEntered = DateTime.UtcNow;
            _window.Pidgey
            .Events()
            .MouseLeave
            .Take(1)
            .Subscribe(__ =>
            {
              var delta = DateTime.UtcNow - mouseEntered;
              if (delta.TotalSeconds < request.MinSeconds)
                return;
              var interaction = new PidgeyInteraction { SecondsHovered = delta.TotalSeconds };
              observer.OnNext(interaction);
            });
          });
      })
      .Do(async interaction => await responseStream.WriteAsync(interaction))
      .ToTask();
    }
  }
}

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Threading;
using Trybot.CircuitBreaker;
using Trybot.DistributedCB.Annotations;

namespace Trybot.DistributedCB
{
    public class CbViewModel : INotifyPropertyChanged
    {
        private bool throwException;
        public bool ThrowException
        {
            get => this.throwException;
            set { this.throwException = value; this.OnPropertyChanged(); }
        }

        private SolidColorBrush bgColor = new SolidColorBrush(Colors.ForestGreen);
        public SolidColorBrush BgColor
        {
            get => this.bgColor;
            set { this.bgColor = value; this.OnPropertyChanged(); }
        }

        private string text = "Closed";
        public string Text

        {
            get => this.text;
            set { this.text = value; this.OnPropertyChanged(); }
        }

        private readonly IBotPolicy cbBotPolicy;

        private readonly DispatcherTimer timer;

        public CbViewModel(string key, ConcurrentDictionary<string, CircuitState> states)
        {
            this.cbBotPolicy = new BotPolicy();
            this.cbBotPolicy.Configure(policyConfig => policyConfig
                .CircuitBreaker(cbConfig => cbConfig
                    .WithStateHandler(new DistributedCircuitStateHandler(states, key,
                        openStatePercentageIndicator: .5,
                        healDuration: TimeSpan.FromSeconds(10)))
                    .DurationOfOpen(TimeSpan.FromSeconds(10))
                    .BrakeWhenExceptionOccurs(ex => true)
                    .OnClosed(() =>
                    {
                        this.Text = "Closed";
                        this.BgColor = new SolidColorBrush(Colors.ForestGreen);
                    })
                    .OnOpen(t =>
                    {
                        this.Text = "Open";
                        this.BgColor = new SolidColorBrush(Colors.Red);
                    })
                    .OnHalfOpen(() =>
                    {
                        this.Text = "HalfOpen";
                        this.BgColor = new SolidColorBrush(Colors.Yellow);
                    }), stratConfig => stratConfig
                        .FailureThresholdBeforeOpen(2)
                        .SuccessThresholdInHalfOpen(2)));

            this.timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            this.timer.Tick += this.TimerOnTick;
            this.timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            try
            {
                this.cbBotPolicy.Execute(() =>
                    {
                        if (this.ThrowException)
                            throw new Exception();
                    });
            }
            catch
            {
                // ignored
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using MinimBlackjack.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinimBlackjack
{
    /// <summary>
    /// Logique d'interaction pour PlayingCard.xaml
    /// </summary>
    public partial class PlayingCardUI : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty PlayingCardProperty =
            DependencyProperty.Register("PlayingCard", typeof(PlayingCard), typeof(PlayingCardUI), new PropertyMetadata(new PlayingCard
            {
                Suit = CardSuit.None,
                Value = "",
                IsFaceDown = true
            }));

        public PlayingCard PlayingCard
        {
            get => (PlayingCard)GetValue(PlayingCardProperty);
            set => SetValue(PlayingCardProperty, value);
        }

        public string ValueString => PlayingCard.IsFaceDown ? "" : PlayingCard.Value;

        public string SuitString => PlayingCard.IsFaceDown ? "" : PlayingCard.Suit.ToDescriptionString();

        protected string Color => (PlayingCard.IsFaceDown || PlayingCard.Suit == CardSuit.None)
            ? "#ffd550"
            : (PlayingCard.Suit == CardSuit.Hearts || PlayingCard.Suit == CardSuit.Diamonds)
            ? "#f05b55"
            : "#404040";
        public Brush CardBrush => (Brush)new BrushConverter().ConvertFromString(Color);

        public PlayingCardUI()
        {
            InitializeComponent();
            Loaded += (s, e) => SubscribeToCardChanges();
        }


        private void SubscribeToCardChanges()
        {
            if (PlayingCard != null)
            {
                PlayingCard.PropertyChanged += PlayingCard_PropertyChanged;
            }
        }

        private void PlayingCard_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlayingCard.IsFaceDown) || e.PropertyName == nameof(PlayingCard.Suit))
            {
                // Notify that CardBrush has changed
                OnPropertyChanged(nameof(ValueString));
                OnPropertyChanged(nameof(SuitString));
                OnPropertyChanged(nameof(CardBrush));
            }
        }

        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}

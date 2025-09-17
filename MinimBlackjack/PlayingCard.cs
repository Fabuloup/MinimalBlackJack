using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimBlackjack
{
    public class PlayingCard : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private CardSuit _suit = CardSuit.None;
        public CardSuit Suit
        {
            get => _suit;
            set
            {
                if (_suit != value)
                {
                    _suit = value;
                    OnPropertyChanged(nameof(Suit));
                }
            }
        }
        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        private bool _isFaceDown = false;
        public bool IsFaceDown
        {
            get => _isFaceDown;
            set
            {
                if (_isFaceDown != value)
                {
                    _isFaceDown = value;
                    OnPropertyChanged(nameof(IsFaceDown));
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum CardSuit
    {
        [Description("")]
        None,
        [Description("♥")]
        Hearts,
        [Description("♦")]
        Diamonds,
        [Description("♣")]
        Clubs,
        [Description("♠")]
        Spades
    }
}

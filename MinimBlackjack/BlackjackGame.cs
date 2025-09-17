using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimBlackjack
{
    public class BlackjackGame : INotifyPropertyChanged
    {
        // Singleton
        private static readonly Lazy<BlackjackGame> _instance = new(() => new BlackjackGame());
        public static BlackjackGame Instance => _instance.Value;

        public event PropertyChangedEventHandler? PropertyChanged;

        // Cartes du croupier et du joueur
        private int _wins;
        public int Wins
        {
            get => _wins;
            private set
            {
                _wins = value;
                OnPropertyChanged(nameof(Wins));
            }
        }

        private int _losses;
        public int Losses
        {
            get => _losses;
            private set
            {
                _losses = value;
                OnPropertyChanged(nameof(Losses));
            }
        }

        // Argent du joueur
        private int _playerMoney;
        public int PlayerMoney
        {
            get => _playerMoney;
            private set
            {
                _playerMoney = value;
                OnPropertyChanged(nameof(PlayerMoney));
            }
        }

        // Mise en jeu
        private int _currentBet;
        public int CurrentBet
        {
            get => _currentBet;
            private set
            {
                _currentBet = value;
                OnPropertyChanged(nameof(CurrentBet));
            }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        // Cartes du croupier et du joueur
        public ObservableCollection<PlayingCard> DealerCards { get; private set; }
        public ObservableCollection<PlayingCard> PlayerCards { get; private set; }

        // Paquet de cartes complet
        public List<PlayingCard> Deck { get; private set; }

        // Constructeur privé
        private BlackjackGame()
        {
            DealerCards = new ObservableCollection<PlayingCard>();
            PlayerCards = new ObservableCollection<PlayingCard>();
            FullReset();
        }

        public void FullReset()
        {
            PlayerMoney = 1000;
            CurrentBet = 0;
            Wins = 0;
            Losses = 0;
            Reset();
        }

        public void Reset()
        {
            IsGameFinished = false;
            StatusMessage = string.Empty;
            DealerCards.Clear();
            PlayerCards.Clear();
            Deck = CreateDeck();
            ShuffleDeck();

            OnPropertyChanged(nameof(HasGameNotStarted));
            OnPropertyChanged(nameof(IsGameRunning));
        }

        #region Méthodes pour gérer le jeu

        // Création du paquet de cartes
        private List<PlayingCard> CreateDeck()
        {
            var suits = new[] { CardSuit.Spades, CardSuit.Hearts, CardSuit.Diamonds, CardSuit.Clubs };
            var ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            var deck = new List<PlayingCard>();
            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    deck.Add(new PlayingCard
                    {
                        Suit = suit,
                        Value = rank,
                        IsFaceDown = true
                    });
                }
            }
            return deck;
        }

        // Mélange du paquet
        private void ShuffleDeck()
        {
            var rng = new Random();
            int n = Deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (Deck[k], Deck[n]) = (Deck[n], Deck[k]);
            }
        }

        private PlayingCard DrawCard(bool isFaceDown = false)
        {
            var card = Deck[0];
            Deck.RemoveAt(0);
            card.IsFaceDown = isFaceDown;
            return card;
        }

        private int CalculateHandValue(IEnumerable<PlayingCard> hand)
        {
            int value = 0;
            int aceCount = 0;
            foreach (var card in hand)
            {
                if (card.Value == "A")
                {
                    aceCount++;
                    value += 11; // Compter l'As comme 11 pour l'instant
                }
                else if (new[] { "K", "Q", "J" }.Contains(card.Value))
                {
                    value += 10;
                }
                else
                {
                    value += int.Parse(card.Value);
                }
            }
            // Ajuster la valeur des As si nécessaire
            while (value > 21 && aceCount > 0)
            {
                value -= 10; // Compter un As comme 1 au lieu de 11
                aceCount--;
            }
            return value;
        }

        public void PlaceBet(int amount = 1)
        {
            if (amount > 0 && amount <= PlayerMoney)
            {
                CurrentBet += amount;
                PlayerMoney -= amount;
            }
        }
        public void RemoveBet(int amount = 1)
        {
            if (amount > 0 && amount <= CurrentBet)
            {
                CurrentBet -= amount;
                PlayerMoney += amount;
            }
        }

        public void DealInitialCards()
        {
            if (CurrentBet > 0 && PlayerCards.Count == 0 && DealerCards.Count == 0)
            {
                DrawCard();
                System.Diagnostics.Debug.WriteLine($"Burn card");

                PlayerCards.Add(DrawCard());
                System.Diagnostics.Debug.WriteLine($"Draw card for player");

                DealerCards.Add(DrawCard());
                System.Diagnostics.Debug.WriteLine($"Draw card for dealer");

                PlayerCards.Add(DrawCard());
                System.Diagnostics.Debug.WriteLine($"Draw card for Player");

                DealerCards.Add(DrawCard(isFaceDown: true));
                System.Diagnostics.Debug.WriteLine($"Draw card for dealer");

                OnPropertyChanged(nameof(HasGameNotStarted));
                OnPropertyChanged(nameof(IsGameRunning));
            }
        }

        public async Task DealerTurn()
        {
            if (CurrentBet > 0 && PlayerCards.Count > 0 && DealerCards.Count > 0)
            {
                // Retourner la carte face cachée du croupier
                if (DealerCards[1].IsFaceDown)
                {
                    DealerCards[1].IsFaceDown = false;
                    OnPropertyChanged(nameof(DealerCards));
                }

                // Le croupier tire des cartes jusqu'à atteindre au moins 17
                int dealerValue = CalculateHandValue(DealerCards);
                while (dealerValue < CalculateHandValue(PlayerCards) && dealerValue < 17)
                {
                    await Task.Delay(1000); // Attendre 1 seconde

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        DealCardToDealer();
                    });
                    dealerValue = CalculateHandValue(DealerCards);
                }
            }
        }

        public void DealCardToPlayer()
        {
            if (CurrentBet > 0 && PlayerCards.Count > 0 && DealerCards.Count > 0)
            {
                PlayerCards.Add(DrawCard());
                System.Diagnostics.Debug.WriteLine($"Draw card for player");

                if(CalculateHandValue(PlayerCards) > 21)
                {
                    CheckWinner();
                }
            }
        }

        public void DealCardToDealer()
        {
            if (CurrentBet > 0 && PlayerCards.Count > 0 && DealerCards.Count > 0)
            {
                DealerCards.Add(DrawCard());
                System.Diagnostics.Debug.WriteLine($"Draw card for dealer");
            }
        }

        public void CheckBlackjack()
        {
            if (CalculateHandValue(PlayerCards) == 21)
            {
                CheckWinner();
            }
        }

        public async Task CheckWinner()
        {
            int playerValue = CalculateHandValue(PlayerCards);
            int dealerValue = CalculateHandValue(DealerCards);
            if (playerValue == 21)
            {
                Wins++;
                PlayerMoney += (int)(CurrentBet * 2.5); // Paiement 3:2 pour un blackjack
                StatusMessage = "Blackjack! You win!";
            }
            else if (playerValue > 21)
            {
                // Le joueur a dépassé 21
                Losses++;
                StatusMessage = "You lose!";
            }
            else if (dealerValue > 21 || playerValue > dealerValue)
            {
                // Le croupier a dépassé 21 ou le joueur a une meilleure main
                Wins++;
                PlayerMoney += CurrentBet * 2; // Paiement 1:1
                StatusMessage = "You win!";
            }
            else if (playerValue < dealerValue)
            {
                // Le croupier a une meilleure main
                Losses++;
                StatusMessage = "You lose!";
            }
            else
            {
                // Égalité, le joueur ne perd pas sa mise
                PlayerMoney += CurrentBet;
                StatusMessage = "It's a tie!";
            }

            if (CurrentBet <= PlayerMoney)
            {
                PlayerMoney -= CurrentBet;
            }
            else
            {
                CurrentBet = 0;
            }

            IsGameFinished = true;
            OnPropertyChanged(nameof(HasGameNotStarted));
            OnPropertyChanged(nameof(IsGameRunning));
        }
        #endregion

        public bool HasGameNotStarted => CurrentBet == 0 || (PlayerCards.Count == 0 && DealerCards.Count == 0) || IsGameFinished;
        public bool IsGameRunning => !HasGameNotStarted && !IsGameFinished;
        
        private bool _isGameFinished = false;
        public bool IsGameFinished
        {
            get
            {
                return _isGameFinished;
            }
            private set
            {
                _isGameFinished = value;
                OnPropertyChanged(nameof(IsGameFinished));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

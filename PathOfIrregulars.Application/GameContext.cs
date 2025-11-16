using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.Enums;

namespace PathOfIrregulars.Application
{
    public class GameContext
    {

        public List<string> Logs { get; private set; } = new();
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }

        public Player ActivePlayer { get; private set; }
        public Player Opponent => ActivePlayer == PlayerOne ? PlayerTwo : PlayerOne;

        public int TurnCount { get; private set; } = 1;

        public Card? TargetCard { get; set; }

        public bool HasGameEnded { get; private set; } = false;
        public Player? Winner { get; private set; }

        public FloorTest floorTest { get; set; } 

        public void StartGame(Player p1, Player p2)
        {
            PlayerOne = p1;
            PlayerTwo = p2;
            ActivePlayer = p1;
        }

        public void EndTurn()
        {
            ActivePlayer = Opponent;
            TurnCount++;
        }

        public void EndRound()
        {
            PlayerOne.hasPassed = true;
            PlayerTwo.hasPassed = true;

           
        }

        public void EndGame(Player winner)
        {
            Winner = winner;
            HasGameEnded = true;
        }


        public static List<Card> GetAdjacentCards(Card card, Lane lane) {

            var cards = lane.CardsInLane;
            var index = cards.IndexOf(card);
            var adjacentCards = new List<Card>();

            if ( index == -1)
            {
                return cards;
            }

            if (index > 0)
            {
                adjacentCards.Add(cards[index - 1]);
            }
            if (index < cards.Count - 1)
            {
                adjacentCards.Add(cards[index + 1]);
            }
            return cards;

        }

        public Card DrawCard(Player player)
        {
            if (player.Deck.Count == 0)
            {
                Log("Deck is empty. Cannot draw a card.");
               
                return null;
            }
            var drawnCard = player.Deck[0];
            player.Deck.RemoveAt(0);
            player.Hand.Add(drawnCard);
            Log($"{drawnCard.Name} drawn to hand.");
            return drawnCard;
        }


     

        //targetmultiplecards?
        public void Log(string message)
        {
            Logs.Add(message);
            Console.WriteLine(message);
        }
    }
}

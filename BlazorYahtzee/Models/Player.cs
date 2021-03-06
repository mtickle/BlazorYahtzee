﻿using System.Collections.Generic;
using System.Linq;
using BlazorYahtzee.Models.Categories;
using BlazorYahtzee.Models.Columns;
using BlazorYahtzee.Models.Modes;

namespace BlazorYahtzee.Models
{
    public class Player
    {
        public Dice Dice { get; } = new Dice();

        private readonly IList<Plays> _plays = new List<Plays>();

        public int TotalRollsAllowed { get; }

        public int RollsRemaining { get; private set; }

        public bool HasForcedPlay { get; private set; }

        public ColumnType? ForcedPlayColumnType { get; private set; }

        public ICategory DeclaredCategory { get; private set; }

        public Player(IMode mode)
        {
            foreach (var columnType in mode.Columns.Select(x => x.Type))
            {
                _plays.Add(new Plays(columnType, mode.NumberOfTurns / mode.Columns.Count()));
            }

            TotalRollsAllowed = mode.NumberOfRolls;
            RollsRemaining = mode.NumberOfRolls;
        }

        public Plays Plays(ColumnType type) => _plays.Single(x => x.Type == type);

        public void RollDice()
        {
            RollsRemaining--;

            foreach (var die in Dice.NotHeldCollection)
            {
                die.Roll();
            }
        }

        public void HoldAllDice()
        {
            foreach (var die in Dice.NotHeldCollection)
            {
                die.Hold();
            }
        }

        public bool IsFirstRoll()
        {
            return TotalRollsAllowed - RollsRemaining == 1;
        }

        public bool IsStartOfTurn()
        {
            return RollsRemaining == TotalRollsAllowed;
        }

        public bool IsEndOfTurn()
        {
            return RollsRemaining == 0;
        }

        public void DeclareCategory(ICategory category)
        {
            DeclaredCategory = category;
        }

        public void ClearDeclaredCategory()
        {
            DeclaredCategory = null;
        }

        public bool HasDeclaredCategory()
        {
            return DeclaredCategory != null;
        }

        public void ForcePlay(ColumnType columnType)
        {
            HasForcedPlay = true;
            ForcedPlayColumnType = columnType;
        }

        public bool HasForcedDeclaration()
        {
            return _plays.Where(x => x.Type != ColumnType.Announce).All(x => x.IsFilled()) && !HasDeclaredCategory();
        }

        public void ResetTurn()
        {
            Dice.Release();
            HasForcedPlay = false;
            ForcedPlayColumnType = null;
            RollsRemaining = TotalRollsAllowed;
        }

        public void RemoveRemainingRolls()
        {
            RollsRemaining = 0;
        }

        public int TotalScore()
        {
            return _plays.Sum(x => x.TotalScore());
        }
    }
}
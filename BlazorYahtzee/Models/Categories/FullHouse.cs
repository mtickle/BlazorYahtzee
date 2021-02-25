﻿namespace BlazorYahtzee.Models.Categories
{
    public class FullHouse : ICategory
    {
        public string Name { get; } = "Full House";
        public SectionType Section { get; } = SectionType.Lower;

        public bool CanBePlayed(Player player, ColumnType type)
        {
            return type switch
            {
                ColumnType.Down => player.Plays(ColumnType.Down).HasPlay(typeof(FourOfAKind)),
                ColumnType.Up => player.Plays(ColumnType.Up).HasPlay(typeof(SmallStraight)),
                _ => true
            };
        }

        public bool CanBeClaimedInFull(Player player)
        {
            return player.Dice.HasFullHouse();
        }

        public int PointsFor(Player player)
        {
            return player.HasForcedPlay || player.Dice.HasFullHouse() ? 25 : 0;
        }
    }
}
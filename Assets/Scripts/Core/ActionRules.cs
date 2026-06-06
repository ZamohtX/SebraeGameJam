using System.Collections.Generic;
using UnityEngine;

public static class ThiefTargetCalculator
{
    public enum ThiefActionRule
    {
        Adjacent,
        SameRow,
        SameColumn,
        KnightMove,
        Diagonal,
        Cross,
        Area,
        Random
    }

    public static List<Passenger> CalculatePossibleTargets(
        BusGrid busGrid,
        Vector2Int thiefPosition,
        ThiefActionRule rule)
    {
        switch (rule)
        {
            case ThiefActionRule.Adjacent:
                return GetAdjacent(busGrid, thiefPosition);

            case ThiefActionRule.SameRow:
                return GetSameRow(busGrid, thiefPosition);

            case ThiefActionRule.SameColumn:
                return GetSameColumn(busGrid, thiefPosition);

            case ThiefActionRule.KnightMove:
                return GetKnightMoves(busGrid, thiefPosition);

            case ThiefActionRule.Diagonal:
                return GetDiagonals(busGrid, thiefPosition);

            case ThiefActionRule.Cross:
                return GetCross(busGrid, thiefPosition);

            case ThiefActionRule.Area:
                return GetArea(busGrid, thiefPosition);

            case ThiefActionRule.Random:
                return GetAllPassengers(busGrid);

            default:
                return new List<Passenger>();
        }
    }

    private static bool isValid(Passenger passenger)
    {
        return passenger != null &&
            passenger.Status == PassengerStatus.Intact;
    }

    private static void AddIfOccupied(
        BusGrid grid,
        List<Passenger> passengers,
        int x,
        int y)
    {
        Passenger passenger = grid.GetPassengerAt(x, y);

        if (isValid(passenger))
        {
            passengers.Add(passenger);
        }
    }

    private static List<Passenger> GetAdjacent(
        BusGrid grid,
        Vector2Int pos)
    {
        List<Passenger> result = new();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                AddIfOccupied(
                    grid,
                    result,
                    pos.x + dx,
                    pos.y + dy);
            }
        }

        return result;
    }

    private static List<Passenger> GetSameRow(
        BusGrid grid,
        Vector2Int pos)
    {
        List<Passenger> result = new();

        for (int x = 0; x < grid.Width; x++)
        {
            if (x == pos.x)
                continue;

            AddIfOccupied(grid, result, x, pos.y);
        }

        return result;
    }

    private static List<Passenger> GetSameColumn(
        BusGrid grid,
        Vector2Int pos)
    {
        List<Passenger> result = new();

        for (int y = 0; y < grid.Height; y++)
        {
            if (y == pos.y)
                continue;

            AddIfOccupied(grid, result, pos.x, y);
        }

        return result;
    }

    private static List<Passenger> GetKnightMoves(
        BusGrid grid,
        Vector2Int pos)
    {
        List<Passenger> result = new();

        int[,] offsets =
        {
            { 2, 1 }, { 2,-1 },
            {-2, 1 }, {-2,-1 },
            { 1, 2 }, { 1,-2 },
            {-1, 2 }, {-1,-2 }
        };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            AddIfOccupied(
                grid,
                result,
                pos.x + offsets[i, 0],
                pos.y + offsets[i, 1]);
        }

        return result;
    }

    private static List<Passenger> GetDiagonals(
        BusGrid grid,
        Vector2Int pos)
    {
        List<Passenger> result = new();

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (x == pos.x && y == pos.y)
                    continue;

                if (Mathf.Abs(x - pos.x) ==
                    Mathf.Abs(y - pos.y))
                {
                    AddIfOccupied(grid, result, x, y);
                }
            }
        }

        return result;
    }

    private static List<Passenger> GetCross(
        BusGrid grid,
        Vector2Int pos)
    {
        List<Passenger> result = new();

        result.AddRange(GetSameRow(grid, pos));
        result.AddRange(GetSameColumn(grid, pos));

        return result;
    }

 private static List<Passenger> GetArea(
        BusGrid grid,
        Vector2Int pos,
        int width = 3,
        int height = 3)
    {
        List<Passenger> result = new();

        // Faz uma varredura de -1 a +1 ao redor da posição do ladrão (Quadrado 3x3)
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue; // Pula o próprio ladrão

                AddIfOccupied(
                    grid,
                    result,
                    pos.x + dx,
                    pos.y + dy); 
            }
        }

        return result;
    }

    private static List<Passenger> GetAllPassengers(BusGrid grid)
    {
        List<Passenger> result = new();

        foreach (Passenger passenger in grid.GetAllPassengers())
        {
            if (isValid(passenger))
            {
                result.Add(passenger);
            }
        }

        return result;
    }
}
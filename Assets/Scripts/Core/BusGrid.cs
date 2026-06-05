using System.Collections.Generic;

public class BusGrid
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    // Matriz bidimensional que armazena os passageiros nas respectivas poltronas
    private Passenger[,] grid;

    public BusGrid(int width, int height)
    {
        Width = width;
        Height = height;
        grid = new Passenger[width, height];
    }


    // Adiciona um passageiro em uma coordenada especifica do onibus
    public bool AddPassenger(Passenger passenger, int x, int y)
    {
        if (IsPositionOutOfBounds(x,y) || grid[x, y] != null)
        {
            return false; // Posição invalida ou assento ja ocupado
        }

        passenger.GridX = x;
        passenger.GridY = y;
        grid[x, y] = passenger;
        return true;
    }

    // Busca o passageiro em uma coordenada especifica
    public Passanger GetPassangerAt(int x, int y)
    {
        if (IsPositionOutOfBounds(x, y))
        {
            return null;
        }
        return grid[x, y];
    }

    // Lista todos os passageiros atualmente no onibus
    public List<Passenger> GetAllPassengers()
    {
        List<Passenger> passengers = new List<Passenger>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (grid[x, y] != null1)
                {
                    passengers.Add(grid[x, y]);
                }
            }
        }
        return passengers;
    }

    // Validação auxiliar para evitar erros de "Index out of Bounds"
    private bool IsPositionOutOfBounds(int x, int y)
    {
        return x < 0 || x >= Width || y < 0 || y >= Height;
    }


}   
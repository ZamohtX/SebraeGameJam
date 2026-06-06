using System.Collections.Generic;

public class MatchSetupManager
{
    private System.Random random;

    public MatchSetupManager()
    {
        random = new System.Random();
    }

    // Configura a partida garantindo a lotação mínima e elegendo o ladrão
    public void SetupMatch(BusGrid busGrid, int minPassengers = 35)
    {
        //  Mapeia todos os assentos físicos existentes no ônibus
        List<KeyValuePair<int, int>> availableSeats = GetAvailableSeats(busGrid);
        int totalSeats = availableSeats.Count;

        // Garante que o ônibus criado tem tamanho suficiente
        if (totalSeats < minPassengers)
        {
            minPassengers = totalSeats;
        }

        // Define dinamicamente quantos passageiros vão spawnar nesta partida
        int passengersToSpawn = random.Next(minPassengers, totalSeats + 1);

        // Loop de distribuição e criação dos passageiros
        for (int i = 0; i < passengersToSpawn; i++)
        {
            // Escolhe um assento aleatório da lista de assentos vazios
            int seatIndex = random.Next(availableSeats.Count);
            KeyValuePair<int, int> selectedSeat = availableSeats[seatIndex];

            // Remove o assento da lista para que outra pessoa não sente no mesmo lugar
            availableSeats.RemoveAt(seatIndex);

            // Sorteia a variação visual (1 a 4) e a cor da roupa (1 a 6)
            int randomSpriteId = random.Next(1, 5);
            int randomColorId = random.Next(1, 7);

            string passengerId = "Passenger_" + i;
            Passenger passenger = new Passenger(passengerId, selectedSeat.Key, selectedSeat.Value, randomSpriteId, randomColorId);

            // Insere o passageiro na matriz lógica do ônibus
            busGrid.AddPassenger(passenger, selectedSeat.Key, selectedSeat.Value);
        }

        // Elege secretamente o ladrao da partida
        SelectRandomThief(busGrid);
    }

    // Varre a matriz em busca de assentos vazios
    private List<KeyValuePair<int, int>> GetAvailableSeats(BusGrid busGrid)
    {
        List<KeyValuePair<int, int>> seats = new List<KeyValuePair<int, int>>();

        for (int x = 0; x < busGrid.Width; x++)
        {
            for (int y = 0; y < busGrid.Height; y++)
            {
                if (busGrid.GetPassengerAt(x, y) == null)
                {
                    seats.Add(new KeyValuePair<int, int>(x, y));
                }
            }
        }

        return seats;
    }

    // Escolhe um dos passageiros gerados para ser o ladrão
    private void SelectRandomThief(BusGrid busGrid)
    {
        List<Passenger> allPassengers = busGrid.GetAllPassengers();

        if (allPassengers.Count > 0)
        {
            int thiefIndex = random.Next(allPassengers.Count);
            allPassengers[thiefIndex].Status = PassengerStatus.Thief;
        }
    }
}
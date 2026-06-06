using System.Collections.Generic;

public class MatchManager
{
    private System.Random random;

    public MatchManager()
    {
        random = new System.Random();
    }

    public void SetupMatch(BusGrid busGrid, int minPassengers = 35)
    {
        List<KeyValuePair<int, int>> availableSeats = GetAvailableSeats(busGrid);
        int totalSeats = availableSeats.Count;

        if (totalSeats < minPassengers)
        {
            minPassengers = totalSeats; 
        }

        int passengersToSpawn = random.Next(minPassengers, totalSeats + 1);

        for (int i = 0; i < passengersToSpawn; i++)
        {
            int seatIndex = random.Next(availableSeats.Count);
            KeyValuePair<int, int> selectedSeat = availableSeats[seatIndex];
            availableSeats.RemoveAt(seatIndex);

            int randomSpriteId = random.Next(1, 5);
            int randomColorId = random.Next(1, 7);
            
            string passengerId = "Passenger_" + i;
            Passenger passenger = new Passenger(passengerId, selectedSeat.Key, selectedSeat.Value, randomSpriteId, randomColorId);

            busGrid.AddPassenger(passenger, selectedSeat.Key, selectedSeat.Value);
        }

        SelectRandomThief(busGrid);
    }

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

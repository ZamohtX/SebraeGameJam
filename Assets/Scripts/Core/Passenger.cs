public enum PassengerStatus
{
    Intact,    // Inteiro/Não roubado
    Robbed,    // Roubado
    Thief      // É o ladrão da partida
}

public class Passenger
{
    public string Id { get; private set; }
    public int GridX { get; set; }
    public int GridY { get; set; }
    public PassengerStatus Status { get; set; }
    public int SpriteId { get; private set; }
    public int ClothingColorId { get; private set; }

    public Passenger(string id, int x, int y, int spriteId, int colorId)
    {
        Id = id;
        GridX = x;
        GridY = y;
        SpriteId = spriteId;
        ClothingColorId = colorId;
        Status = PassengerStatus.Intact; 
    }
}
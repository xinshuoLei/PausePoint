namespace PausePoint.Models;

public class Checkpoint
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Link { get; set; }
    public TimeSpan Timestamp { get; set; }
    public string UserId { get; set; }

    public Checkpoint()
    {
        
    }
}
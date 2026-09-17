namespace StepanMascot;

public class Program
{
    public static void Main(string[] args)
    {
        Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanMascots"));

        using (var window = new mascotGraphicManager("Stepan Mascot", 300, 300, 120))
        {
            window.Run();
        };
    }
}
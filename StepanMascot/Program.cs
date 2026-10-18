using StepanMascot.GraphicsWindow.Assets;
using StepanMascot.StepanInterComunications;

namespace StepanMascot;

public class Program
{
    public static void Main(string[] args)
    {
        Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanMascots"));

        HttpHostManager httpHostManager = new HttpHostManager();

        Thread serverThread = new Thread(async () =>  { await httpHostManager.Start(); }, 1);

        AssetsManager.Init();
        using (var window = new mascotGraphicManager("Stepan Mascot", 300, 300, 120))
        {
            serverThread.Start();
            window.Run();
        };
    }
}
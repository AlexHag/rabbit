using System.Text;
using Rabbit.Domain.Producers;

namespace Rabbit.ConsoleApp;

public class FileStreamProducer
{
    private readonly StreamProducer _streamProducer;
    public FileStreamProducer(StreamProducer streamProducer)
    {
        _streamProducer = streamProducer;
        Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);
    }

    public async Task StartReading()
    {
        FileStream fsSource = new FileStream("./data.txt", FileMode.Open, FileAccess.Read);
        byte[] bytes = new byte[fsSource.Length];
        int numBytesToRead = (int) fsSource.Length;
        int numBytesRead = 0;
        
        int itteration = 0;
        while (numBytesToRead > 0)
        {
            itteration++;
            int n = fsSource.Read(bytes, numBytesRead, 256);
            if (n == 0)
            {
                break;
            }
            
            var newBytes = bytes.Skip(numBytesRead).Take(2048).ToArray();
            await ProcessBytes(newBytes);

            numBytesRead += n;
            numBytesToRead -= n;
            if (itteration == 50)
            {
                await StopStream();
                break;
            }
        }
    }

    public void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine("Ctrl+C pressed. Running cleanup...");

        StopStream().GetAwaiter().GetResult();

        e.Cancel = false; 
    }

    public async Task StopStream()
    {
        await _streamProducer.Send(Encoding.UTF8.GetBytes("STEAM_ENDED"));
        _streamProducer.Dispose();
    }

    public async Task ProcessBytes(byte[] bytes)
    {
        Console.WriteLine($"Processing {bytes.Length} bytes");
        Console.WriteLine(Encoding.UTF8.GetString(bytes));
        await Task.Delay(100);
        await _streamProducer.Send(bytes);
    }
}


using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.ML;
using Microsoft.ML.Data;

public class RealTimePrediction
{
    private static Timer _timer;
    private static MLContext _mlContext;
    private static PredictionEngine<ModelInput, ModelOutput> _predictionEngine;

    public static async Task Main(string[] args)
    {
        // Load the pre-trained ML.NET model
        _mlContext = new MLContext();
        DataViewSchema modelSchema;
        ITransformer mlModel = _mlContext.Model.Load("model.zip", out modelSchema);
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

        // Set up a timer to simulate real-time streaming data every second
        _timer = new Timer(1000);
        _timer.Elapsed += async (sender, e) => await OnTimedEvent();
        _timer.AutoReset = true;
        _timer.Enabled = true;

        Console.WriteLine("Real-time predictions started. Press Enter to stop.");
        Console.ReadLine();
        _timer.Stop();
    }

    private static async Task OnTimedEvent()
    {
        // Simulate incoming data (replace with actual streaming data source)
        ModelInput input = new ModelInput
        {
            Feature1 = GetRandomValue(),
            Feature2 = GetRandomValue(),
            Feature3 = GetRandomValue()
        };

        ModelOutput prediction = _predictionEngine.Predict(input);

        // Display prediction in a user-friendly format
        Console.WriteLine($"Prediction at {DateTime.Now}: {prediction.PredictedLabel} - Score: {prediction.Score}");
    }

    private static float GetRandomValue()
    {
        Random random = new Random();
        return (float)random.NextDouble();
    }
}

// Define input and output classes matching the ML.NET model
public class ModelInput
{
    public float Feature1 { get; set; }
    public float Feature2 { get; set; }
    public float Feature3 { get; set; }
}

public class ModelOutput
{
    public bool PredictedLabel { get; set; }
    public float Score { get; set; }
}

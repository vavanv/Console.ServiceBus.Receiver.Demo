using Azure.Messaging.ServiceBus;

const string serviceBusConnectionString = "";
const string queueName = "az-vvv-queue-1";



async Task MessageHandler(ProcessMessageEventArgs processMessageEventArgs)
{
    string body = processMessageEventArgs.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");
    await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
}

Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
{
    Console.WriteLine($"Error handler: {processErrorEventArgs.Exception}");
    return Task.CompletedTask;
}


ServiceBusClient client = new ServiceBusClient(serviceBusConnectionString);

ServiceBusProcessor processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());


try
{
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;
    await processor.StartProcessingAsync();

    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();

    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
catch (Exception ex)
{
    Console.WriteLine($"Receive batch error {ex.Message}");
}
finally
{
    await processor.DisposeAsync();
    await client.DisposeAsync();
}   
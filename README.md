# HopeRF.NET
C# .NET/mono client for RFM95 / RFM98 / SX1276 / SX1278 LORA modules for RaspberryPi and dragino shield

## Out of the box with RX/TX with Dragino GPS/Lora shield without resoldering!
![dragino](http://wiki.dragino.com/images/d/d6/Lora_GPS_HAT.png)

## Simple transmitter
```cs
TrancieverConnectionFactory trancieverConnectionFactory = new TrancieverConnectionFactory();
ITransceiverSpiConnection spiConnection = trancieverConnectionFactory.CreateForDragino();

RFM9XLoraTransceiver rfm9XLoraTransceiver = new RFM9XLoraTransceiver(spiConnection);
rfm9XLoraTransceiver.Initialize();

while (true)
{
    Console.Write("Sending..");

    Task<bool> transmitTask = rfm9XLoraTransceiver.Transmit(Encoding.ASCII.GetBytes("All your base are belogn to us!"));
    transmitTask.Wait();

    Console.WriteLine("OK");

    Thread.Sleep(1000);
}
```

## Simple reciever
```cs
TrancieverConnectionFactory trancieverConnectionFactory = new TrancieverConnectionFactory();
ITransceiverSpiConnection spiConnection = trancieverConnectionFactory.CreateForDragino();

RFM9XLoraTransceiver rfm9XLoraTransceiver = new RFM9XLoraTransceiver(spiConnection);
rfm9XLoraTransceiver.Initialize();

while (true)
{
    Task<RawData> recieveTask = rfm9XLoraTransceiver.Recieve();

    recieveTask.Wait();

    if (recieveTask.Result != null)
    {
        Console.WriteLine(Encoding.ASCII.GetString(recieveTask.Result.Buffer));
    }
}


```

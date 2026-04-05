using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using WeighForce.Extensions;
using WeighForce.Services;
using WeighForce.Data;

namespace WeighForce
{
    public class WeightHub : Hub
    {
        bool sendWeight = false;
        bool sendStatus = false;

        private readonly SyncStatus _status;
        private readonly SerialPortInterface _serialPortInterface;

        public WeightHub(SerialPortInterface serialPortInterface, SyncStatus status)
        {
            _serialPortInterface = serialPortInterface;
            _status = status;
        }

        public ChannelReader<ScaleWeight> StreamWeight()
        {
            if (!_serialPortInterface.isOpen)
                _serialPortInterface.Open();
            return _serialPortInterface
                .StreamWeight()
                .AsChannelReader(10);
        }

        public async Task CloseStream()
        {
            _serialPortInterface.Close();
            await Clients.All.SendAsync("StreamClosed");
        }

        public async IAsyncEnumerable<int> GetWeights([EnumeratorCancellation]
        CancellationToken cancellationToken)
        {
            await Clients.All.SendAsync("Sending Weights");
            for (var i = 0; i < 10; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var rand = new Random();
                yield return rand.Next(50, 100);
                await Task.Delay(1000, cancellationToken);
            }

        }

        public async Task StopWeights()
        {
            sendWeight = false;
            Console.WriteLine("Stopping Weights");
            Console.WriteLine(sendWeight);
            await Clients.All.SendAsync("Stopping Weights");
        }

        public ChannelReader<Status> StreamStatus()
        {
            return _status
                .StreamStatus()
                .AsChannelReader(10);
        }

        public async Task CloseStatusStream()
        {
            await Clients.All.SendAsync("StatusStreamClosed");
        }

        public async IAsyncEnumerable<int> GetStatus([EnumeratorCancellation]
        CancellationToken cancellationToken)
        {
            await Clients.All.SendAsync("Sending status");
            for (var i = 0; i < 10; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var rand = new Random();
                yield return rand.Next(50, 100);
                await Task.Delay(1000, cancellationToken);
            }

        }

        public async Task StopStatus()
        {
            sendStatus = false;
            Console.WriteLine("Stopping status");
            Console.WriteLine(sendStatus);
            await Clients.All.SendAsync("Stopping status");
        }
    }
}
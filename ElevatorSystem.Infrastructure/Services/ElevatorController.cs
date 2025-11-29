using System;
using System.Threading;
using System.Threading.Tasks;
using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;
using ElevatorSystem.Domain.ValueObjects;

namespace ElevatorSystem.Infrastructure.Services
{
    /// <summary>
    /// Thread-safe controller that processes requests and moves the elevator.
    /// </summary>
    public sealed class ElevatorController : IDisposable
    {
        private readonly Elevator _elevator;
        private readonly IRequestQueue _requestQueue;
        private readonly ILogger _logger;
        private readonly ITimeProvider _timeProvider;

        private readonly CancellationTokenSource _cts = new();
        private readonly AutoResetEvent _newRequestEvent = new(false);
        private readonly object _stateLock = new();

        private Task? _workerTask;
        private DateTime _lastMovementAt;

        // Configuration
        private readonly TimeSpan _doorOpenDuration = TimeSpan.FromSeconds(1);
        private readonly TimeSpan _floorTravelDuration = TimeSpan.FromMilliseconds(300);
        private readonly TimeSpan _stuckTimeout = TimeSpan.FromSeconds(10);

        public ElevatorController(
            Elevator elevator,
            IRequestQueue requestQueue,
            ILogger logger,
            ITimeProvider timeProvider)
        {
            _elevator = elevator;
            _requestQueue = requestQueue;
            _logger = logger;
            _timeProvider = timeProvider;

            _lastMovementAt = _timeProvider.UtcNow;
        }

        /// <summary>
        /// Starts the background task that processes elevator requests.
        /// </summary>
        public void Start()
        {
            if (_workerTask != null)
                return;

            _workerTask = Task.Run(() => ProcessRequestsLoopAsync(_cts.Token));
        }

        /// <summary>
        /// Stops the background task.
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
            _newRequestEvent.Set();
            _workerTask?.Wait();
        }

        /// <summary>
        /// Public API: pickup request (floor + direction).
        /// Thread-safe and fast: just enqueues and signals.
        /// </summary>
        public void RequestElevator(int floor, Direction direction)
        {
            var pickup = new Floor(floor);
            var request = new ElevatorRequest(pickup, direction);

            _requestQueue.Enqueue(request);
            _logger.Info($"RequestElevator: {request}");
            _newRequestEvent.Set();
        }

        /// <summary>
        /// Public API: passenger inside elevator selects destination floor.
        /// </summary>
        public void AddDestination(int destinationFloor)
        {
            var dest = new Floor(destinationFloor);
            Direction dir = dest.Value > _elevator.CurrentFloor.Value
                ? Direction.Up
                : Direction.Down;

            var request = new ElevatorRequest(
                pickupFloor: _elevator.CurrentFloor,
                direction: dir,
                destinationFloor: dest);

            _requestQueue.Enqueue(request);
            _logger.Info($"Destination added: {request}");
            _newRequestEvent.Set();
        }

        /// <summary>
        /// Main processing loop: runs on a single background thread.
        /// </summary>
        private async Task ProcessRequestsLoopAsync(CancellationToken token)
        {
            _logger.Info("ElevatorController worker started.");

            while (!token.IsCancellationRequested)
            {
                // If no pending work, wait for a signal or timeout
                if (_requestQueue.Count == 0)
                {
                    DetectStuckElevator();
                    _logger.Debug("No pending requests. Waiting...");
                    _newRequestEvent.WaitOne(TimeSpan.FromSeconds(2));
                    continue;
                }

                if (!_requestQueue.TryDequeue(out var request) || request is null)
                    continue;

                try
                {
                    await HandleRequestAsync(request, token);
                }
                catch (OperationCanceledException)
                {
                    // Graceful shutdown
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error while handling request: {ex}");
                }
            }

            _logger.Info("ElevatorController worker stopped.");
        }

        private async Task HandleRequestAsync(ElevatorRequest request, CancellationToken token)
        {
            _logger.Info($"Handling request: {request}");

            // 1) Move to pickup floor
            await MoveElevatorToFloorAsync(request.PickupFloor, token);

            // 2) Open door for pickup
            await OpenDoorAsync(token);
            await Task.Delay(_doorOpenDuration, token);
            await CloseDoorAsync(token);

            // 3) If destination specified, move there
            if (request.DestinationFloor.HasValue)
            {
                await MoveElevatorToFloorAsync(request.DestinationFloor.Value, token);
                await OpenDoorAsync(token);
                await Task.Delay(_doorOpenDuration, token);
                await CloseDoorAsync(token);
            }
        }

        /// <summary>
        /// Moves elevator step-by-step to the target floor (FIFO scheduling).
        /// </summary>
        private async Task MoveElevatorToFloorAsync(Floor target, CancellationToken token)
        {
            while (_elevator.CurrentFloor.Value != target.Value && !token.IsCancellationRequested)
            {
                lock (_stateLock)
                {
                    if (_elevator.CurrentFloor.Value < target.Value)
                    {
                        _elevator.MoveUp();
                        _logger.Info($"Elevator moved up to floor {_elevator.CurrentFloor.Value}");
                    }
                    else if (_elevator.CurrentFloor.Value > target.Value)
                    {
                        _elevator.MoveDown();
                        _logger.Info($"Elevator moved down to floor {_elevator.CurrentFloor.Value}");
                    }

                    _lastMovementAt = _timeProvider.UtcNow;
                }

                await Task.Delay(_floorTravelDuration, token);
            }

            // Arrived at target floor, set to Idle if not opening door yet
            lock (_stateLock)
            {
                if (_elevator.State is ElevatorState.MovingUp or ElevatorState.MovingDown)
                {
                    _elevator.State = ElevatorState.Idle;
                }
            }

            _logger.Info($"Elevator arrived at floor {target.Value}");
        }

        private async Task OpenDoorAsync(CancellationToken token)
        {
            lock (_stateLock)
            {
                _elevator.OpenDoor();
                _logger.Info($"Door opened at floor {_elevator.CurrentFloor.Value}");
            }
            await Task.CompletedTask;
        }

        private async Task CloseDoorAsync(CancellationToken token)
        {
            lock (_stateLock)
            {
                _elevator.CloseDoor();
                _logger.Info($"Door closed at floor {_elevator.CurrentFloor.Value}");
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Detects a "stuck" elevator: has pending targets or requests but no movement for too long.
        /// </summary>
        private void DetectStuckElevator()
        {
            var now = _timeProvider.UtcNow;
            var sinceLastMove = now - _lastMovementAt;

            if (sinceLastMove > _stuckTimeout)
            {
                _logger.Warn(
                    $"Elevator might be stuck. No movement for {sinceLastMove.TotalSeconds:F1} seconds.");
            }
        }

        public void Dispose()
        {
            Stop();
            _cts.Dispose();
            _newRequestEvent.Dispose();
        }
    }
}

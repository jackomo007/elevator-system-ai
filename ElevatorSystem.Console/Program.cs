using System;
using System.Threading;
using ElevatorSystem.Application.UseCases;
using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.ValueObjects;
using ElevatorSystem.Infrastructure.Logging;
using ElevatorSystem.Infrastructure.Services;
using ElevatorSystem.Infrastructure.Time;

namespace ElevatorSystem.ConsoleApp
{
    internal static class Program
    {
        private static void Main()
        {
            var logger = new ConsoleLogger();
            var timeProvider = new SystemTimeProvider();
            var requestQueue = new FifoRequestQueue();
            var elevator = new Elevator(id: 1, initialFloor: new Floor(1));

            using var controller = new ElevatorController(
                elevator,
                requestQueue,
                logger,
                timeProvider);

            var requestUseCase = new RequestElevatorUseCase(controller, logger);

            controller.Start();

            logger.Info("Elevator simulation started. Type commands:");
            logger.Info("  r <floor> <up|down>   - request elevator");
            logger.Info("  d <floor>             - add destination from inside elevator");
            logger.Info("  q                     - quit");

            while (true)
            {
                // Tell logger that the user is now typing.
                ConsoleLogger.SetUserTyping(true);

                // Optional: print a prompt without logger interference.
                lock (typeof(Console)) // simple prompt lock to avoid rare overlaps
                {
                    System.Console.Write("> ");
                }

                var line = System.Console.ReadLine();

                // User finished typing (or input ended) -> resume logging and flush buffer.
                ConsoleLogger.SetUserTyping(false);

                if (line is null)
                {
                    // EOF or input closed; exit gracefully.
                    break;
                }

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    continue;

                if (parts[0].Equals("q", StringComparison.OrdinalIgnoreCase))
                    break;

                try
                {
                    switch (parts[0])
                    {
                        case "r":
                            if (parts.Length != 3)
                            {
                                logger.Warn("Usage: r <floor> <up|down>");
                                break;
                            }

                            if (!int.TryParse(parts[1], out var floor))
                            {
                                logger.Warn("Invalid floor value.");
                                break;
                            }

                            var direction = parts[2].Equals("up", StringComparison.OrdinalIgnoreCase)
                                ? Direction.Up
                                : Direction.Down;

                            requestUseCase.Execute(floor, direction);
                            break;

                        case "d":
                            if (parts.Length != 2)
                            {
                                logger.Warn("Usage: d <floor>");
                                break;
                            }

                            if (!int.TryParse(parts[1], out var dest))
                            {
                                logger.Warn("Invalid floor value.");
                                break;
                            }

                            controller.AddDestination(dest);
                            break;

                        default:
                            logger.Warn("Unknown command.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error($"Error: {ex.Message}");
                }
            }

            logger.Info("Shutting down...");
            controller.Stop();
            Thread.Sleep(500);
        }
    }
}

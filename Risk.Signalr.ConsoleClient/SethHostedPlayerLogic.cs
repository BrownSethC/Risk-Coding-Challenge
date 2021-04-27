using System.Collections.Generic;
using System.Linq;
using Risk.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Risk.Signalr.ConsoleClient
{
    public class SethHostedPlayerLogic : HostedPlayerLogic
    {
        public SethHostedPlayerLogic(IConfiguration configuration, IHostApplicationLifetime appLifetime) : base(configuration, appLifetime)
        {

        }

        public override string MyPlayerName { get; set; } = "Seth";

        public override Location WhereDoYouWantToDeploy(IEnumerable<BoardTerritory> board)
        {
            var available = false;
            var spotRow = 0;
            var spotCol = 0;
            var attempts = 0;

            while (!available && attempts < 10)
            {
                var width = board.Max(t => t.Location.Column);
                Random random = new Random();
                var tryLocation = random.Next(board.Count());
                spotRow = tryLocation / width;
                spotCol = tryLocation % width;
                var cell = board.First(t => t.Location.Row == spotRow && t.Location.Column == spotCol);
                Console.WriteLine("Trying to fill ({0}, {1})", spotRow, spotCol);

                if (cell.OwnerName == MyPlayerName || string.IsNullOrEmpty(cell.OwnerName))
                {
                    available = true;
                }
                else
                {
                    Console.WriteLine("Tried to fill ({0}, {1}) but failed", spotRow, spotCol);
                }
                attempts++;
            }
            
            if (!available)
            {
                var useCell = board.First(t => t.OwnerName == MyPlayerName);
                spotRow = useCell.Location.Row;
                spotCol = useCell.Location.Column;
            }
            return new Location(spotRow, spotCol);
        }

        public override (Location from, Location to) WhereDoYouWantToAttack(IEnumerable<BoardTerritory> board)
        {
            var myTerritories = board.Where(t => t.OwnerName == MyPlayerName && t.Armies > 1);
            foreach (var t in myTerritories)
            {
                var neighbors = GetNeighbors(t, board);
                var attackableNeighbor = neighbors.FirstOrDefault(t => t.OwnerName != MyPlayerName);

                if (attackableNeighbor != null)
                {
                    return (t.Location, attackableNeighbor.Location);
                }
            }
            throw new System.Exception("Unable to find somewhere to attack.");
        }
    }
}
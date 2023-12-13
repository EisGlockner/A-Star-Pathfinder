using System;
using System.Collections.Generic;
using System.Linq;

namespace AStar
{
    class Program
    {
        private static void Main()
        {
            List<string> map = new List<string>
            {
                "A      |     ",
                "--| |--- -- -",
                "         -| |",
                "   |----| |  ",
                "   |       | ",
                "   |  |    | ",
                "   | -|B|--| ",
                "   |   -     ",
                "---|     | | "
            };

            // Starting Tile
            var start = new Tile();
            start.Y = map.FindIndex(y => y.Contains("A"));
            start.X = map[start.Y].IndexOf("A", StringComparison.Ordinal);
            
            // Finish Tile
            var finish = new Tile();
            finish.Y = map.FindIndex(y => y.Contains("B"));
            finish.X = map[finish.Y].IndexOf("B", StringComparison.Ordinal);
            
            // Initial distance
            start.SetDistance(finish.X, finish.Y);
            
            var activeTiles = new List<Tile>();
            activeTiles.Add(start);
            var visitedTiles = new List<Tile>();

            while (activeTiles.Any())
            {
                // Order by Descending and taking the last makes the algorithm faster because most Times CostDistance is high at the begin and gets lower 
                var checkTile = activeTiles.OrderByDescending(x => x.CostDistance).Last();
                Console.WriteLine($"{checkTile.X} : {checkTile.Y}");

                // Checks if at exit
                if (checkTile.X == finish.X && checkTile.Y == finish.Y)
                {
                    var tile = checkTile;
                    Console.WriteLine("Retracing Path:");
                    while (true)
                    {
                        Console.WriteLine($"{tile.X} : {tile.Y}");
                        if (map[tile.Y][tile.X] == ' ')
                        {
                            var newMapRow = map[tile.Y].ToCharArray();
                            newMapRow[tile.X] = '*';
                            map[tile.Y] = new string(newMapRow);
                        }

                        tile = tile.Parent;
                        if (tile == null)
                        {
                            Console.WriteLine("Map:");
                            map.ForEach(x => Console.WriteLine(x));
                            return;
                        }
                    }
                }

                visitedTiles.Add(checkTile);
                activeTiles.Remove(checkTile);

                var walkableTiles = GetWalkableTiles(map, checkTile, finish);

                foreach (var walkableTile in walkableTiles)
                {
                    // Checks if Tile was already visited
                    if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    {
                        continue;
                    }

                    if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    {
                        var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                        if (existingTile.CostDistance > checkTile.CostDistance)
                        {
                            activeTiles.Remove(existingTile);
                            activeTiles.Add(walkableTile);
                        }
                    }
                    else
                    {
                        activeTiles.Add(walkableTile);
                    }
                }
            }

            Console.WriteLine("No Path Found!");
        }

        private static List<Tile> GetWalkableTiles(List<string> map, Tile currentTile, Tile targetTile)
        {
            var possibleTiles = new List<Tile>
            {
                new Tile
                {
                    X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1
                },
                new Tile
                {
                    X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1
                },
                new Tile
                {
                    X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1
                },
                new Tile
                {
                    X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1
                },
            };

            possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

            var maxX = map.First().Length - 1;
            var maxY = map.Count - 1;

            return possibleTiles
                .Where(tile => tile.X >= 0 && tile.X <= maxX)
                .Where(tile => tile.Y >= 0 && tile.Y <= maxY)
                .Where(tile => map[tile.Y][tile.X] == ' ' || map[tile.Y][tile.X] == 'B')
                .ToList();
        }
    }

    class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public Tile Parent { get; set; }
        
        // Direct Distance without Walls
        public void SetDistance(int targetX, int targetY)
        {
            Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
        }
    }
}

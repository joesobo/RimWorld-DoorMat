// Test doubles for isolated behavior checks. These do not run the RimWorld engine.
namespace Verse
{
    public enum DestroyMode { Vanish }

    public readonly record struct IntVec3(int X, int Z);

    public class Map
    {
        public readonly ThingGrid thingGrid;
        public readonly HashSet<IntVec3> Blocked = new();
        public int Width = 10;
        public int Height = 10;
        public Map() { thingGrid = new(this); }
    }

    public class ThingGrid
    {
        private readonly Map map;
        private readonly Dictionary<IntVec3, List<Thing>> cells = new();
        public ThingGrid(Map map) { this.map = map; }
        public List<Thing> At(IntVec3 cell)
        {
            if (!cell.InBounds(map)) throw new IndexOutOfRangeException("Thing-grid access outside the map");
            if (!cells.TryGetValue(cell, out var things))
            {
                cells[cell] = things = new();
            }
            return things;
        }

        public IEnumerable<Thing> ThingsAt(IntVec3 cell) => At(cell);
    }

    public class Thing
    {
        public Map Map;
        public IntVec3 Position;
        public bool Destroyed;
        public void Destroy(DestroyMode mode)
        {
            Destroyed = true;
            Map.thingGrid.At(Position).Remove(this);
        }
    }

    public class Building : Thing
    {
        public int TestTick;
        public IntVec3[] TestCells = [];
        protected virtual void Tick() { }
        public virtual void DrawGUIOverlay() { }
        public void RunTestTick(int tick) { TestTick = tick; Tick(); }
    }

    public class Pawn : Thing
    {
        public RimWorld.Pawn_FilthTracker filth;
    }

    public static class GridExtensions
    {
        public static bool IsHashIntervalTick(this Building thing, int interval) => thing.TestTick % interval == 0;
        public static IEnumerable<IntVec3> OccupiedRect(this Building thing) => thing.TestCells;
        public static bool InBounds(this IntVec3 cell, Map map) => cell.X >= 0 && cell.Z >= 0 && cell.X < map.Width && cell.Z < map.Height;
        public static bool Impassable(this IntVec3 cell, Map map) => map.Blocked.Contains(cell);
        public static IntVec3 RandomAdjacentCell8Way(this IntVec3 cell) => new(cell.X - 1, cell.Z - 1);
    }
}

namespace RimWorld
{
    public class Filth : Verse.Thing
    {
        public bool CanDrop = true;
        public bool CanDropAt(Verse.IntVec3 cell, Verse.Map map) => CanDrop;
    }

    public class Pawn_FilthTracker
    {
        private readonly Verse.Pawn pawn;
        public List<Filth> CarriedFilthListForReading { get; } = new();
        public Pawn_FilthTracker(Verse.Pawn pawn) { this.pawn = pawn; }

        private void DropCarriedFilth(Filth filth)
        {
            CarriedFilthListForReading.Remove(filth);
            filth.Map = pawn.Map;
            filth.Position = pawn.Position;
            // Deliberately mutate the same list that enumerating pawns reads.
            pawn.Map.thingGrid.At(pawn.Position).Add(filth);
        }
    }
}

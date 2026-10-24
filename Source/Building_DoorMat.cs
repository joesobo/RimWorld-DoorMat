// Unofficial vibecoded RimWorld 1.7 port, 2026-10-24.
// Based on Dracoix's Door Mat, originally by Latta. DropCarriedFilth support by ignis.
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace LT
{
    public class Building_DoorMat : Building
    {
        private static readonly MethodInfo DropCarriedFilth = typeof(Pawn_FilthTracker)
            .GetMethod("DropCarriedFilth", BindingFlags.Instance | BindingFlags.NonPublic);

        protected override void Tick()
        {
            base.Tick();

            if (Map == null || !this.IsHashIntervalTick(10))
            {
                return;
            }

            foreach (var cell in this.OccupiedRect())
            {
                if (!cell.InBounds(Map) || cell.Impassable(Map))
                {
                    continue;
                }

                if (this.IsHashIntervalTick(2000))
                {
                    CleanFirstFilthAt(cell.RandomAdjacentCell8Way());
                }

                if (this.IsHashIntervalTick(3000))
                {
                    CleanFirstFilthAt(cell);
                }

                HavePawnsDropFilth(cell);
            }
        }

        private void CleanFirstFilthAt(IntVec3 cell)
        {
            if (cell.InBounds(Map))
            {
                Map.thingGrid.ThingsAt(cell).OfType<Filth>()
                    .FirstOrDefault()?.Destroy(DestroyMode.Vanish);
            }
        }

        private void HavePawnsDropFilth(IntVec3 cell)
        {
            // Dropping filth can change this cell's thing list during iteration.
            var pawns = Map.thingGrid.ThingsAt(cell).OfType<Pawn>().ToArray();
            foreach (var pawn in pawns)
            {
                var carriedFilth = pawn.filth?.CarriedFilthListForReading;
                if (carriedFilth == null)
                {
                    continue;
                }

                for (int index = carriedFilth.Count - 1; index >= 0; index--)
                {
                    var filth = carriedFilth[index];
                    if (filth.CanDropAt(pawn.Position, pawn.Map))
                    {
                        DropCarriedFilth.Invoke(pawn.filth, new object[] { filth });
                    }
                }
            }
        }

        public override void DrawGUIOverlay()
        {
        }
    }
}

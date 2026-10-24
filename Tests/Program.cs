using LT;
using RimWorld;
using Verse;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.Json;

CheckGameReferenceContract();

var cases = new (string Name, Action Run)[]
{
    ("Filth drops on the ten-tick check", () =>
    {
        var mat = Mat(new IntVec3(3, 3));
        var pawn = DirtyPawn(mat.Map, mat.Position);
        mat.RunTestTick(9);
        Check(pawn.filth.CarriedFilthListForReading.Count == 2, "Dropped filth before the interval");
        mat.RunTestTick(10);
        Check(pawn.filth.CarriedFilthListForReading.Count == 0, "Carried filth remains");
        Check(CountFilth(mat.Map, mat.Position) == 2, "Filth was not deposited");
    }),
    ("Undroppable filth stays on the pawn", () =>
    {
        var mat = Mat(new IntVec3(3, 3));
        var pawn = DirtyPawn(mat.Map, mat.Position);
        foreach (var filth in pawn.filth.CarriedFilthListForReading) filth.CanDrop = false;
        mat.RunTestTick(10);
        Check(pawn.filth.CarriedFilthListForReading.Count == 2, "Undroppable filth was erased");
        Check(CountFilth(mat.Map, mat.Position) == 0, "Undroppable filth was deposited");
    }),
    ("Several pawns survive cell-list mutation", () =>
    {
        var mat = Mat(new IntVec3(3, 3));
        var first = DirtyPawn(mat.Map, mat.Position);
        var second = DirtyPawn(mat.Map, mat.Position);
        mat.RunTestTick(10);
        Check(first.filth.CarriedFilthListForReading.Count == 0 && second.filth.CarriedFilthListForReading.Count == 0,
            "A pawn was skipped after another pawn dropped filth");
        Check(CountFilth(mat.Map, mat.Position) == 4, "A deposit was lost");
    }),
    ("Large mats process both occupied cells", () =>
    {
        var mat = Mat(new IntVec3(3, 3), new IntVec3(4, 3));
        var first = DirtyPawn(mat.Map, mat.TestCells[0]);
        var second = DirtyPawn(mat.Map, mat.TestCells[1]);
        mat.RunTestTick(10);
        Check(first.filth.CarriedFilthListForReading.Count == 0 && second.filth.CarriedFilthListForReading.Count == 0,
            "An occupied cell was missed");
    }),
    ("Impassable cells are skipped", () =>
    {
        var mat = Mat(new IntVec3(3, 3));
        var pawn = DirtyPawn(mat.Map, mat.Position);
        mat.Map.Blocked.Add(mat.Position);
        mat.RunTestTick(10);
        Check(pawn.filth.CarriedFilthListForReading.Count == 2, "An impassable cell was processed");
    }),
    ("Self-cleaning removes one filth object at 3000 ticks", () =>
    {
        var mat = Mat(new IntVec3(3, 3));
        GroundFilth(mat.Map, mat.Position, 2);
        mat.RunTestTick(2990);
        Check(CountFilth(mat.Map, mat.Position) == 2, "Self-cleaned early");
        mat.RunTestTick(3000);
        Check(CountFilth(mat.Map, mat.Position) == 1, "Incorrect self-cleaning amount");
    }),
    ("Nearby cleaning removes one filth object at 2000 ticks", () =>
    {
        var mat = Mat(new IntVec3(3, 3));
        var neighbor = new IntVec3(2, 2);
        GroundFilth(mat.Map, neighbor, 2);
        GroundFilth(mat.Map, mat.Position, 2);
        mat.RunTestTick(1990);
        Check(CountFilth(mat.Map, neighbor) == 2, "Cleaned a neighbor early");
        mat.RunTestTick(2000);
        Check(CountFilth(mat.Map, neighbor) == 1, "Incorrect nearby cleaning amount");
        Check(CountFilth(mat.Map, mat.Position) == 2, "Self-cleaned at the wrong interval");
    }),
    ("A map-edge mat skips an out-of-bounds neighbor", () =>
    {
        var mat = Mat(new IntVec3(0, 0));
        mat.RunTestTick(2000);
        Check(CountFilth(mat.Map, mat.Position) == 0, "Unexpected filth at the edge");
    }),
    ("Unspawned mats and pawns without trackers are safe", () =>
    {
        var mat = Mat(new IntVec3(3, 3));
        mat.Map.thingGrid.At(mat.Position).Add(new Pawn { Map = mat.Map, Position = mat.Position });
        mat.RunTestTick(10);
        mat.Map = null;
        mat.RunTestTick(20);
    })
};

foreach (var (name, run) in cases)
{
    run();
    Console.WriteLine($"PASS: {name}");
}
Console.WriteLine($"{cases.Length} isolated behavior checks passed. In-game testing remains pending.");

static Building_DoorMat Mat(params IntVec3[] cells) => new() { Map = new(), Position = cells[0], TestCells = cells };
static Pawn DirtyPawn(Map map, IntVec3 position)
{
    var pawn = new Pawn { Map = map, Position = position };
    pawn.filth = new(pawn);
    pawn.filth.CarriedFilthListForReading.AddRange([new Filth(), new Filth()]);
    map.thingGrid.At(position).Add(pawn);
    return pawn;
}
static void GroundFilth(Map map, IntVec3 position, int count)
{
    for (int index = 0; index < count; index++) map.thingGrid.At(position).Add(new Filth { Map = map, Position = position });
}
static int CountFilth(Map map, IntVec3 position) => map.thingGrid.At(position).OfType<Filth>().Count();
static void Check(bool passed, string message)
{
    if (!passed) throw new Exception(message);
}

static void CheckGameReferenceContract()
{
    using var assets = JsonDocument.Parse(File.ReadAllText("Source/obj/project.assets.json"));
    var assembly = assets.RootElement.GetProperty("packageFolders").EnumerateObject()
        .Select(folder => Path.Combine(folder.Name, "krafs.rimworld.ref", "1.7.4985-unstable", "ref", "net10.0", "Assembly-CSharp.dll"))
        .First(File.Exists);
    using var file = File.OpenRead(assembly);
    using var pe = new PEReader(file);
    var metadata = pe.GetMetadataReader();
    var tracker = metadata.TypeDefinitions.Select(metadata.GetTypeDefinition)
        .Single(type => metadata.GetString(type.Namespace) == "RimWorld" && metadata.GetString(type.Name) == "Pawn_FilthTracker");
    var drop = tracker.GetMethods().Select(metadata.GetMethodDefinition)
        .Single(method => metadata.GetString(method.Name) == "DropCarriedFilth");
    Check((drop.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private,
        "The reflected drop method is no longer private");
    Check((drop.Attributes & MethodAttributes.Static) == 0, "The reflected drop method is no longer an instance method");
    var signature = metadata.GetBlobReader(drop.Signature);
    signature.ReadSignatureHeader();
    Check(signature.ReadCompressedInteger() == 1, "The reflected drop method has a different parameter count");
    Check(signature.ReadSignatureTypeCode() == SignatureTypeCode.Void, "The reflected drop method has a different return type");
    Check(signature.ReadSignatureTypeCode() == SignatureTypeCode.TypeHandle, "The drop parameter has a different signature");
    var parameter = metadata.GetTypeDefinition((TypeDefinitionHandle)signature.ReadTypeHandle());
    Check(metadata.GetString(parameter.Name) == "Filth" && metadata.GetString(parameter.Namespace) == "RimWorld",
        "The reflected drop method no longer takes RimWorld.Filth");
    Console.WriteLine("PASS: Reflected drop-method signature matches the actual RimWorld 1.7 reference assembly");
}

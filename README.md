# Door Mat: Vibecoded RimWorld 1.7 Port

**Unofficial, vibecoded port made by joesobo with OpenAI Codex. In-game testing is pending.**

This updates **Dracoix's Door Mat**, originally by Latta, for RimWorld **1.7.4985-unstable**. The code compiles against the 1.7 reference assemblies with .NET 10. The game needs to be tested on a separate computer.

## Download and install

Download **[DoorMat-1.7.zip](https://github.com/joesobo/RimWorld-DoorMat/releases/download/v1.7.0-test.1/DoorMat-1.7.zip)** from the [test release](https://github.com/joesobo/RimWorld-DoorMat/releases/tag/v1.7.0-test.1).

1. Extract the `DoorMat-1.7` folder into your RimWorld installation's `Mods` folder.
2. Check that `Mods/DoorMat-1.7/About/About.xml` exists. Avoid an extra nested folder.
3. Disable the original Workshop **Door Mat** and enable **Door Mat (1.7 Vibecoded Port)** in the game's mod menu.
4. Restart RimWorld 1.7 and test in a new colony or a copy of your save.

This port uses the original `dracoix.doormat.r12a` package ID and `LT.*` building class and definition names. Existing mats should be recognized, but loading an existing colony still needs an in-game test. Enable one version of Door Mat at a time.

## Changes

- Targets .NET 10 and RimWorld 1.7, with a portable build and pinned game references.
- Keeps Dracoix's filth-dropping rules and the original 10, 2,000, and 3,000 tick intervals.
- Keeps the original mat textures, building definitions, and construction costs.
- Uses the public carried-filth list accessor and the 1.7 tick override visibility.
- Takes a snapshot of pawns before dropping filth, since dropping it changes the cell's thing list.
- Checks map bounds before cleaning an adjacent cell, and skips pawns without a filth tracker.
- Removes older game builds and obsolete Architect Sense translation entries from this 1.7 package.

This is a 1.7-only port. It has no Harmony dependency.

## Build

Requires the .NET 10 SDK. The game does not need to be installed to compile.

```sh
dotnet restore Source/RimWorld-DoorMat.csproj --locked-mode
dotnet build Source/RimWorld-DoorMat.csproj -c Release --no-restore
```

The output is `Assemblies/RimWorld-DoorMat.dll`. The NuGet package provides reference signatures; game binaries are not bundled.

From a repository checkout, run the isolated behavior checks and build a download ZIP:

```sh
dotnet run --project Tests/BehaviorChecks.csproj -c Release
python3 tools/validate.py
python3 tools/package.py
```

The behavior checks use test doubles for the game's map, pawns, and filth tracker. They also inspect the actual 1.7 reference assembly for the private method used by reflection. They cover filth deposits, undroppable filth, multiple pawns, large mats, cleaning intervals, map edges, and missing trackers. They do not establish in-game compatibility. GitHub Actions runs the build and checks on Linux.

## In-game test checklist

- Start with Core and this port; check the log for load, definition, and ticking errors.
- Build small and large mats, including cloth and leather variants. Check their textures and rotation.
- Walk a dirty pawn across a mat. Check that carried filth drops on the mat when it can be deposited.
- Check that a full or blocked mat does not erase filth that cannot be deposited.
- Let the game run long enough to check slow self-cleaning and nearby cleaning.
- Test a mat near the edge of the map and several pawns crossing a large mat.
- Save and reload. Then test a copy of a colony that already contains Dracoix mats.

## Credits and source

- **Latta**: original [LT-DoorMat](https://github.com/alattalatta/RimWorld-DoorMat).
- **Dracoix**: the [Door Mat Workshop mod](https://steamcommunity.com/sharedfiles/filedetails/?id=1505423207), maintained through 1.6.
- **ignis**: filth-dropping support credited in Dracoix's source.
- **joesobo with Codex**: this vibecoded 1.7 port, 2026-10-24.
- **Krafs**: [RimRef](https://github.com/krafs/RimRef), the build reference package.

The baseline is Dracoix's Workshop release updated on 2025-06-11, downloaded directly from Steam on 2026-10-24. This port is independent of Dracoix and Ludeon Studios.

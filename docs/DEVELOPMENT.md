# Development

This is a vibe-coded compatibility port made by joesobo with OpenAI Codex. Review proposed changes against the original mod's behavior and retain the upstream credits. See [CREDITS.md](../CREDITS.md) and the [license scope](../LICENSE.md) before reusing or redistributing material.

## Requirements

- **.NET 10 SDK** to build and run the behavior checks.
- **Python 3.9 or later** for content validation and ZIP packaging; only the standard library is used.
- **RimWorld 1.7.4985-unstable** for in-game testing. The game is not needed for compilation.

Run the commands below from the repository root. Use `python` in place of `python3` if that is how Python is installed on your system.

## Build

```sh
dotnet restore Source/RimWorld-DoorMat.csproj --locked-mode
dotnet build Source/RimWorld-DoorMat.csproj -c Release --no-restore
```

The compiled mod is written to `Assemblies/RimWorld-DoorMat.dll`. The project pins `Krafs.Rimworld.Ref` to `1.7.4985-unstable` through its lockfile. That package supplies reference signatures; the installable mod does not include RimWorld's game binaries.

## Validate

```sh
dotnet run --project Tests/BehaviorChecks.csproj -c Release
python3 tools/validate.py
```

The nine isolated behavior checks use test doubles for maps, pawns, and filth trackers. A separate check inspects the pinned reference assembly for the private `Pawn_FilthTracker.DropCarriedFilth(Filth)` method called through reflection. These checks help catch regressions; they do not replace testing inside RimWorld.

Content validation checks the metadata, all 14 mat definitions, XML and translation references, texture paths, and the single-assembly layout. GitHub Actions runs the build, behavior checks, content validation, and packaging on Linux. See [testing notes](TESTING.md) for manual coverage.

## Package locally

```sh
python3 tools/package.py
```

This writes `artifacts/DoorMat-1.7.zip`, with one top-level `DoorMat-1.7` folder ready to copy into the game's `Mods` folder. It includes the compiled assembly, corresponding source, documentation, credits, and license notice. Build outputs and intermediate `bin` and `obj` folders are excluded. Packaging verifies ZIP integrity and confirms that the only DLL is the mod itself.

An alternate output path is supported:

```sh
python3 tools/package.py --output artifacts/local-test.zip
```

These commands create local files. The workflow does not publish releases or upload anything to Steam Workshop.

## What changed for 1.7

- Targets .NET 10 and the pinned RimWorld 1.7 reference package.
- Uses the public carried-filth list accessor and the visibility required by the 1.7 tick override.
- Takes a snapshot of pawns before dropping filth, because depositing filth changes the cell's thing list.
- Checks map bounds before cleaning an adjacent cell and skips pawns without filth trackers.
- Removes older game builds and obsolete Architect Sense translation entries from the 1.7 package.
- Corrects translation directory casing and the `ThingDef` directory name for consistent loading across platforms.

The existing drop rules remain: only filth that can be deposited is dropped. Mats check for pawns every 10 ticks, attempt nearby cleaning every 2,000 ticks, and clean their occupied cells every 3,000 ticks. The original textures, construction costs, package ID, and building names are preserved.

## Contributing

Small fixes, testing notes, and attribution corrections are welcome. Explain the problem, the change in behavior, and how you verified it. When changing gameplay code, run the existing checks and test in game where possible. Keep the unofficial status and AI assistance disclosure clear, and do not describe untested cases as verified.

The repository is shared as a personal community resource. It has no planned Steam Workshop publication or promised update schedule.

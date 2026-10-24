# Testing notes

## Current evidence

On **October 24, 2026**, joesobo installed the `v1.7.0-test.1` archive on Windows with **RimWorld 1.7.4985 rev1269** and reported that it worked in game.

This records an initial successful user test. It does not establish that every variant, cleaning interval, save migration, or combination of mods has been checked. The detailed manual cases below remain unverified individually.

The installed archive's SHA-256 was checked before installation:

```text
535e9df2a52591628a4c9bd9518bccd4ca8489bbebaffbf4575367a86a35c02a  DoorMat-1.7.zip
```

The archive and its checksum are available on the [existing test download page](https://github.com/joesobo/RimWorld-DoorMat/releases/tag/v1.7.0-test.1). That page and archive were created before this documentation refresh; use the current repository for testing status and instructions.

## Automated coverage

The repository includes nine isolated checks for:

1. Filth deposits at the 10-tick check.
2. Filth that cannot be deposited remaining on the pawn.
3. Multiple pawns when deposits change the cell's thing list.
4. Both cells of a large mat.
5. Skipping impassable cells.
6. Self-cleaning at 3,000 ticks.
7. Nearby cleaning at 2,000 ticks.
8. An out-of-bounds neighbor at the map edge.
9. Unspawned mats and pawns without filth trackers.

A separate check verifies the reflected method signature in the pinned game reference assembly. XML, definition, texture, translation, and archive validation are also included. See the [development guide](DEVELOPMENT.md) for commands and [GitHub Actions](https://github.com/joesobo/RimWorld-DoorMat/actions/workflows/build.yml) for current results.

These checks use test doubles rather than a running game. Passing them does not prove in-game compatibility.

## Manual checklist

Use a new test colony or a copy of a save. Start with Core and this port, then repeat relevant checks with the mods you normally use.

- [ ] Start the game and inspect the log for assembly, definition, and ticking errors.
- [ ] Build small and large mats, including fabric, leather, colored, and checker variants. Check textures and rotation.
- [ ] Walk a dirty pawn across a mat and confirm carried filth is deposited when possible.
- [ ] Check that a full or blocked mat does not erase filth that cannot be deposited.
- [ ] Run long enough to observe slow self-cleaning and nearby cleaning.
- [ ] Check mats at map edges and several pawns crossing a large mat.
- [ ] Save and reload the test colony.
- [ ] Test a copy of a colony containing mats from Dracoix's version.

## Reporting results

Include the exact game build, operating system, port commit or archive version, relevant mod list, steps tried, and what happened. Identify which checklist items you actually tested. Share relevant error excerpts and remove personal information from logs before posting them in [GitHub Issues](https://github.com/joesobo/RimWorld-DoorMat/issues).

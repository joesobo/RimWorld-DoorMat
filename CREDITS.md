# Credits and provenance

This is an unofficial compatibility port of an existing mod. It preserves the original mod's identity and assets while adapting its code for RimWorld 1.7. The original mod and its gameplay design were not created by joesobo or OpenAI Codex.

## Original work

| Contributor | Contribution | Source |
| --- | --- | --- |
| Latta / alattalatta | Original LT-DoorMat mod | [Original repository](https://github.com/alattalatta/RimWorld-DoorMat) · [Original Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=727309536) |
| Dracoix | Continued Door Mat maintenance and ports through RimWorld 1.6 | [Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=1505423207) |
| ignis | Filth-dropping support, as credited in Dracoix's source | Credit retained in [Building_DoorMat.cs](Source/Building_DoorMat.cs) |
| Upstream artists and translators | Textures, preview artwork, and translations inherited with the mod | Included in `Textures/`, `About/Preview.png`, and `Languages/` |
| Ludeon Studios | RimWorld and its game content | [RimWorld](https://rimworldgame.com/) |

Individual authorship of every inherited asset and translation has not been established. The general credit above does not assign that work to the port maintainer. Attribution corrections are welcome.

## This port

**joesobo** created this unofficial RimWorld 1.7 port with **OpenAI Codex** assistance on October 24, 2026. AI assistance was used for the compatibility changes, validation tooling, and documentation. The port is described as vibe-coded so that this process is visible to anyone using or reviewing it.

**Krafs's [RimRef](https://github.com/krafs/RimRef)** supplies the game reference signatures used during compilation. RimWorld's game binaries are not redistributed in the mod package.

## Baseline and preserved identity

The recorded baseline is Dracoix's Workshop distribution, updated June 11, 2025 and downloaded from Steam on October 24, 2026. The imported baseline is available on the [`upstream-1.6` branch](https://github.com/joesobo/RimWorld-DoorMat/tree/upstream-1.6).

The port preserves:

- The `dracoix.doormat.r12a` package ID and `LT.*` building class and definition names.
- The inherited textures, preview image, translations, construction costs, and mat variants.
- The existing filth-dropping rules and cleaning intervals.

Preserving these names is intended to retain compatibility with existing mats; it does not establish save migration or endorsement by the original authors.

## Reuse and attribution

The project is shared at no charge to help other players and modders. joesobo's original contributions are available under the MIT terms in [LICENSE.md](LICENSE.md), with the scope described there.

No explicit license covering all inherited material was found in the upstream GitHub repository or Workshop description reviewed on October 24, 2026. This repository does not claim to grant new rights over that material. If you redistribute or publish a derivative, check the original authors' terms or seek their permission as needed, retain their credits, and identify your changes.

This project is independent of Latta, Dracoix, Ludeon Studios, and OpenAI. No endorsement is claimed. Credit, permission, or removal concerns can be raised through [GitHub Issues](https://github.com/joesobo/RimWorld-DoorMat/issues).

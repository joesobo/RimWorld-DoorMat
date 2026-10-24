"""Create an installable ZIP with the compiled mod and corresponding source."""
import argparse
from pathlib import Path
import zipfile

root = Path(__file__).resolve().parents[1]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--output", type=Path, default=root / "artifacts/DoorMat-1.7.zip")
args = parser.parse_args()
args.output.parent.mkdir(parents=True, exist_ok=True)
assembly = root / "Assemblies/RimWorld-DoorMat.dll"
if not assembly.is_file():
    parser.error("Build Source/RimWorld-DoorMat.csproj in Release mode first.")

paths = [root / "README.md"]
for folder in ("About", "Assemblies", "Defs", "Languages", "Textures", "Source"):
    paths.extend(path for path in (root / folder).rglob("*")
                 if path.is_file() and not {"bin", "obj"}.intersection(path.relative_to(root).parts))

with zipfile.ZipFile(args.output, "w", compression=zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
    for path in sorted(paths):
        entry = zipfile.ZipInfo("DoorMat-1.7/" + path.relative_to(root).as_posix())
        entry.compress_type = zipfile.ZIP_DEFLATED
        entry.external_attr = 0o100644 << 16
        archive.writestr(entry, path.read_bytes(), compresslevel=9)

with zipfile.ZipFile(args.output) as archive:
    assert archive.testzip() is None
    assert "DoorMat-1.7/About/About.xml" in archive.namelist()
    dlls = [name for name in archive.namelist() if name.endswith(".dll")]
    assert dlls == ["DoorMat-1.7/Assemblies/RimWorld-DoorMat.dll"]
    assert archive.read(dlls[0]) == assembly.read_bytes()
print(args.output.resolve())

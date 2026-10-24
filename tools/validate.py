"""Check the mod's metadata, XML references, textures, and assembly layout."""
from pathlib import Path
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[1]
about = ET.parse(root / "About/About.xml").getroot()
assert about.findtext("packageId") == "dracoix.doormat.r12a"
assert [entry.text for entry in about.find("supportedVersions")] == ["1.7"]
assert "Vibecoded" in about.findtext("name")
assert not (root / "About/PublishedFileId.txt").exists()
assert sorted(path.name for path in (root / "Assemblies").iterdir()) == ["RimWorld-DoorMat.dll"]
assert not (root / "LoadFolders.xml").exists()

defs = ET.parse(root / "Defs/ThingDefs/Buildings_DoorMat.xml").getroot()
names = {node.findtext("defName") for node in defs if node.find("defName") is not None}
mats = [node for node in defs.findall("ThingDef") if node.find("defName") is not None]
assert len(mats) == 14
assert defs.find("ThingDef/thingClass").text == "LT.Building_DoorMat"
for texture in defs.findall(".//texPath"):
    assert (root / "Textures" / f"{texture.text}.png").is_file(), texture.text

xml_files = list(root.rglob("*.xml"))
for path in xml_files:
    if "obj" in path.parts or "bin" in path.parts:
        continue
    tree = ET.parse(path).getroot()
    if "Languages" in path.parts:
        assert path.parent.name == "ThingDef", path
        assert path.parent.parent.name == "DefInjected", path
        for entry in tree:
            target = entry.tag.split(".")[0]
            for suffix in ("_Blueprint_Install", "_Blueprint", "_Frame"):
                target = target.removesuffix(suffix)
            assert target in names, (path, entry.tag)
print("PASS: metadata, 14 mat definitions, texture paths, translations, and single-assembly layout")

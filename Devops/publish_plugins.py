
import os
import xml.etree.ElementTree as ET
import subprocess
import re
import os
import shutil
import zipfile
from datetime import datetime

def find_csproj_files(solution_path):
    csproj_files = []

    # Lire le contenu du fichier .sln
    with open(solution_path, 'r') as file:
        content = file.read()

    # Utiliser une expression régulière pour extraire les chemins des fichiers .csproj
    project_pattern = re.compile(r'Project\(".*?"\) = ".*?", "(.*?\.csproj)"')
    matches = project_pattern.findall(content)

    # Construire les chemins complets pour chaque fichier .csproj
    solution_dir = os.path.dirname(solution_path)
    for match in matches:
        csproj_path = os.path.join(solution_dir, match)
        csproj_path = os.path.normpath(csproj_path)  # Normaliser le chemin
        csproj_files.append(csproj_path)

    return csproj_files

def get_version_from_csproj(csproj_file):
    tree = ET.parse(csproj_file)
    root = tree.getroot()
    version = root.find(".//Version")
    return version.text if version is not None else None

def set_version_in_csproj(csproj_file, version):
    tree = ET.parse(csproj_file)
    root = tree.getroot()
    version_element = root.find(".//Version")

    if version_element is not None:
        version_element.text = version
    else:
        # Si l'élément Version n'existe pas, nous devons le créer
        property_group = root.find(".//PropertyGroup")
        if property_group is not None:
            version_element = ET.Element('Version')
            version_element.text = version
            property_group.append(version_element)
        else:
            # Si PropertyGroup n'existe pas, nous devons le créer
            property_group = ET.Element('PropertyGroup')
            version_element = ET.Element('Version')
            version_element.text = version
            property_group.append(version_element)
            root.append(property_group)

    # Sauvegarder les modifications dans le fichier
    tree.write(csproj_file, encoding='utf-8', xml_declaration=True)

def publish_project(csproj_file, outputFile):
    command = ["dotnet", "publish", csproj_file, "-c", "Release", "-o", outputFile]
    subprocess.run(command, check=True)


def zip_dll_files(tmp_dir, out_dir, projectName, version):
    zip_path = os.path.join(out_dir, version + "_" + projectName + ".vplug")
    with zipfile.ZipFile(zip_path, 'w') as zipf:
        for root, _, files in os.walk(tmp_dir):
            for file in files:
                file_path = os.path.join(root, file)
                zipf.write(file_path, os.path.relpath(file_path, tmp_dir))
        zipf.comment = ("version=" + version).encode('utf-8')

def move_single_dll(tmp_dir, out_dir, version):
    # Déplacer le fichier .dll unique vers le dossier out
    for root, _, files in os.walk(tmp_dir):
        for file in files:
            if file.endswith(".dll"):
                src_path = os.path.join(root, file)
                shutil.move(src_path, out_dir)
                return

def project_is_plugin(csproj_file):
    try:
        tree = ET.parse(csproj_file)
        root = tree.getroot()

        # Vérifier la présence de la balise <Plugin>
        plugin_element = root.find(".//Plugin")
        return plugin_element is not None
    except ET.ParseError:
        # En cas d'erreur de parsing, on considère que ce n'est pas un plugin
        return False

def next_letter(letter):
    # Vérifier si la dernière lettre est bien une lettre de l'alphabet
    if letter.isalpha():
        # Obtenir le code ASCII de la lettre
        ascii_value = ord(letter)

        # Calculer le code ASCII de la lettre suivante
        # Si c'est 'z', revenir à 'a'
        if letter == 'z':
            next_ascii_value = ord('a')
        # Si c'est 'Z', revenir à 'A'
        elif letter == 'Z':
            next_ascii_value = ord('A')
        else:
            next_ascii_value = ascii_value + 1

        # Obtenir la lettre correspondant au nouveau code ASCII
        next_letter = chr(next_ascii_value)
        
        return next_letter
    else:
        # Si la dernière lettre n'est pas une lettre, retourner la chaîne inchangée
        return "a"
    
def get_filename_without_extension(file_path):
    return os.path.splitext(os.path.basename(file_path))[0]

def main(solution_path, reference_csproj):
    # Step 1: Identify all .csproj files
    csproj_files = find_csproj_files(solution_path)

    # Step 2: Get the version from the reference .csproj
    now = datetime.now()
    versionTheoryShort = f"{now.strftime('%y')}w{now.strftime('%U')}"
    versionTheory = f"1.0.0-{versionTheoryShort}"
    version = get_version_from_csproj(reference_csproj)
    if version is None:
        raise ValueError(f"Version not found in {reference_csproj}")
    
    if version.startswith(versionTheory):
        nLetter = next_letter(version[-1])
        versionTheoryShort = versionTheoryShort + nLetter
        version = versionTheory + nLetter
    else:
        version = versionTheory + "a"

    # Step 3: Set the same version on all .csproj files
    for csproj_file in csproj_files:
        set_version_in_csproj(csproj_file, version)

    # Step 4: Publish each project
    tmp_dir = ".tmp"
    out_dir = "assets/plugins"

    # Créer les dossiers tmp et out s'ils n'existent pas
    os.makedirs(tmp_dir, exist_ok=True)
    os.makedirs(out_dir, exist_ok=True)

    for csproj_file in csproj_files:
        if project_is_plugin(csproj_file) == False:
            continue
        publish_project(csproj_file, tmp_dir)

        # Vérifier le nombre de fichiers .dll dans tmp
        dll_files = [f for f in os.listdir(tmp_dir) if f.endswith(".dll")]
        if len(dll_files) > 1:
            zip_dll_files(tmp_dir, out_dir, get_filename_without_extension(csproj_file), versionTheoryShort)
        elif len(dll_files) == 1:
            move_single_dll(tmp_dir, out_dir, versionTheoryShort)

        # Nettoyer le dossier tmp
        shutil.rmtree(tmp_dir)

if __name__ == "__main__":
    solution_path = "./QMTGroup.sln"
    reference_csproj = "./QMTGroup.Camera/QMTGroup.Camera.csproj"
    main(solution_path, reference_csproj)

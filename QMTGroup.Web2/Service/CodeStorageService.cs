namespace QMTGroup.Web.Service;

public class CodeStorageService
{
    private static Dictionary<string, string> _codeStorage = new Dictionary<string, string>();

    public CodeStorageService()
    {

    }

    public string GetCode(string id)
    {
        if (_codeStorage.ContainsKey(id))
        {
            return _codeStorage[id];
        }
        return "-- Fonction pour calculer le salaire d'un employé après impôts\nfunction calculerSalaireBrut(emp)\n    local tauxImpot = 0.2  -- Taux d'imposition de 20%\n    return emp.salaire * (1 - tauxImpot)\nend\n\n-- Fonction pour afficher des informations sur l'employé\nfunction afficherInfoEmploye(emp)\n    print(\"Nom: \" .. emp.nom)\n    print(\"Poste: \" .. emp.poste)\n    print(\"Salaire Brut: \" .. emp.salaire)\n    print(\"Salaire après impôt: \" .. calculerSalaireBrut(emp))\nend\n\nfunction init()\n  employes = {\n      {nom = \"Alice\", poste = \"Développeuse\", salaire = 3500},\n      {nom = \"Bob\", poste = \"Manager\", salaire = 5000},\n      {nom = \"Charlie\", poste = \"Analyste\", salaire = 4200},\n  }\nend\n\n-- Création d'une table avec des employés\nemployes = {}\n\n-- Fonction qui retourne un itérateur pour parcourir les employés\nfunction itererEmployes()\n    local i = 0\n    return function()\n        i = i + 1\n        if employes[i] then\n            return employes[i]\n        end\n    end\nend\n\n-- Fonction pour traiter la paie des employés\nfunction execute()\n    local iter = itererEmployes()\n\n    -- Utilisation de la fonction anonyme pour appliquer une action sur chaque employé\n    local success, errorMsg = pcall(function()\n        for emp in iter do\n            afficherInfoEmploye(emp)\n            print(\"------\")\n        end\n    end)\n\n    if not success then\n        print(\"Erreur: \" .. errorMsg)\n    end\nend\n";
    }

    public void SaveCode(string id, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            _codeStorage.Remove(id);
        else
            _codeStorage[id] = code;
    }
}

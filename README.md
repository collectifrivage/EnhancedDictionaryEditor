# Éditeur de clé de dictionaire rehaussé Sigmund

[![Build status](https://sigmundftw.visualstudio.com/EnhancedDictionaryEditor/_apis/build/status/EnhancedDictionaryEditor%20Build%20pipeline)](https://sigmundftw.visualstudio.com/EnhancedDictionaryEditor/_build/latest?definitionId=78)

Permet de 
- Voir la liste des clés
- Créer des clés
- Modifier des clés
- Supprimer des clés
- Rechercher par identifiant ou par valeur des clés

## Pour installer dans votre projet Umbraco

```
Install-Package EnhancedDictionaryEditor
```

## Installation développeur

- Clonez le repository
- Ouvrez le projet dans Visual Studio 2017
- Installez le package nuget: `UmbracoCms`
- Copiez le contenu des fichiers situés dans le dossier Sigmund.EnhancedDictionaryEditor\Transforms\config dans les fichiers de configuration d'umbraco.
- Faites un build du projet.
- Configurez IIS pour le projet web Sigmund.EnhancedDictionaryEditor
- Accédez à `http://{adresse_dans_iis}/umbraco
- Installez la base de donnée à l'aide de l'assistant.
- Donnez accès à votre groupe d'utilisateur "Administrators" à la section du plugin.
- Accédez à l'interface du plugin dans la barre de gauche.

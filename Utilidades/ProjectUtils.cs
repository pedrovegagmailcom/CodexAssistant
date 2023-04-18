namespace CodexAssistant.Utilidades
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class ProjectUtils
    {
        public static List<string> GetEditableFiles(string projectPath)
        {
            List<string> editableFiles = new List<string>();

            // Obtener la extensión de archivo de los archivos editables (puedes cambiarla según tus necesidades)
            string[] editableExtensions = { ".cs", ".xaml" };

            // Recorrer el árbol de directorios del proyecto
            TraverseDirectory(projectPath, editableExtensions, editableFiles);

            return editableFiles;
        }

        private static void TraverseDirectory(string directoryPath, string[] editableExtensions, List<string> editableFiles)
        {
            try
            {
                // Obtener todos los archivos del directorio actual
                string[] allFiles = Directory.GetFiles(directoryPath);

                // Filtrar los archivos editables
                foreach (string file in allFiles)
                {
                    string extension = Path.GetExtension(file);

                    if (Array.Exists(editableExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                    {
                        editableFiles.Add(file);
                    }
                }

                // Recorrer los subdirectorios
                string[] subDirectories = Directory.GetDirectories(directoryPath);
                foreach (string subDirectory in subDirectories)
                {
                    TraverseDirectory(subDirectory, editableExtensions, editableFiles);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Manejar excepción de acceso no autorizado a directorios
            }
        }
    }

}

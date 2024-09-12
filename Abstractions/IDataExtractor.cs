namespace SoporteLLM.Abstractions
{
    public interface IDataExtractor
    {
        /// <summary>
        /// Extrae datos en particular de un archivo y los guarda en un archivo nuevo
        /// </summary>
        /// <param name="fileName">Archivo del cual leer los datos</param>
        /// <param name="extractedFileName">Archivo nuevo con los datos a extraer</param>
        Task ExtractDataAsync(string fileName, string extractedFileName);
    }
}

using System;

namespace Domain.Utils.Password;

public class Converts_Utils
{
    /// <summary>
    /// Converte um Guid para sua representação em string.
    /// </summary>
    /// <param name="id">O Guid a ser convertido.</param>
    /// <returns>A representação em string do Guid.</returns>
    public static string GuidToString(Guid id)
    {
        return id.ToString();
    }


    /// <summary>
    /// Converte uma string para um Guid.
    /// </summary>
    /// <param name="guidString">A representação em string do Guid.</param>
    /// <returns>O Guid correspondente.</returns>
    public static Guid StringToGuid(string guidString)
    {
        return Guid.Parse(guidString);
    }

}

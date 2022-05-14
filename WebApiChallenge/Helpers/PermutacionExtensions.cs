using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiChallenge.Helpers
{
    public static class PermutacionExtensions
    {
        public static IEnumerable<T[]> Permutaciones<T>(this IEnumerable<T> coleccion, int k)
        {
            List<T[]> resultado = new List<T[]>();

            // Primero validamos si el conjunto viene vacio.
            if (k == 0)
                resultado.Add(new T[0] { });
            else
            {
                var secActual = 1;
                foreach (var element in coleccion)
                    // Combinamos cada elemento (k - 1) menos las combinaciones posteriores (Recursividad)
                    resultado.AddRange(coleccion
                        .Skip(Math.Min(Interlocked.Increment(ref secActual), secActual - 1))
                        .Permutaciones(k - 1)
                        .Select(combinacion => (new T[] { element })
                        .Concat(combinacion).ToArray()));
            }

            return resultado;
        }
    }
}

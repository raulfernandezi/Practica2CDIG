using UnityEngine;

[CreateAssetMenu(fileName = "Receta", menuName = "Receta")]
public class Receta : ScriptableObject
{
    public TipoElemento[] ingredientes;
    public TipoElemento elementoPrincipal;
    public TipoElemento resultado;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Receta", menuName = "Receta")]
public class Receta : ScriptableObject
{
    public Elemento.TipoElemento[] ingredientes;
    public Elemento.TipoElemento elementoPrincipal;
    public Elemento.TipoElemento resultado;
}

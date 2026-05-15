using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Elemento : MonoBehaviour
{
    private static TipoElemento[] utensilios = { TipoElemento.BANDEJA, TipoElemento.PLATO, TipoElemento.SARTEN };

    private static TipoElemento[] ingredientesBasicos = { TipoElemento.PAN, TipoElemento.LECHE, TipoElemento.HUEVO, TipoElemento.AZUCAR
                , TipoElemento.CANELA, TipoElemento.ACEITE };

    private static TipoElemento[] elementosPrincipales = { TipoElemento.BANDEJA, TipoElemento.PLATO, TipoElemento.SARTEN, TipoElemento.HUEVO
            , TipoElemento.AZUCAR, TipoElemento.AZUCARCANELA, TipoElemento.SARTENACEITE};


    public static int NUM_UTENSILIOS = Elemento.utensilios.Length;
    public static int NUM_INGREDIENTES = Elemento.ingredientesBasicos.Length;

    [SerializeField] private TipoElemento tipoElemento;

    public TipoElemento GetTipoElemento() { return this.tipoElemento; }

    public static List<TipoElemento> GetListaUtensilios()
    {
        return utensilios.ToList();
    }

    public static bool EsUtensilio(TipoElemento elemento)
    {
        return GetListaUtensilios().Contains(elemento);
    }

    public static List<TipoElemento> GetListaIngredientesBasicos()
    {
        return ingredientesBasicos.ToList();
    }

    public static bool EsIngredienteBasico(TipoElemento elemento)
    {
        return GetListaIngredientesBasicos().Contains(elemento);
    }
}

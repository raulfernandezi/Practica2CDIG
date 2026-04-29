using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Elemento:  MonoBehaviour
{
    private static TipoElemento[] utensilios = { TipoElemento.BANDEJA, TipoElemento.PLATO, TipoElemento.SARTEN };

    private static TipoElemento[] ingredientesBasicos = { TipoElemento.PAN, TipoElemento.LECHE, TipoElemento.HUEVO, TipoElemento.AZUCAR
                , TipoElemento.CANELA, TipoElemento.ACEITE };

    private static TipoElemento[] elementosPrincipales = { TipoElemento.BANDEJA, TipoElemento.PLATO, TipoElemento.SARTEN, TipoElemento.HUEVO
            , TipoElemento.AZUCAR, TipoElemento.AZUCARCANELA, TipoElemento.SARTENACEITE};

    public enum TipoElemento
    {
        PAN, LECHE, HUEVO, AZUCAR, CANELA, ACEITE, BANDEJA, SARTEN, PLATO, PANMOJADO, PANREBOZADO, SARTENACEITE, PANFRITO,
        AZUCARCANELA, PANDULCE, TORRIJA
    }

    [SerializeField] private TipoElemento tipoElemento;


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

    public static bool EsIngredientesBasico(TipoElemento elemento)
    {
        return GetListaIngredientesBasicos().Contains(elemento);
    }

    public static List<TipoElemento> GetListaElementosPrincipales()
    {
        return elementosPrincipales.ToList();
    }

    public static bool EsElementoPrincipal(TipoElemento elemento)
    {
        return GetListaElementosPrincipales().Contains(elemento);
    }
}

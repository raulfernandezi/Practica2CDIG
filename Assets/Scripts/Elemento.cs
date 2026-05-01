using System;
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

    public static int NUM_UTENSILIOS = Elemento.utensilios.Length;
    public static int NUM_INGREDIENTES = Elemento.ingredientesBasicos.Length;

    public enum TipoElemento
    {
        PAN, LECHE, HUEVO, AZUCAR, CANELA, ACEITE, BANDEJA, SARTEN, PLATO, PANMOJADO, PANREBOZADO, SARTENACEITE, PANFRITO,
        AZUCARCANELA, PANDULCE, TORRIJA
    }

    bool mostrarNombre = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            mostrarNombre = !mostrarNombre;
        }
    }

    void OnGUI()
    {
        if (mostrarNombre)
        {
            GUI.TextField(new Rect(10, 10, 200, 20), "Nombre de prueba");
        }
    }

    public EventHandler<ElementoEventArgs> elementoDetectado;

    public EventHandler<ElementoEventArgs> elementoPerdido;

    public class ElementoEventArgs : EventArgs {
        public TipoElemento tipoElemento;
    }

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

    public static List<TipoElemento> GetListaElementosPrincipales()
    {
        return elementosPrincipales.ToList();
    }

    public static bool EsElementoPrincipal(TipoElemento elemento)
    {
        return GetListaElementosPrincipales().Contains(elemento);
    }

    public void ElementoDetectado() {
        elementoDetectado?.Invoke(this, new ElementoEventArgs
        {
            tipoElemento = tipoElemento
        });
    }

    public void ElementoPerdido()
    {
        elementoPerdido?.Invoke(this, new ElementoEventArgs
        {
            tipoElemento = tipoElemento
        });
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;



public class Elemento : MonoBehaviour
{
    private static TipoElemento[] utensilios = { TipoElemento.BANDEJA, TipoElemento.PLATO, TipoElemento.SARTEN };

    private static TipoElemento[] ingredientesBasicos = { TipoElemento.PAN, TipoElemento.LECHE, TipoElemento.HUEVO, TipoElemento.AZUCAR
                , TipoElemento.CANELA, TipoElemento.ACEITE };

    private static TipoElemento[] elementosPrincipales = { TipoElemento.BANDEJA, TipoElemento.PLATO, TipoElemento.SARTEN, TipoElemento.HUEVO
            , TipoElemento.AZUCAR, TipoElemento.AZUCARCANELA, TipoElemento.SARTENACEITE};


    public static int NUM_UTENSILIOS = Elemento.utensilios.Length;
    public static int NUM_INGREDIENTES = Elemento.ingredientesBasicos.Length;

    bool mostrarNombre = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            mostrarNombre = !mostrarNombre;
        }
    }

    ObserverBehaviour observer;

    void Start()
    {
        observer = GetComponent<ObserverBehaviour>();
    }

    void OnGUI()
    {
        if (!mostrarNombre) return;
        if (observer.TargetStatus.Status == Status.TRACKED)
        {
            Vector3 offset = transform.position + Vector3.right * 2.0f;
            Vector3 posPantalla = Camera.main.WorldToScreenPoint(offset);
            posPantalla.y = Screen.height - posPantalla.y;
            GUIStyle estilo = new GUIStyle(GUI.skin.textField);
            estilo.fontSize = 50;
            GUI.Label(new Rect(posPantalla.x, posPantalla.y, 250, 60), tipoElemento.ToString(), estilo);
        }
    }

    public EventHandler<ElementoEventArgs> elementoDetectado;

    public EventHandler<ElementoEventArgs> elementoPerdido;

    public class ElementoEventArgs : EventArgs
    {
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
}

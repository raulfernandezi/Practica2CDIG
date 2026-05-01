using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static Elemento;

public class Controlador : MonoBehaviour
{
    [SerializeField] Elemento[] elementos;
    [SerializeField] TextMeshProUGUI textoReceta;

    private int numUtensilios;
    private int numIngredientes;

    void Start()
    {
        foreach(Elemento e in elementos)
        {
            e.elementoDetectado += DeteccionElemento;
            e.elementoPerdido += DeteccionPerdido;
        }
        numIngredientes = 0;
        numUtensilios = 0;
        ActualizarTextoReceta();
    }



    private void DeteccionElemento(object o, ElementoEventArgs e) {
        if (Elemento.EsUtensilio(e.tipoElemento)) {
            numUtensilios++;
        }
        if (Elemento.EsIngredienteBasico(e.tipoElemento)) {
            numIngredientes++;
        }
        ActualizarTextoReceta();
    }

    private void DeteccionPerdido(object o, ElementoEventArgs e)
    {
        if (Elemento.EsUtensilio(e.tipoElemento))
        {
            numUtensilios++;
        }
        if (Elemento.EsIngredienteBasico(e.tipoElemento))
        {
            numIngredientes++;
        }
        ActualizarTextoReceta();
    }

    private void ActualizarTextoReceta() {
        if (numUtensilios < Elemento.NUM_UTENSILIOS && numIngredientes < Elemento.NUM_INGREDIENTES)
        {
            textoReceta.text = "Faltan elementos";
            textoReceta.color = Color.red;
        }
        else if (numUtensilios < Elemento.NUM_UTENSILIOS)
        {
            textoReceta.text = "Faltan utensilios";
            textoReceta.color = Color.red;
        }
        else if (numIngredientes < Elemento.NUM_INGREDIENTES)
        {
            textoReceta.text = "Faltan ingredientes";
            textoReceta.color = Color.red;
        }
        else {
            textoReceta.text = "Receta completa";
            textoReceta.color = Color.green;
        }
    }
}

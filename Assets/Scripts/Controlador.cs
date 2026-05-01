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

    private void Start()
    {
        foreach(Elemento e in elementos)
        {
            e.elementoDetectado += DeteccionElemento;
        }
        numIngredientes = 0;
        numUtensilios = 0;
        textoReceta.text = ActualizarTextoReceta();
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

    private string ActualizarTextoReceta() {
        string result;
        if (numUtensilios < Elemento.NUM_UTENSILIOS && numIngredientes < Elemento.NUM_INGREDIENTES)
        {
            result = "Faltan elementos";
        }
        else if (numUtensilios < Elemento.NUM_UTENSILIOS)
        {
            result = "Faltan utensilios";
        }
        else if (numIngredientes < Elemento.NUM_INGREDIENTES)
        {
            result = "Faltan ingredientes";
        }
        else {
            result = "Receta completa";
        }
        return result;
    }
}

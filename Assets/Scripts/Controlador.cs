using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using static Elemento;

public class Controlador : MonoBehaviour
{
    [SerializeField] Elemento[] elementos;
    List<Elemento> elementosActivos;
    [SerializeField] List<Receta> recetas;
    [SerializeField] TextMeshProUGUI textoReceta;
    [SerializeField] Dictionary<TipoElemento, GameObject> prefabs;
     
    private int numUtensilios;
    private int numIngredientes;

    void Start()
    {
        foreach(Elemento e in elementos)
        {
            e.elementoDetectado += DeteccionElemento;
            e.elementoPerdido += DeteccionElementoPerdido;
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
        elementosActivos.Add((Elemento)o);
        ActualizarReceta();
        ActualizarTextoReceta();
    }

    private void DeteccionElementoPerdido(object o, ElementoEventArgs e)
    {
        if (Elemento.EsUtensilio(e.tipoElemento))
        {
            numUtensilios--;
        }
        if (Elemento.EsIngredienteBasico(e.tipoElemento))
        {
            numIngredientes--;
        }
        elementosActivos.Remove((Elemento)o);
        ActualizarReceta();
        ActualizarTextoReceta();
    }

    private void ActualizarReceta()
    {
        Boolean esPrincipal = false;
        foreach (Receta receta in recetas) {
            TipoElemento principal = receta.elementoPrincipal;

            foreach (Elemento elemento in elementosActivos) {
               esPrincipal = elemento.GetTipoElemento().Equals(principal);
               if (esPrincipal) { break; }
            }

            if (esPrincipal) {
                List<TipoElemento> ingredientes = receta.ingredientes.ToList();
                int presentes = ingredientes.Count;
                Boolean ingredientesPresentes = false;
                foreach (Elemento elemento in elementosActivos)
                {
                    if (ingredientes.Contains(elemento.GetTipoElemento())) { presentes--; }
                    if (presentes == 0) {ingredientesPresentes = true; break; }
                }

                if (ingredientesPresentes)
                {
                    TipoElemento resultado = receta.resultado;
                    //Continuar por aquí
                }
            }
        }
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

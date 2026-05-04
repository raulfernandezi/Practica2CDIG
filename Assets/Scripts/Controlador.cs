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
    [SerializeField] List<TipoElementoPrefab> prefabs;
     
    private int numUtensilios;
    private int numIngredientes;

    [Serializable]
    public struct TipoElementoPrefab
    {
        public TipoElemento tipoElemento;
        public GameObject prefab;
    }


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
    private void ActualizarReceta_Ver2()
    {
        foreach (Receta receta in recetas)
        {
            TipoElemento principal = receta.elementoPrincipal;
            Elemento refPrincipal = null;

            // 1. Buscamos si el elemento principal está activo y guardamos su referencia
            foreach (Elemento elemento in elementosActivos)
            {
                if (elemento.GetTipoElemento().Equals(principal))
                {
                    refPrincipal = elemento;
                    break;
                }
            }

            // Si encontramos el elemento principal, comprobamos los ingredientes
            if (refPrincipal != null)
            {
                // Copiamos la lista de ingredientes necesarios. 
                // Usar Remove() es más seguro que un contador por si hay objetos del mismo tipo duplicados.
                List<TipoElemento> ingredientesFaltantes = receta.ingredientes.ToList();

                foreach (Elemento elemento in elementosActivos)
                {
                    // Si el elemento detectado es uno de los que nos falta, lo tachamos de la lista
                    if (ingredientesFaltantes.Contains(elemento.GetTipoElemento()))
                    {
                        ingredientesFaltantes.Remove(elemento.GetTipoElemento());
                    }

                    // Si ya no faltan ingredientes, dejamos de buscar
                    if (ingredientesFaltantes.Count == 0) { break; }
                }

                // 2. Si están todos los ingredientes presentes, creamos el resultado
                if (ingredientesFaltantes.Count == 0)
                {
                    TipoElemento resultado = receta.resultado;

                    // Comprobamos que no esté presente en escena
                    
                    if (!elementosActivos.Any(e => e.GetTipoElemento().Equals(resultado)))
                    {
                        GameObject prefabAResultado = prefabs[].;

                        // Instanciamos el resultado en la posición del elemento principal
                        // (Asumiendo que la clase Elemento hereda de MonoBehaviour y tiene un Transform)
                        Instantiate(prefabAResultado, refPrincipal.transform.position, Quaternion.identity);

                        Debug.Log("¡Receta completada!: " + resultado.ToString());

                        // Rompemos el bucle principal para no fabricar múltiples recetas a la vez
                        break;
                    }
                    else
                    {
                        Debug.LogWarning("No hay prefab asignado en el diccionario para: " + resultado);
                    }
                }
            }
        }
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
                    Debug.Log(resultado.ToString());
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

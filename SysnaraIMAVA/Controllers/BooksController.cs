using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SysnaraIMAVA.Controllers
{
    public class BooksController : Controller
    {
        // Acción principal para la vista index
        public IActionResult Index()
        {
            // Obtener el rol del usuario desde la sesión
            var userRole = HttpContext.Session.GetString("Role");

            // Normalizar el nombre del rol (convertir a minúsculas)
            var normalizedUserRole = userRole?.ToLower();

            // Diccionario de libros por grado (claves en minúsculas)
            var librosPorGrado = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "kindergarten", new Dictionary<string, string> {
                    { "MATH", "/LibrosBooks/Kindergarten/Math Kinder.pdf" },
                    { "PHONICS", "/LibrosBooks/Kindergarten/PHONICS KINDER.pdf" },
                    { "PRE-HANDWRITING", "/LibrosBooks/Kindergarten/PRE HINDWRITING KINDER.pdf" },
                    { "READING", "/LibrosBooks/Kindergarten/READING KINDER.pdf" },
                }},
                { "preparatory", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Preparatory/LANGUAGE PREPA.pdf" },
                    { "MATH", "/LibrosBooks/Preparatory/MATH PREPA.pdf" },
                    { "PHONICS", "/LibrosBooks/Preparatory/PHONICS PREPA.pdf" },
                    { "PRE-WRITING", "/LibrosBooks/Preparatory/PRE-WRITING PREPA.pdf" },
                    { "READING", "/LibrosBooks/Preparatory/READING PREPA.pdf" },
                }},
                { "first", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/First/LANGUAGE PRIMERO.pdf" },
                    { "MATH", "/LibrosBooks/First/MATH PRIMERO.pdf" },
                    { "PHONICS", "/LibrosBooks/First/PHONICS PRIMERO.pdf" },
                    { "READING", "/LibrosBooks/First/READING PRIMERO.pdf" },
                    { "SPELLING", "/LibrosBooks/First/SPELLING PRIMERO.pdf" },
                    { "SCIENCE I", "/LibrosBooks/First/SCIENCE 1 PRIMERO.pdf" },
                    //{ "SCIENCE II", "/LibrosBooks/First/SCIENCE 2 PRIMERO.pdf" },
                }},
                { "second", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Second/Language Arts Workbook Grade 2.pdf" },
                    { "MATH", "/LibrosBooks/Second/Math Workbook Grade 2.pdf" },
                    { "PHONICS", "/LibrosBooks/Second/Spectrum Phonics Workbook Grade 2.pdf" },
                    { "READING", "/LibrosBooks/Second/Spectrum Reading Workbook Grade 2.pdf" },
                    { "SPELLING", "/LibrosBooks/Second/Spectrum Spelling Workbook Grade 2.pdf" },
                    { "SCIENCE II", "/LibrosBooks/Second/Science 2.pdf" },
                }},
                { "third", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Third/Language Arts Workbook Grade 3.pdf" },
                    { "MATH", "/LibrosBooks/Third/Math Workbook Grade 3.pdf" },
                    { "PHONICS", "/LibrosBooks/Third/Phonics Workbook Grade 3.pdf" },
                    { "GEOGRAPHY", "/LibrosBooks/Third/Spectrum Geography Workbook Grade 3.pdf" },
                    { "READING", "/LibrosBooks/Third/Spectrum Reading Workbook Grade 3.pdf" },
                    { "SCIENCE", "/LibrosBooks/Third/Spectrum Science Workbook Grade 3.pdf" },
                    { "SPELLING", "/LibrosBooks/Third/Spectrum Spelling Workbook Grade 3.pdf" },
                }},
                { "fourth", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Fourth/Language Arts Workbook Grade 4.pdf" },
                    { "MATH", "/LibrosBooks/Fourth/Math Workbook Grade 4.pdf" },
                    { "PHONICS", "/LibrosBooks/Fourth/Phonics Workbook Grade 4.pdf" },
                    { "GEOGRAPHY", "/LibrosBooks/Fourth/Spectrum Geography Workbook Grade 4.pdf" },
                    { "READING", "/LibrosBooks/Fourth/Spectrum Reading Workbook Grade 4.pdf" },
                    { "SCIENCE", "/LibrosBooks/Fourth/Spectrum Science Workbook Grade 4.pdf" },
                    { "SPELLING", "/LibrosBooks/Fourth/Spectrum Spelling Workbook Grade 4.pdf" },
                }},
                { "fifth", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Fifth/Language Arts Workbook Grade 5.pdf" },
                    { "MATH", "/LibrosBooks/Fifth/Math Workbook Grade 5.pdf" },
                    { "PHONICS", "/LibrosBooks/Fifth/Phonics Workbook Grade 5.pdf" },
                    { "GEOGRAPHY", "/LibrosBooks/Fifth/Spectrum Geography Workbook Grade 5.pdf" },
                    { "READING", "/LibrosBooks/Fifth/Spectrum Reading Workbook Grade 5.pdf" },
                    { "SCIENCE", "/LibrosBooks/Fifth/Spectrum Science Workbook Grade 5.pdf" },
                    { "SPELLING", "/LibrosBooks/Fifth/Spectrum Spelling Workbook Grade 5.pdf" },
                }},
                { "sixth", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Sixth/Language Arts Workbook Grade 6.pdf" },
                    { "MATH", "/LibrosBooks/Sixth/Math Workbook Grade 6.pdf" },
                    { "PHONICS", "/LibrosBooks/Sixth/Phonics Workbook Grade 6.pdf" },
                    { "GEOGRAPHY", "/LibrosBooks/Sixth/Spectrum Geography Workbook Grade 6.pdf" },
                    { "READING", "/LibrosBooks/Sixth/Spectrum Reading Workbook Grade 6.pdf" },
                    { "SCIENCE", "/LibrosBooks/Sixth/Spectrum Science Workbook Grade 6.pdf" },
                    { "SPELLING", "/LibrosBooks/Sixth/Spectrum Spelling Workbook Grade 6.pdf" },
                }},
                { "seventh", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Seventh/LANGUAGE 7.pdf" },
                    { "MATH", "/LibrosBooks/Seventh/MATH 7.pdf" },
                    { "READING", "/LibrosBooks/Seventh/READING 7.pdf" },
                    { "SCIENCE", "/LibrosBooks/Seventh/SCIENCE 7.pdf" },
                }},
                { "eighth", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Eighth/LANGUAGE 8.pdf" },
                    { "MATH", "/LibrosBooks/Eighth/MATH 8.pdf" },
                    { "SCIENCE", "/LibrosBooks/Eighth/SCIENCE 8.pdf" },
                    { "SOCIAL STUDIES", "/LibrosBooks/Eighth/Social Studies  8.pdf" },
                }},
                { "ninth", new Dictionary<string, string> {
                    { "GRAMMAR", "/LibrosBooks/Ninth/Grammar in Context - 2, 9no grado.pdf" },
                    { "LITERATURE", "/LibrosBooks/Ninth/LITERATURA  9.pdf" },
                }},
                { "tenth", new Dictionary<string, string> {
                    { "GRAMMAR", "/LibrosBooks/Tenth/Grammar in Context - 2, 10mo grado.pdf" },
                    { "LITERATURE", "/LibrosBooks/Tenth/LITERATURA 10MO.pdf" },
                }},
                { "eleventh", new Dictionary<string, string> {
                    { "ANÁLISIS Y DISEÑO", "/LibrosBooks/Eleventh/Analisis de Sistemas 1 - 2.pdf" },
                    { "INFORMÁTICA", "/LibrosBooks/Eleventh/Informática 11.pdf" },
                    { "PROGRAMACIÓN", "/LibrosBooks/Eleventh/Programación 1 y 2.pdf" },
                    { "GRAMMAR", "/LibrosBooks/Eleventh/Grammar in Context - 3, 11vo grado.pdf" },
                    { "LITERATURE", "/LibrosBooks/Eleventh/LITERATURA 11VO faltan 27.pdf" },
                }},
                { "twelfth", new Dictionary<string, string> {
                    { "PROGRAMACIÓN I", "/LibrosBooks/Twelfth/Programación I.pdf" },
                    { "PROGRAMACIÓN II", "/LibrosBooks/Twelfth/Programación II.pdf" },
                    { "REDES INFORMÁTICAS", "/LibrosBooks/Twelfth/Redes Informáticas 12.pdf" },
                    { "MANTENIMIENTO Y REPARACIÓN", "/LibrosBooks/Twelfth/Mantenimiento de Computadoras.pdf" },
                    { "DISEÑO WEB", "/LibrosBooks/Twelfth/Diseño Web.pdf" },
                    { "ANÁLISIS Y DISEÑO", "/LibrosBooks/Twelfth/Analisis y Diseño de Sistemas 1y2.pdf" },
                    { "INFORMÁTICA", "/LibrosBooks/Twelfth/Informática.pdf" },
                    { "GRAMMAR (2)", "/LibrosBooks/Twelfth/Grammar in Context - 2, 9no grado.pdf" },
                    { "GRAMMAR (3)", "/LibrosBooks/Twelfth/Grammar in Context - 3, 12vo grado faltan 30.pdf" },
                }},
            };

            // Filtrar libros según el rol del usuario
            var librosFiltrados = new Dictionary<string, Dictionary<string, string>>();
            if (librosPorGrado.ContainsKey(normalizedUserRole))
            {
                librosFiltrados.Add(normalizedUserRole, librosPorGrado[normalizedUserRole]);
            }

            // Pasamos el diccionario filtrado a la vista
            return View(librosFiltrados);
        }

        // Acción para previsualizar el libro en PDF
        public IActionResult PrevisualizarLibrosPdf(string grado, string libro)
        {
            var rutaPdf = ObtenerRutaPdf(grado, libro);
            if (rutaPdf != null)
            {
                return View((object)rutaPdf);  // Pasamos la ruta del PDF a la vista
            }
            return NotFound();  // Si no encontramos el libro, mostramos un error 404
        }

        // Método para obtener la ruta del PDF según el grado y el libro
        private string? ObtenerRutaPdf(string grado, string libro)
        {
            var librosPorGrado = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "kindergarten", new Dictionary<string, string> {
                    { "MATH", "/LibrosBooks/Kindergarten/Math Kinder.pdf" },
                    { "PHONICS", "/LibrosBooks/Kindergarten/PHONICS KINDER.pdf" },
                    { "PRE-HANDWRITING", "/LibrosBooks/Kindergarten/PRE HINDWRITING KINDER.pdf" },
                    { "READING", "/LibrosBooks/Kindergarten/READING KINDER.pdf" },
                }},
                { "preparatory", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Preparatory/LANGUAGE PREPA.pdf" },
                    { "MATH", "/LibrosBooks/Preparatory/MATH PREPA.pdf" },
                    { "PHONICS", "/LibrosBooks/Preparatory/PHONICS PREPA.pdf" },
                    { "PRE-WRITING", "/LibrosBooks/Preparatory/PRE-WRITING PREPA.pdf" },
                    { "READING", "/LibrosBooks/Preparatory/READING PREPA.pdf" },
                }},
                { "first", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/First/LANGUAGE PRIMERO.pdf" },
                    { "MATH", "/LibrosBooks/First/MATH PRIMERO.pdf" },
                    { "PHONICS", "/LibrosBooks/First/PHONICS PRIMERO.pdf" },
                    { "READING", "/LibrosBooks/First/READING PRIMERO.pdf" },
                    { "SPELLING", "/LibrosBooks/First/SPELLING PRIMERO.pdf" },
                    { "SCIENCE I", "/LibrosBooks/First/SCIENCE 1 PRIMERO.pdf" },
                    //{ "SCIENCE II", "/LibrosBooks/First/SCIENCE 2 PRIMERO.pdf" },
                }},
                { "second", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Second/Language Arts Workbook Grade 2.pdf" },
                    { "MATH", "/LibrosBooks/Second/Math Workbook Grade 2.pdf" },
                    { "PHONICS", "/LibrosBooks/Second/Spectrum Phonics Workbook Grade 2.pdf" },
                    { "READING", "/LibrosBooks/Second/Spectrum Reading Workbook Grade 2.pdf" },
                    { "SPELLING", "/LibrosBooks/Second/Spectrum Spelling Workbook Grade 2.pdf" },
                    { "SCIENCE II", "/LibrosBooks/Second/Science 2.pdf" },
                }},
                { "third", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Third/Language Arts Workbook Grade 3.pdf" },
                    { "MATH", "/LibrosBooks/Third/Math Workbook Grade 3.pdf" },
                    { "PHONICS", "/LibrosBooks/Third/Phonics Workbook Grade 3.pdf" },
                    { "GEOGRAPHY", "/LibrosBooks/Third/Spectrum Geography Workbook Grade 3.pdf" },
                    { "READING", "/LibrosBooks/Third/Spectrum Reading Workbook Grade 3.pdf" },
                    { "SCIENCE", "/LibrosBooks/Third/Spectrum Science Workbook Grade 3.pdf" },
                    { "SPELLING", "/LibrosBooks/Third/Spectrum Spelling Workbook Grade 3.pdf" },
                }},
                { "fourth", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Fourth/Language Arts Workbook Grade 4.pdf" },
                    { "MATH", "/LibrosBooks/Fourth/Math Workbook Grade 4.pdf" },
                    { "PHONICS", "/LibrosBooks/Fourth/Phonics Workbook Grade 4.pdf" },
                    { "GEOGRAPHY", "/LibrosBooks/Fourth/Spectrum Geography Workbook Grade 4.pdf" },
                    { "READING", "/LibrosBooks/Fourth/Spectrum Reading Workbook Grade 4.pdf" },
                    { "SCIENCE", "/LibrosBooks/Fourth/Spectrum Science Workbook Grade 4.pdf" },
                    { "SPELLING", "/LibrosBooks/Fourth/Spectrum Spelling Workbook Grade 4.pdf" },
                }},
                { "fifth", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Fifth/Language Arts Workbook Grade 5.pdf" },
                    { "MATH", "/LibrosBooks/Fifth/Math Workbook Grade 5.pdf" },
                    { "PHONICS", "/LibrosBooks/Fifth/Phonics Workbook Grade 5.pdf" },
                    { "GEOGRAPHY", "/LibrosBooks/Fifth/Spectrum Geography Workbook Grade 5.pdf" },
                    { "READING", "/LibrosBooks/Fifth/Spectrum Reading Workbook Grade 5.pdf" },
                    { "SCIENCE", "/LibrosBooks/Fifth/Spectrum Science Workbook Grade 5.pdf" },
                    { "SPELLING", "/LibrosBooks/Fifth/Spectrum Spelling Workbook Grade 5.pdf" },
                }},
                { "sixth", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Sixth/Language Arts Workbook Grade 6.pdf" },
                    { "MATH", "/LibrosBooks/Sixth/Math Workbook Grade 6.pdf" },
                    { "PHONICS", "/LibrosBooks/Sixth/Phonics Workbook Grade 6.pdf" },
                    { "GEOGRAPHY", "/LibrosBooks/Sixth/Spectrum Geography Workbook Grade 6.pdf" },
                    { "READING", "/LibrosBooks/Sixth/Spectrum Reading Workbook Grade 6.pdf" },
                    { "SCIENCE", "/LibrosBooks/Sixth/Spectrum Science Workbook Grade 6.pdf" },
                    { "SPELLING", "/LibrosBooks/Sixth/Spectrum Spelling Workbook Grade 6.pdf" },
                }},
                { "seventh", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Seventh/LANGUAGE 7.pdf" },
                    { "MATH", "/LibrosBooks/Seventh/MATH 7.pdf" },
                    { "READING", "/LibrosBooks/Seventh/READING 7.pdf" },
                    { "SCIENCE", "/LibrosBooks/Seventh/SCIENCE 7.pdf" },
                }},
                { "eighth", new Dictionary<string, string> {
                    { "LANGUAGE", "/LibrosBooks/Eighth/LANGUAGE 8.pdf" },
                    { "MATH", "/LibrosBooks/Eighth/MATH 8.pdf" },
                    { "SCIENCE", "/LibrosBooks/Eighth/SCIENCE 8.pdf" },
                    { "SOCIAL STUDIES", "/LibrosBooks/Eighth/Social Studies  8.pdf" },
                }},
                { "ninth", new Dictionary<string, string> {
                    { "GRAMMAR", "/LibrosBooks/Ninth/Grammar in Context - 2, 9no grado.pdf" },
                    { "LITERATURE", "/LibrosBooks/Ninth/LITERATURA  9.pdf" },
                }},
                { "tenth", new Dictionary<string, string> {
                    { "GRAMMAR", "/LibrosBooks/Tenth/Grammar in Context - 2, 10mo grado.pdf" },
                    { "LITERATURE", "/LibrosBooks/Tenth/LITERATURA 10MO.pdf" },
                }},
                { "eleventh", new Dictionary<string, string> {
                    { "ANÁLISIS", "/LibrosBooks/Eleventh/Analisis de Sistemas 1 - 2.pdf" },
                    { "INFORMÁTICA", "/LibrosBooks/Eleventh/Informática 11.pdf" },
                    { "PROGRAMACIÓN", "/LibrosBooks/Eleventh/Programación 1 y 2.pdf" },
                    { "GRAMMAR", "/LibrosBooks/Eleventh/Grammar in Context - 3, 11vo grado.pdf" },
                    { "LITERATURE", "/LibrosBooks/Eleventh/LITERATURA 11VO faltan 27.pdf" },
                }},
                { "twelfth", new Dictionary<string, string> {
                    { "PROGRAMACIÓN I", "/LibrosBooks/Twelfth/Programación I.pdf" },
                    { "PROGRAMACIÓN II", "/LibrosBooks/Twelfth/Programación II.pdf" },
                    { "REDES INFORMÁTICAS", "/LibrosBooks/Twelfth/Redes Informáticas 12.pdf" },
                    { "MANTENIMIENTO Y REPARACIÓN", "/LibrosBooks/Twelfth/Mantenimiento de Computadoras.pdf" },
                    { "DISEÑO WEB", "/LibrosBooks/Twelfth/Diseño Web.pdf" },
                    { "ANÁLISIS Y DISEÑO", "/LibrosBooks/Twelfth/Analisis y Diseño de Sistemas 1y2.pdf" },
                    { "INFORMÁTICA", "/LibrosBooks/Twelfth/Informática.pdf" },
                    { "GRAMMAR (2)", "/LibrosBooks/Twelfth/Grammar in Context - 2, 9no grado.pdf" },
                    { "GRAMMAR (3)", "/LibrosBooks/Twelfth/Grammar in Context - 3, 12vo grado faltan 30.pdf" },
                }},
            };

            return librosPorGrado.ContainsKey(grado) && librosPorGrado[grado].ContainsKey(libro)
                ? librosPorGrado[grado][libro]
                : null;
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SysnaraIMAVA.Models; // Asegúrate de agregar este using

namespace SysnaraIMAVA.Controllers
{
    public class TareasController : Controller
    {
        public IActionResult Index()
        {
            // Obtener el rol del usuario desde la sesión
            var userRole = HttpContext.Session.GetString("Role");

            // Normalizar el nombre del rol (convertir a minúsculas)
            var normalizedUserRole = userRole?.ToLower();

            // Diccionario de clases por grado (claves en minúsculas)
            var clasesPorGrado = new Dictionary<string, List<Clase>>(StringComparer.OrdinalIgnoreCase)
            {
                { "kindergarten", new List<Clase> {
                    new Clase { Titulo = "MATH", Descripcion = "Código de la clase: ch6w6bl", Url = "https://classroom.google.com/u/6/c/NzQ4ODA0NjUzNjc3" },
                    new Clase { Titulo = "PHONICS", Descripcion = "Código de la clase: 4qedmla", Url = "https://classroom.google.com/u/6/c/NzQ4ODA0MjA2MTAx" },
                    new Clase { Titulo = "PRE-HANDWRITING", Descripcion = "Código de la clase: ajy6ub4", Url = "https://classroom.google.com/u/6/c/NzQ4ODA1NzI4MjI2" },
                    new Clase { Titulo = "READING", Descripcion = "Código de la clase: c35dwzm", Url = "https://classroom.google.com/u/6/c/NzQ4ODA1MDg2NDE4" },
                }},
                { "preparatory", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase: 6ntt4jt", Url = "https://classroom.google.com/u/6/c/NzQ4ODA1NTE0MjU0" },
                    new Clase { Titulo = "MATH", Descripcion = "Código de la clase: pdshg22", Url = "https://classroom.google.com/u/6/c/NzQ4ODA3MjcwOTEx" },
                    new Clase { Titulo = "PHONICS", Descripcion = "Código de la clase: 6llfyw4", Url = "https://classroom.google.com/u/6/c/NzQ4ODA3MzEyNzMy" },
                    new Clase { Titulo = "PRE-WRITING", Descripcion = "Código de la clase: 3omstlk", Url = "https://classroom.google.com/u/6/c/NzQ4ODA2NjM4Nzky" },
                    new Clase { Titulo = "READING", Descripcion = "Código de la clase: rrjkxeb", Url = "https://classroom.google.com/u/6/c/NzQ4ODA2MDUyNDEw" },
                }},
                { "first", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase: wuwdljp", Url = "https://classroom.google.com/u/6/c/NzQ4ODA3NzU4OTQ5" },
                    new Clase { Titulo = "MATH", Descripcion = "Código de la clase: ul7vprp", Url = "https://classroom.google.com/u/6/c/NzQ4ODA5NzkxMTg5" },
                    new Clase { Titulo = "PHONICS", Descripcion = "Código de la clase: hrvdg7q", Url = "https://classroom.google.com/u/6/c/NzQ4ODEwMjQxOTI0" },
                    new Clase { Titulo = "READING", Descripcion = "Código de la clase: eki3orz", Url = "https://classroom.google.com/u/6/c/NzQ4ODEwNjMzOTkz" },
                    new Clase { Titulo = "SPELLING", Descripcion = "Código de la clase: 4umim5m", Url = "https://classroom.google.com/u/6/c/NzQ4ODA5NTkxMzk5" },
                }},
                { "second", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase: eutlfgl", Url = "https://classroom.google.com/u/6/c/NzQ4ODEwOTY5MTk3" },
                    new Clase { Titulo = "MATH", Descripcion = "Código de la clase: rsgst6y", Url = "https://classroom.google.com/u/6/c/NzQ4ODEwODg2MTM1" },
                    new Clase { Titulo = "PHONICS", Descripcion = "Código de la clase: hiendwu", Url = "https://classroom.google.com/u/6/c/NzQ4ODEwNTU1OTQ5" },
                    new Clase { Titulo = "READING", Descripcion = "Código de la clase: epmkbaj", Url = "https://classroom.google.com/u/6/c/NzQ4ODExNTU0NzE4" },
                    new Clase { Titulo = "SPELLING", Descripcion = "Código de la clase ffcw6uk:", Url = "https://classroom.google.com/u/6/c/NzQ4ODExNDcwMjc4" },
                }},
                { "third", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase: fo5v6vo", Url = "https://classroom.google.com/u/6/c/NzQ4ODEwODg3NDUz" },
                    new Clase { Titulo = "MATH", Descripcion = "Código de la clase: vndkkc7", Url = "https://classroom.google.com/u/6/c/NzQ4ODEyODU3Njg2" },
                    new Clase { Titulo = "PHONICS", Descripcion = "Código de la clase: xord6cd", Url = "https://classroom.google.com/u/6/c/NzQ4ODEzNjk1MTM1" },
                    new Clase { Titulo = "GEOGRAPHY", Descripcion = "Código de la clase: 24xk4nt", Url = "https://classroom.google.com/u/6/c/NzQ4ODEyMzU4ODI0" },
                    new Clase { Titulo = "READING", Descripcion = "Código de la clase: gqhh6xt", Url = "https://classroom.google.com/u/6/c/Njg4NDQ2NTE3Mjc0" },
                    new Clase { Titulo = "SCIENCE", Descripcion = "Código de la clase: g3czfe7", Url = "https://classroom.google.com/u/6/c/Njg4NDQ2NDcxNzYw" },
                    new Clase { Titulo = "SPELLING", Descripcion = "Código de la clase: tqesdbb", Url = "https://classroom.google.com/u/6/c/NzQ4ODEzMzQ2NTM0" },
                }},
                { "fourth", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase: cfuqvbh", Url = "https://classroom.google.com/u/6/c/NzQ4ODEzMzcyMTc0" },
                    new Clase { Titulo = "MATH", Descripcion = "Código de la clase: befxkux", Url = "https://classroom.google.com/u/6/c/Njg4NDQ2NTU2OTM3" },
                    new Clase { Titulo = "PHONICS", Descripcion = "Código de la clase: zgos4hx", Url = "https://classroom.google.com/u/6/c/Njg4NDQ2NjM4MTk0" },
                    new Clase { Titulo = "GEOGRAPHY", Descripcion = "Código de la clase: lbmdibj", Url = "https://classroom.google.com/u/6/c/Njg4NDQ2NTAwOTA1" },
                    new Clase { Titulo = "READING", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "SCIENCE", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "SPELLING", Descripcion = "Código de la clase:", Url = "" },
                }},
                { "fifth", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "MATH", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "PHONICS", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "GEOGRAPHY", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "READING", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "SCIENCE", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "SPELLING", Descripcion = "Código de la clase:", Url = "" },
                }},
                { "sixth", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "MATH", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "PHONICS", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "GEOGRAPHY", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "READING", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "SCIENCE", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "SPELLING", Descripcion = "Código de la clase:", Url = "" },
                }},
                { "seventh", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "MATH", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "READING", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "SCIENCE", Descripcion = "Código de la clase:", Url = "" },
                }},
                { "eighth", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "MATH", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "SCIENCE", Descripcion = "Código de la clase:", Url = "" },
                    new Clase { Titulo = "SOCIAL STUDIES", Descripcion = "Código de la clase:", Url = "" },
                }},
                { "ninth", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase:", Url = "" },
                }},
                { "tenth", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase:", Url = "" },
                }},
                { "eleventh", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase:", Url = "" },
                }},
                { "twelfth", new List<Clase> {
                    new Clase { Titulo = "LANGUAGE", Descripcion = "Código de la clase:", Url = "" },
                }},
            };

            // Filtrar clases según el rol del usuario
            var clasesFiltradas = new List<Clase>();
            if (clasesPorGrado.ContainsKey(normalizedUserRole))
            {
                clasesFiltradas = clasesPorGrado[normalizedUserRole];
            }

            // Pasamos la lista de clases filtradas a la vista
            return View(clasesFiltradas);
        }
    }
}
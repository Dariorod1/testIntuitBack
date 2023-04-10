using System.Text.RegularExpressions;

namespace testIntuit.Utils
{
    public class Validations
    {
        // Expresión regular para validar la fecha de nacimiento en formato ISO 8601
        const string fechaNac = @"^\d{4}-\d{2}-\d{2}$";

        // Expresión regular para validar el CUIT en formato argentino
        const string cuitFormato = @"^\d{2}-\d{8}-\d{1}$";

        public static bool ValidaDatos(string nombre,string apellido,string cuit,string telefono,string email)
        {
            // Validamos que los campos obligatorios estén presentes
            if (string.IsNullOrEmpty(nombre) ||
                string.IsNullOrEmpty(apellido) ||
                string.IsNullOrEmpty(cuit) ||
                string.IsNullOrEmpty(telefono) ||
                string.IsNullOrEmpty(email))
            {
                return false;
            }
            return true;
        }

        public static bool ValidateEmail(string email)
        {
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return false;
            }
            return true;
        }
        public static bool ValidateCUIT(string cuit)
        {
            // Validamos que el CUIT tenga el formato correcto
            if (!Regex.IsMatch(cuit, cuitFormato))
            {
                return false;
            }
            return true;
        }

    }
}

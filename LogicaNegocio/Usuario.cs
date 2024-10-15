using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LogicaNegocio.Interface;

namespace LogicaNegocio
{
    public abstract class Usuario : IEquatable<Usuario>, IValidate
    {
        #region Atributes
        private string _email;
        private string _password;
        //Debe de comnezar con una letra. consta de 4 a 20 caracteres letras seguido por un @ y un dominio que deben de ser letras de 2 a 10 caracters y debe terminar en .com
        private static string s_EmailRegex = @"^(\w{4,20}@{1})(\w{2,10})(\.com)$";
        //La contrasena va a constar de caracteres de tipo letras numeros o los simbolos especificados debe comenzar con un caracter y terminar con un caracter
        //Y debe tener de 8 a 20 caracteres
        private static string s_PasswordRegex = "^[a-zA-Z0-9!@#$%^&*_+=\\-]{8,20}$";

        #endregion

        #region Methods
        public Usuario(string email, string password)
        {
            _email = email;
            _password = password;
        }
        public Usuario() { }

        public string Email { get => _email; set => _email = value; }

        public string Password { get => _password;  set => _password = value; }
        public void Validate()
        {
            if(_email == null)
            {
                throw new Exception("Ingrese un mail por favor");
            }
            if (_password == null)
            {
                throw new Exception("Ingrese una contrasena por favor");
            }
            if (!ValidarEmail() || !ValidarPassword())
            {
                throw new Exception("Campos usuario/contrasena no validos el nombre de mail debe tener entre 4 y 20 caracteres y la contrasena entre 8 y 20");
            }
        }
        //Utiliza el metodo IsMatch con una expresion regular, en caso de cumplirse la expresion declarada como variable estatica retorna verdadero
        //Sirve para verificar formato
        private bool ValidarEmail()
        {
            return Regex.IsMatch(_email, s_EmailRegex);
        }

        //Igual que email pero cambia la expresion regular
        private bool ValidarPassword()
        {
            return Regex.IsMatch( _password, s_PasswordRegex);
        }

        public override string ToString()
        {
            return $"\n\n--USUARIO--\nEmail: {_email}\n";
        }

        public bool Equals(Usuario? other)
        {
            return _email == other._email;
        }

        public abstract string Rol();

        #endregion

    }
}

using System.ComponentModel.DataAnnotations;

namespace LoginRegistrationInMVCWithDatabase.ViewModel
{
    public class Register
    {
        [Required]
        [Display(Name = "First Name")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string PrimerApellido { get; set; }

        //Required attribute implements validation on Model item that this fields is mandatory for user
        [Required]
        //We are also implementing Regular expression to check if email is valid like a1@test.com
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Correo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }
    }
}